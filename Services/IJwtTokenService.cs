using InvoiceService.Models;

namespace InvoiceService.Services;

public interface IJwtTokenService
{
    LoginResponse CreateToken(string username);
}
