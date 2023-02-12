var builder = WebApplication.CreateBuilder(args);
IdentityModelEventSource.ShowPII = true;

builder.Services.AddAuthentication("Bearer").AddJwtBearer("Bearer", options =>
{
    options.Authority = builder.Configuration["BureauHouseConfig:IdentityAuthority"]; // this needs to point to the EasyVerifyAuthenticationApi

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateAudience = false
    };
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<ConfigModel>(builder.Configuration.GetSection("BureauHouseConfig")); // read appsettings.json into the configmodel class
builder.Services.AddSingleton<CouchbaseService>();
builder.Services.AddSingleton<BureauhouseService>();
builder.Services.AddHttpClient<BureauhouseService>( client => 
{
    client.BaseAddress = new Uri(builder.Configuration["BureauHouseConfig:BaseAddress"]); // reads from appsettings.json
});

builder.Services.AddCouchbase(options => 
{
    options.ConnectionString = builder.Configuration["BureauHouseConfig:CouchbaseServer"]; // reads from appsettings.json
    options.UserName = builder.Configuration["BureauHouseConfig:CouchbaseUsername"]; // reads from appsettings.json
    options.Password = builder.Configuration["BureauHouseConfig:CouchbasePassword"]; // reads from appsettings.json

});

var settings = new ElasticsearchClientSettings(new Uri(builder.Configuration["BureauHouseConfig:ElasticsearchURL"])) // reads from appsettings.json
    .DefaultIndex(builder.Configuration["BureauHouseConfig:ElasticsearchDefaultIndex"]); // reads from appsettings.json
var client = new ElasticsearchClient(settings);
builder.Services.AddSingleton<ElasticsearchClient>(client);
builder.Services.AddSingleton<ElasticsearchService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers().RequireAuthorization();

app.MapPost("/api/easyverify",async (BureauhouseService service, ClaimsPrincipal user,[FromBody] PersonInfoRequest request) => 
{
    try
    {
        if (request == null || string.IsNullOrEmpty(request.IDNumber)
           || string.IsNullOrEmpty(request.Query)) { return Results.BadRequest("An error has occured."); }

        string clientId = user.Claims.FirstOrDefault(r => r.Type == "client_id").Value;
        string clientName = user.Claims.FirstOrDefault(r => r.Type == "client_name").Value;

        var queryType = (QueryType)Enum.Parse(typeof(QueryType), request.Query);

        var data = await service.QueryInformation(queryType, request, clientId, clientName);
        return Results.Ok(data);
    }
    catch (Exception ex)
    {
        // you can log here and throw a friendly message with error code :)
        return Results.BadRequest(ex.Message);
    }
}).RequireAuthorization()
.WithOpenApi()
.WithName("easy-verify");

app.Run();