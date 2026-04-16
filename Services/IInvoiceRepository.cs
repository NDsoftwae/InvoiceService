using InvoiceService.Models;

namespace InvoiceService.Services;

public interface IInvoiceRepository
{
    Task<InvoiceRecord> CreateAsync(InvoiceDocument invoice, CancellationToken cancellationToken = default);

    Task<long> DeleteAllAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<InvoiceRecord>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<InvoiceRecord?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
}
