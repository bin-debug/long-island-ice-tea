using EasyVerifyModels;
using Elastic.Clients.Elasticsearch.Requests;
using Elastic.Clients.Elasticsearch;

namespace BureauhouseApi.Services;
public class BureauhouseService
{
    private readonly HttpClient _httpClient;
    private readonly IOptions<ConfigModel> _configModel;
    private readonly CouchbaseService _couchbaseService;
    private readonly ElasticsearchService _elasticsearchService;

    public BureauhouseService(HttpClient httpClient, IOptions<ConfigModel> configModel, 
        CouchbaseService couchbaseService, ElasticsearchService elasticsearchService)
    {
        _httpClient = httpClient;
        _configModel = configModel;
        _couchbaseService = couchbaseService;
        _elasticsearchService = elasticsearchService;
    }

    private async Task Login() 
    {
        var parameters = new Dictionary<string, string> 
        {
            { "AccountNumber", _configModel.Value.AccountNumber },
            { "UserCode", _configModel.Value.UserCode },
            { "BureauName", _configModel.Value.BureauName },
            { "Password", _configModel.Value.Password },
            { "CallingModule", _configModel.Value.CallingModule }
        };
        var encodedContent = new FormUrlEncodedContent(parameters);

        var result = await _httpClient.PostAsync("token/token", encodedContent);
        var raw = await result.Content.ReadAsStringAsync();
        var lg = JsonSerializer.Deserialize<LoginResponse>(raw);
        await _couchbaseService.PersistToken(lg);
    }

    public async Task<string> QueryInformation(QueryType queryType, PersonInfoRequest request, string clientID, string clientName)
    {
        // check if record exists in couchbase first before making an external call
        var dataInDB = await _couchbaseService.QueryPeopleInformation(queryType, request.IDNumber);
        if (dataInDB != null)
        {
            await InsertTransaction(clientID, clientName, queryType, request, true);
            return dataInDB;
        }

        var token = await GetToken();
        var parameters = ParameterHelper.GetParameters(queryType, token, _configModel.Value.PermissiblePurpose,request);
        var encodedContent = new FormUrlEncodedContent(parameters);

        var url = UrlQuery.GetUrl(queryType);
        var result = await _httpClient.PostAsync(url, encodedContent);
        var raw = await result.Content.ReadAsStringAsync();
        await _couchbaseService.PersistBureauInformation(queryType, request.IDNumber, raw);

        await InsertTransaction(clientID, clientName, queryType, request, result.IsSuccessStatusCode);

        return raw;
    }

    #region Helpers

    private async Task<string> GetToken()
    {
       var token = await _couchbaseService.QueryToken();
        if (string.IsNullOrEmpty(token))
        {
            await Login();
            return await _couchbaseService.QueryToken();
        }
        else
            return token;
    }

    private async Task InsertTransaction(string clientID, string clientName, QueryType queryType, PersonInfoRequest request, bool successStatusCode)
    {
        var transaction = new Transaction
        {
            ClientId = clientID,
            ClientName = clientName,
            MethodName = queryType.ToString(),
            CreatedDate = DateTime.Now,
            Request = JsonSerializer.Serialize(request),
            SuccessfulResponse = successStatusCode,
            Type = "transaction"
        };
        await _elasticsearchService.PersistTransaction(transaction);
        await _couchbaseService.PersistTransaction(transaction);
    }

    #endregion

}