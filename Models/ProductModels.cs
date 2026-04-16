using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace InvoiceService.Models;

public sealed class ProductRecord
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    [BsonElement("product")]
    public ProductDocument? Product { get; set; }
}

public sealed class ProductDocument
{
    [JsonPropertyName("sku")]
    [BsonElement("sku")]
    public string? Sku { get; set; }

    [JsonPropertyName("description")]
    [BsonElement("description")]
    public string? Description { get; set; }

    [JsonPropertyName("size")]
    [BsonElement("size")]
    public string? Size { get; set; }

    [JsonPropertyName("qty")]
    [BsonElement("qty")]
    [BsonRepresentation(BsonType.Decimal128)]
    public decimal? Qty { get; set; }

    [JsonPropertyName("cost")]
    [BsonElement("cost")]
    [BsonRepresentation(BsonType.Decimal128)]
    public decimal? Cost { get; set; }

    [JsonPropertyName("unitcost")]
    [BsonElement("unitcost")]
    [BsonRepresentation(BsonType.Decimal128)]
    public decimal? UnitCost { get; set; }

    [JsonPropertyName("price")]
    [BsonElement("price")]
    [BsonRepresentation(BsonType.Decimal128)]
    public decimal? Price { get; set; }

    [JsonPropertyName("unitprice")]
    [BsonElement("unitprice")]
    [BsonRepresentation(BsonType.Decimal128)]
    public decimal? UnitPrice { get; set; }

    [JsonPropertyName("vat")]
    [BsonElement("vat")]
    [BsonRepresentation(BsonType.Decimal128)]
    public decimal? Vat { get; set; }

    [JsonPropertyName("hash")]
    [BsonElement("hash")]
    public string? Hash { get; set; }

    public string ComputeHash()
    {
        var payload = string.Join("|",
            Sku ?? string.Empty,
            Description ?? string.Empty,
            Size ?? string.Empty,
            Qty?.ToString(System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty,
            Cost?.ToString(System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty,
            UnitCost?.ToString(System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty,
            Price?.ToString(System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty,
            UnitPrice?.ToString(System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty,
            Vat?.ToString(System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty);

        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(payload));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }
}

public sealed class ProductResponse
{
    public string Id { get; init; } = string.Empty;

    public DateTime CreatedAtUtc { get; init; }

    public ProductDocument? Product { get; init; }
}
