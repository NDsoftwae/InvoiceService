using InvoiceService.Configuration;
using Microsoft.Extensions.Options;

namespace InvoiceService.Services;

public sealed class ConfigUserAuthService : IUserAuthService
{
    private readonly IReadOnlyList<AuthUserSettings> _users;

    public ConfigUserAuthService(IOptions<List<AuthUserSettings>> users)
    {
        _users = users.Value;
    }

    public bool ValidateCredentials(string? username, string? password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            return false;
        }

        return _users.Any(x =>
            string.Equals(x.Username, username, StringComparison.Ordinal) &&
            string.Equals(x.Password, password, StringComparison.Ordinal));
    }
}
