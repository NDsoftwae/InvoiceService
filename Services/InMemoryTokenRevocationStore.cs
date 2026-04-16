using System.Collections.Concurrent;

namespace InvoiceService.Services;

public sealed class InMemoryTokenRevocationStore : ITokenRevocationStore
{
    private readonly ConcurrentDictionary<string, DateTime> _revokedTokens = new();

    public void Revoke(string jti, DateTime expiresAtUtc)
    {
        CleanupExpired();
        _revokedTokens[jti] = expiresAtUtc;
    }

    public bool IsRevoked(string jti)
    {
        CleanupExpired();
        return _revokedTokens.ContainsKey(jti);
    }

    private void CleanupExpired()
    {
        var now = DateTime.UtcNow;
        foreach (var entry in _revokedTokens)
        {
            if (entry.Value <= now)
            {
                _revokedTokens.TryRemove(entry.Key, out _);
            }
        }
    }
}
