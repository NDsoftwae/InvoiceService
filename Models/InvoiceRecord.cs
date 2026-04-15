using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace InvoiceService.Models;

public sealed class InvoiceRecord
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public string RawJson { get; set; } = string.Empty;

    public string? InvoiceNumber { get; set; }
}
