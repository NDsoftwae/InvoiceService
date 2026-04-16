using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace InvoiceService.Models;

public sealed class InvoiceRecord
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    [Required]
    [BsonElement("invoice")]
    public InvoiceDocument Invoice { get; set; } = new();
}

public sealed class InvoiceResponse
{
    public string Id { get; init; } = string.Empty;

    public DateTime CreatedAtUtc { get; init; }

    public InvoiceDocument Invoice { get; init; } = new();
}

public sealed class InvoiceDocument
{
    [Required]
    [JsonPropertyName("invoice_header")]
    [BsonElement("invoice_header")]
    public InvoiceHeader InvoiceHeader { get; set; } = new();

    [Required]
    [MinLength(1)]
    [JsonPropertyName("line_items")]
    [BsonElement("line_items")]
    public List<InvoiceLineItem> LineItems { get; set; } = [];

    [Required]
    [JsonPropertyName("totals")]
    [BsonElement("totals")]
    public InvoiceTotals Totals { get; set; } = new();

    [Required]
    [JsonPropertyName("payment_details")]
    [BsonElement("payment_details")]
    public PaymentDetails PaymentDetails { get; set; } = new();
}

public sealed class InvoiceHeader
{
    [Required]
    [JsonPropertyName("invoice_number")]
    [BsonElement("invoice_number")]
    public string InvoiceNumber { get; set; } = string.Empty;

    [Required]
    [JsonPropertyName("date")]
    [BsonElement("date")]
    public string Date { get; set; } = string.Empty;

    [Required]
    [JsonPropertyName("seller")]
    [BsonElement("seller")]
    public SellerDetails Seller { get; set; } = new();

    [Required]
    [JsonPropertyName("customer")]
    [BsonElement("customer")]
    public CustomerDetails Customer { get; set; } = new();
}

public sealed class SellerDetails
{
    [Required]
    [JsonPropertyName("name")]
    [BsonElement("name")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [JsonPropertyName("address")]
    [BsonElement("address")]
    public string Address { get; set; } = string.Empty;

    [JsonPropertyName("phone")]
    [BsonElement("phone")]
    public string? Phone { get; set; }

    [JsonPropertyName("email")]
    [BsonElement("email")]
    public string? Email { get; set; }

    [JsonPropertyName("vat_number")]
    [BsonElement("vat_number")]
    public string? VatNumber { get; set; }

    [JsonPropertyName("company_number")]
    [BsonElement("company_number")]
    public string? CompanyNumber { get; set; }
}

public sealed class CustomerDetails
{
    [Required]
    [JsonPropertyName("name")]
    [BsonElement("name")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [JsonPropertyName("address")]
    [BsonElement("address")]
    public string Address { get; set; } = string.Empty;

    [JsonPropertyName("customer_id")]
    [BsonElement("customer_id")]
    public string? CustomerId { get; set; }

    [JsonPropertyName("contact_name")]
    [BsonElement("contact_name")]
    public string? ContactName { get; set; }

    [JsonPropertyName("phone")]
    [BsonElement("phone")]
    public string? Phone { get; set; }
}

public sealed class InvoiceLineItem
{
    [Required]
    [JsonPropertyName("code")]
    [BsonElement("code")]
    public string Code { get; set; } = string.Empty;

    [Required]
    [JsonPropertyName("description")]
    [BsonElement("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("quantity")]
    [BsonElement("quantity")]
    public decimal Quantity { get; set; }

    [JsonPropertyName("unit")]
    [BsonElement("unit")]
    public string? Unit { get; set; }

    [JsonPropertyName("unit_price")]
    [BsonElement("unit_price")]
    [BsonRepresentation(BsonType.Decimal128)]
    public decimal UnitPrice { get; set; }

    [JsonPropertyName("vat_rate")]
    [BsonElement("vat_rate")]
    [BsonRepresentation(BsonType.Decimal128)]
    public decimal VatRate { get; set; }

    [JsonPropertyName("total_inc_vat")]
    [BsonElement("total_inc_vat")]
    [BsonRepresentation(BsonType.Decimal128)]
    public decimal TotalIncVat { get; set; }

    [JsonPropertyName("pack_size")]
    [BsonElement("pack_size")]
    public string? PackSize { get; set; }

    [JsonPropertyName("unit_grams")]
    [BsonElement("unit_grams")]
    [BsonRepresentation(BsonType.Decimal128)]
    public decimal? UnitGrams { get; set; }
}

public sealed class InvoiceTotals
{
    [JsonPropertyName("subtotal")]
    [BsonElement("subtotal")]
    [BsonRepresentation(BsonType.Decimal128)]
    public decimal Subtotal { get; set; }

    [JsonPropertyName("vat_total")]
    [BsonElement("vat_total")]
    [BsonRepresentation(BsonType.Decimal128)]
    public decimal VatTotal { get; set; }

    [JsonPropertyName("discount")]
    [BsonElement("discount")]
    [BsonRepresentation(BsonType.Decimal128)]
    public decimal Discount { get; set; }

    [JsonPropertyName("grand_total")]
    [BsonElement("grand_total")]
    [BsonRepresentation(BsonType.Decimal128)]
    public decimal GrandTotal { get; set; }
}

public sealed class PaymentDetails
{
    [JsonPropertyName("bank_name")]
    [BsonElement("bank_name")]
    public string? BankName { get; set; }

    [JsonPropertyName("account_number")]
    [BsonElement("account_number")]
    public string? AccountNumber { get; set; }

    [JsonPropertyName("sort_code")]
    [BsonElement("sort_code")]
    public string? SortCode { get; set; }

    [JsonExtensionData]
    [BsonExtraElements]
    public Dictionary<string, object>? ExtraFields { get; set; }
}
