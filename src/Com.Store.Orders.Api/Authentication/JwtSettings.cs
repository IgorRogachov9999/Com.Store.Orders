namespace Com.Store.Orders.Api.Authentication;

public class JwtSettings
{
    public const string SectionName = "JwtSettings";

    public string SecretKey { get; set; }

    public string Issuer { get; set; }

    public string Audience { get; set; }

    public int ExpirationInMinutes { get; set; }
}