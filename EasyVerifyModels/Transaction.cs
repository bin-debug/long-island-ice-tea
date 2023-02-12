namespace EasyVerifyModels;

public class Transaction
{
    public Guid Id { get; set; }
    public string ClientId { get; set; }
    public string ClientName { get; set; }
    public string MethodName { get; set; }
    public string Request { get; set; }
    public bool SuccessfulResponse { get; set; }
    public DateTime CreatedDate { get; set; }
    public string Type { get; set; }
}
