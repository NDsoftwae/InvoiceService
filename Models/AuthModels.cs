using System.Text.Json.Serialization;

namespace InvoiceService.Models;

public sealed class LoginRequest
{
    [JsonPropertyName("username")]
    public string? Username { get; set; }

    [JsonPropertyName("password")]
    public string? Password { get; set; }
}

public sealed class LoginResponse
{
    [JsonPropertyName("token")]
    public string Token { get; init; } = string.Empty;

    [JsonPropertyName("token_type")]
    public string TokenType { get; init; } = "Bearer";

    [JsonPropertyName("expires_at_utc")]
    public DateTime ExpiresAtUtc { get; init; }

    [JsonPropertyName("username")]
    public string Username { get; init; } = string.Empty;
}
