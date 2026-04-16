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

    public async Task<InvoiceRecord> CreateAsync(InvoiceDocument invoice, CancellationToken cancellationToken = default)
    {
        var record = new InvoiceRecord
        {
            CreatedAtUtc = DateTime.UtcNow,
            Invoice = invoice
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
}
