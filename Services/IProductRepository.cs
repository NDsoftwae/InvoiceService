using InvoiceService.Models;

namespace InvoiceService.Services;

public interface IProductRepository
{
    Task<ProductRecord> CreateAsync(ProductDocument product, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ProductRecord>> GetAllAsync(CancellationToken cancellationToken = default);
}
