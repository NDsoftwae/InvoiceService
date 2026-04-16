using InvoiceService.Filters;
using InvoiceService.Models;
using InvoiceService.Services;
using Microsoft.AspNetCore.Mvc;

namespace InvoiceService.Controllers;

[ApiController]
[Route("api/products")]
[ServiceFilter(typeof(RequireApiKeyOrJwtAttribute))]
public sealed class ProductsController : ControllerBase
{
    private readonly IProductRepository _repository;

    public ProductsController(IProductRepository repository)
    {
        _repository = repository;
    }

    [HttpPost]
    public async Task<ActionResult<ProductResponse>> Create([FromBody] ProductDocument product, CancellationToken cancellationToken)
    {
        var created = await _repository.CreateAsync(product, cancellationToken);
        var response = new ProductResponse
        {
            Id = created.Id ?? string.Empty,
            CreatedAtUtc = created.CreatedAtUtc,
            Product = created.Product
        };

        return Ok(response);
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ProductResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var products = await _repository.GetAllAsync(cancellationToken);
        return Ok(products.Select(x => new ProductResponse
        {
            Id = x.Id ?? string.Empty,
            CreatedAtUtc = x.CreatedAtUtc,
            Product = x.Product
        }).ToList());
    }
}
