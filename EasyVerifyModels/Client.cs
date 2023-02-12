namespace EasyVerifyModels;

public class Client
{
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    public string ClientName { get; set; }
    public string AllowedScopes { get; set; }
    public bool Active { get; set; }
    public string Type { get; set; }
}
