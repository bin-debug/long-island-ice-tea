namespace EasyVerifyModels;
public class ConfigModel
{
    public string BaseAddress { get; set; }
    public string AccountNumber { get; set; }
    public string UserCode { get; set; }
    public string BureauName { get; set; }
    public string Password { get; set; }
    public string CallingModule { get; set; }
    public string PermissiblePurpose { get; set; }
    public string CouchbaseServer { get; set; }
    public string CouchbaseUsername { get; set; }
    public string CouchbasePassword { get; set; }
    public string ElasticsearchURL { get; set; }
    public string ElasticsearchDefaultIndex { get; set; }
    public string IdentityAuthority { get; set;}
}
