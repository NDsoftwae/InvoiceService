namespace InvoiceService.Services;

public interface IUserAuthService
{
    Task<bool> ValidateCredentialsAsync(string? username, string? password, CancellationToken cancellationToken = default);
}
