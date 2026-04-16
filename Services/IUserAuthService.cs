namespace InvoiceService.Services;

public interface IUserAuthService
{
    bool ValidateCredentials(string? username, string? password);
}
