using Couchbase.Extensions.DependencyInjection;
using Couchbase.KeyValue;
using EasyVerifyAuthenticationApi;
using EasyVerifyModels;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCouchbase(options =>
{
    options.ConnectionString = builder.Configuration["Couchbase:Server"];
    options.UserName = builder.Configuration["Couchbase:Username"];
    options.Password = builder.Configuration["Couchbase:Password"];

});

builder.Services.AddIdentityServer()
    .AddDeveloperSigningCredential()
    .AddInMemoryIdentityResources(Config.IdentityResources)
    .AddInMemoryApiScopes(Config.ApiScopes)
    .AddClientStore<ClientStore>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseIdentityServer();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapGet("/ping", () => { return Results.Ok($"pong {DateTime.Now} :)"); }).WithName("EasyVerify").WithOpenApi();
app.Run();

public class ClientStore : IClientStore
{
    private readonly IBucketProvider _bucketProvider;
    private readonly ICouchbaseCollection _collection;


    public ClientStore(IBucketProvider bucketProvider)
    {
        _bucketProvider = bucketProvider;
        var bucket = bucketProvider.GetBucketAsync("easyverify-general").Result;
        _collection = bucket.DefaultCollection();

    }

    public async Task<IdentityServer4.Models.Client> FindClientByIdAsync(string clientId)
    {
        var doc = await _collection.GetAsync(clientId);
        if (doc == null)
            return null;

        var easyClient = doc.ContentAs<EasyVerifyModels.Client>();
        return new IdentityServer4.Models.Client()
        {
            ClientId = easyClient.ClientId,
            ClientSecrets = { new Secret($"{easyClient.ClientSecret}".Sha256()) },
            ClientName = easyClient.ClientName,
            AllowedScopes = new List<string>() { easyClient.AllowedScopes },
            AllowedGrantTypes = GrantTypes.ClientCredentials,
            AccessTokenLifetime = 4380,
            Claims = new List<IdentityServer4.Models.ClientClaim>
            {
                new ClientClaim("name", easyClient.ClientName)
            }
        };
    }
}
