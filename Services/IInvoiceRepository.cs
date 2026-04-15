using InvoiceService.Models;

namespace InvoiceService.Services;

public interface IInvoiceRepository
{
    Task<InvoiceRecord> CreateAsync(string rawJson, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<InvoiceRecord>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<InvoiceRecord?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
}
