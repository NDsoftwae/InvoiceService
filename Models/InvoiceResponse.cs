using System.Text.Json.Nodes;

namespace InvoiceService.Models;

public sealed class InvoiceResponse
{
    public string Id { get; init; } = string.Empty;

    public DateTime CreatedAtUtc { get; init; }

    public JsonNode? Invoice { get; init; }
}
