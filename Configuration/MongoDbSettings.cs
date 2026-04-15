namespace InvoiceService.Configuration;

public sealed class MongoDbSettings
{
    public const string SectionName = "MongoDb";

    public string ConnectionString { get; set; } = "mongodb://localhost:27017";

    public string DatabaseName { get; set; } = "invoice_service";

    public string CollectionName { get; set; } = "invoices";
}
