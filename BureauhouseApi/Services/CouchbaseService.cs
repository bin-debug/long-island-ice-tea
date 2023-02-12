using Couchbase.Extensions.DependencyInjection;

namespace BureauhouseApi.Services;
public class CouchbaseService
{
    private readonly ICouchbaseCollection _couchbaseCollection;
    private readonly ICluster _cluster;
    private readonly IBucket _bucket;
    private readonly IBucketProvider _bucketProvider;

    public CouchbaseService(IBucketProvider bucketProvider)
    {
        _bucketProvider = bucketProvider;
        var bucket = bucketProvider.GetBucketAsync("easyverify-general").Result;
        _cluster = bucket.Cluster;
        _couchbaseCollection = bucket.DefaultCollection();
    }

    public async Task PersistToken(LoginResponse loginResponse)
    {
        try
        {
            var result = await _couchbaseCollection.UpsertAsync<LoginResponse>("token", loginResponse, options => 
            {
                options.Expiry(TimeSpan.FromMinutes(15));
            });
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task PersistBureauInformation(QueryType queryType, string IDNumber, string response)
    {
        try
        {
            var bucketName = UrlQuery.GetUrl(queryType).Replace("/", "");
            var bucket = await _bucketProvider.GetBucketAsync(bucketName);
            var collection = bucket.DefaultCollection();

            var result = await collection.UpsertAsync<string>(IDNumber, response);
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task PersistTransaction(Transaction transaction)
    {
        try
        {
            var bucket = await _bucketProvider.GetBucketAsync("easyverify-transactions");
            var collection = bucket.DefaultCollection();
            transaction.Id= Guid.NewGuid();
            var result = await collection.UpsertAsync<Transaction>(transaction.Id.ToString(), transaction);
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<string> QueryPeopleInformation(QueryType queryType, string IDNumber)
    {
        try
        {
            var bucketName = UrlQuery.GetUrl(queryType).Replace("/", "");
            var bucket = await _bucketProvider.GetBucketAsync(bucketName);
            var collection = bucket.DefaultCollection();

            var doc = await collection.GetAsync(IDNumber);
            if (doc != null)
            {
                var content = doc.ContentAs<string>();
                return content;
            }
            else
                return null;
        }
        catch (Exception)
        {
            return null;
            throw;
        }
    }

    public async Task<string> QueryToken()
    {
        try
        {
            var doc = await _couchbaseCollection.GetAsync("token");
            if (doc != null)
            {
                var content = doc.ContentAs<LoginResponse>();
                return content.Results.FirstOrDefault();
            }
            else
                return null;
        }
        catch (Exception)
        {
            return null;
            throw;
        }
    }
}
