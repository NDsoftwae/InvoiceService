using System.Text.Json;
using InvoiceService.Configuration;
using InvoiceService.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace InvoiceService.Services;

public sealed class MongoInvoiceRepository : IInvoiceRepository
{
    private readonly IMongoCollection<InvoiceRecord> _collection;

    public MongoInvoiceRepository(IOptions<MongoDbSettings> settings)
    {
        var mongoSettings = settings.Value;
        var client = new MongoClient(mongoSettings.ConnectionString);
        var database = client.GetDatabase(mongoSettings.DatabaseName);
        _collection = database.GetCollection<InvoiceRecord>(mongoSettings.CollectionName);
    }

    public async Task<InvoiceRecord> CreateAsync(string rawJson, CancellationToken cancellationToken = default)
    {
        var record = new InvoiceRecord
        {
            CreatedAtUtc = DateTime.UtcNow,
            RawJson = rawJson,
            InvoiceNumber = TryReadInvoiceNumber(rawJson)
        };

        await _collection.InsertOneAsync(record, cancellationToken: cancellationToken);
        return record;
    }

    public async Task<IReadOnlyList<InvoiceRecord>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _collection.Find(Builders<InvoiceRecord>.Filter.Empty)
            .SortByDescending(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<InvoiceRecord?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await _collection.Find(x => x.Id == id).FirstOrDefaultAsync(cancellationToken);
    }

    private static string? TryReadInvoiceNumber(string rawJson)
    {
        using var document = JsonDocument.Parse(rawJson);

        if (document.RootElement.TryGetProperty("invoice_header", out var header) &&
            header.TryGetProperty("invoice_number", out var invoiceNumber))
        {
            return invoiceNumber.GetString();
        }

        return null;
    }
}
