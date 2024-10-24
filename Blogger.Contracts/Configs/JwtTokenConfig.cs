namespace Blogger.Contracts.Configs;

public class JwtTokenConfig
{
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public int ExpiryInMinutes { get; set; }
    public string Key { get; set; }
}