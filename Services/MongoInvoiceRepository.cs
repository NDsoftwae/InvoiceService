using System.Text.Json;
using InvoiceService.Configuration;
using InvoiceService.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace InvoiceService.Services;

public sealed class MongoInvoiceRepository : IInvoiceRepository
{
    private readonly IMongoCollection<BsonDocument> _collection;

    public MongoInvoiceRepository(IOptions<MongoDbSettings> settings)
    {
        var mongoSettings = settings.Value;
        var client = new MongoClient(mongoSettings.ConnectionString);
        var database = client.GetDatabase(mongoSettings.DatabaseName);
        _collection = database.GetCollection<BsonDocument>(mongoSettings.CollectionName);
    }

    public async Task<InvoiceRecord> CreateAsync(InvoiceDocument invoice, CancellationToken cancellationToken = default)
    {
        var record = new InvoiceRecord
        {
            Id = ObjectId.GenerateNewId().ToString(),
            CreatedAtUtc = DateTime.UtcNow,
            Invoice = invoice
        };

        await _collection.InsertOneAsync(record.ToBsonDocument(), cancellationToken: cancellationToken);
        return record;
    }

    public async Task<long> DeleteAllAsync(CancellationToken cancellationToken = default)
    {
        var result = await _collection.DeleteManyAsync(FilterDefinition<BsonDocument>.Empty, cancellationToken);
        return result.DeletedCount;
    }

    public async Task<IReadOnlyList<InvoiceRecord>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var documents = await _collection.Find(FilterDefinition<BsonDocument>.Empty)
            .Sort(Builders<BsonDocument>.Sort.Descending(nameof(InvoiceRecord.CreatedAtUtc)))
            .ToListAsync(cancellationToken);

        return documents
            .Select(MapDocument)
            .Where(record => record is not null)
            .Cast<InvoiceRecord>()
            .ToList();
    }

    public async Task<InvoiceRecord?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        if (!ObjectId.TryParse(id, out var objectId))
        {
            return null;
        }

        var filter = Builders<BsonDocument>.Filter.Eq("_id", objectId);
        var document = await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        return document is null ? null : MapDocument(document);
    }

    private static InvoiceRecord? MapDocument(BsonDocument document)
    {
        var id = document.GetValue("_id", BsonNull.Value);
        var createdAtUtc = document.GetValue(nameof(InvoiceRecord.CreatedAtUtc), BsonNull.Value);

        var invoice = TryGetTypedInvoice(document);
        if (invoice is null)
        {
            return null;
        }

        return new InvoiceRecord
        {
            Id = id == BsonNull.Value ? null : id.ToString(),
            CreatedAtUtc = createdAtUtc == BsonNull.Value ? DateTime.UtcNow : createdAtUtc.ToUniversalTime(),
            Invoice = invoice
        };
    }

    private static InvoiceDocument? TryGetTypedInvoice(BsonDocument document)
    {
        if (document.TryGetValue("invoice", out var invoiceValue) && invoiceValue.IsBsonDocument)
        {
            return BsonSerializer.Deserialize<InvoiceDocument>(invoiceValue.AsBsonDocument);
        }

        if (document.TryGetValue("RawJson", out var rawJsonValue) && rawJsonValue.IsString)
        {
            return JsonSerializer.Deserialize<InvoiceDocument>(rawJsonValue.AsString);
        }

        if (document.Contains("invoice_header") &&
            document.Contains("line_items") &&
            document.Contains("totals") &&
            document.Contains("payment_details"))
        {
            return BsonSerializer.Deserialize<InvoiceDocument>(document);
        }

        return null;
    }
}
