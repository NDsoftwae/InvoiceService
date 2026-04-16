using InvoiceService.Configuration;
using InvoiceService.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace InvoiceService.Services;

public sealed class MongoProductRepository : IProductRepository
{
    private readonly IMongoCollection<BsonDocument> _collection;

    public MongoProductRepository(IOptions<MongoDbSettings> settings)
    {
        var mongoSettings = settings.Value;
        var client = new MongoClient(mongoSettings.ConnectionString);
        var database = client.GetDatabase(mongoSettings.DatabaseName);
        _collection = database.GetCollection<BsonDocument>("products");
    }

    public async Task<ProductRecord> CreateAsync(ProductDocument product, CancellationToken cancellationToken = default)
    {
        product.Hash = product.ComputeHash();

        var record = new ProductRecord
        {
            Id = ObjectId.GenerateNewId().ToString(),
            CreatedAtUtc = DateTime.UtcNow,
            Product = product
        };

        await _collection.InsertOneAsync(record.ToBsonDocument(), cancellationToken: cancellationToken);
        return record;
    }

    public async Task<IReadOnlyList<ProductRecord>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var documents = await _collection.Find(FilterDefinition<BsonDocument>.Empty)
            .Sort(Builders<BsonDocument>.Sort.Descending("CreatedAtUtc"))
            .ToListAsync(cancellationToken);

        return documents
            .Select(x => new ProductRecord
            {
                Id = x.GetValue("_id", BsonNull.Value).ToString(),
                CreatedAtUtc = x.GetValue("CreatedAtUtc", BsonNull.Value) == BsonNull.Value
                    ? DateTime.UtcNow
                    : x["CreatedAtUtc"].ToUniversalTime(),
                Product = x.TryGetValue("product", out var product) && product.IsBsonDocument
                    ? BsonSerializer.Deserialize<ProductDocument>(product.AsBsonDocument)
                    : null
            })
            .ToList();
    }
}
