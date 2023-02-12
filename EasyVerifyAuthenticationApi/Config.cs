using IdentityServer4.Models;
using IdentityServer4;

namespace EasyVerifyAuthenticationApi;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new List<IdentityResource>
        {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
        };


    public static IEnumerable<ApiScope> ApiScopes =>
        new List<ApiScope>
        {
                new ApiScope("api1", "My API")
        };
}
