namespace InvoiceService.Configuration;

public sealed class JwtSettings
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = "InvoiceService";

    public string Audience { get; set; } = "InvoiceServiceClient";

    public string SecretKey { get; set; } = string.Empty;

    public int ExpiresMinutes { get; set; } = 480;
}
