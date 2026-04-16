using InvoiceService.Configuration;
using InvoiceService.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace InvoiceService.Services;

public sealed class MongoUserRepository : IUserRepository
{
    private readonly IMongoCollection<UserRecord> _collection;
    private readonly IPasswordHasher _passwordHasher;

    public MongoUserRepository(IOptions<MongoDbSettings> settings, IPasswordHasher passwordHasher)
    {
        var mongoSettings = settings.Value;
        var client = new MongoClient(mongoSettings.ConnectionString);
        var database = client.GetDatabase(mongoSettings.DatabaseName);
        _collection = database.GetCollection<UserRecord>("users");
        _passwordHasher = passwordHasher;
    }

    public async Task<UserRecord?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        return await _collection.Find(x => x.Username == username).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<UserRecord> CreateAsync(string username, string password, CancellationToken cancellationToken = default)
    {
        var existing = await GetByUsernameAsync(username, cancellationToken);
        if (existing is not null)
        {
            throw new InvalidOperationException("A user with that username already exists.");
        }

        var (hash, salt) = _passwordHasher.HashPassword(password);
        var user = new UserRecord
        {
            Username = username,
            PasswordHash = hash,
            PasswordSalt = salt,
            CreatedAtUtc = DateTime.UtcNow
        };

        await _collection.InsertOneAsync(user, cancellationToken: cancellationToken);
        return user;
    }
}
