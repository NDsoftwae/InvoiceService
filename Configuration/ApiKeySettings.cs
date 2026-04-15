namespace InvoiceService.Configuration;

public sealed class ApiKeySettings
{
    public const string SectionName = "ApiKey";

    public string HeaderName { get; set; } = "api-key";

    public string Value { get; set; } = string.Empty;
}
