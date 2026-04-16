using InvoiceService.Models;

namespace InvoiceService.Services;

public interface IUserRepository
{
    Task<UserRecord?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);

    Task<UserRecord> CreateAsync(string username, string password, CancellationToken cancellationToken = default);
}
