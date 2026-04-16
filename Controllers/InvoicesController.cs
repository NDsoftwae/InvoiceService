using InvoiceService.Filters;
using InvoiceService.Models;
using InvoiceService.Services;
using Microsoft.AspNetCore.Mvc;

namespace InvoiceService.Controllers;

[ApiController]
[Route("api/invoices")]
[ServiceFilter(typeof(RequireApiKeyAttribute))]
public sealed class InvoicesController : ControllerBase
{
    private readonly IInvoiceRepository _repository;

    public InvoicesController(IInvoiceRepository repository)
    {
        _repository = repository;
    }

    [HttpPost]
    public async Task<ActionResult<InvoiceResponse>> Create([FromBody] InvoiceDocument invoice, CancellationToken cancellationToken)
    {
        var created = await _repository.CreateAsync(invoice, cancellationToken);
        var response = ToResponse(created);

        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<InvoiceResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var invoices = await _repository.GetAllAsync(cancellationToken);
        return Ok(invoices.Select(ToResponse).ToList());
    }

    [HttpDelete]
    public async Task<ActionResult<object>> DeleteAll(CancellationToken cancellationToken)
    {
        var deletedCount = await _repository.DeleteAllAsync(cancellationToken);
        return Ok(new { deleted_count = deletedCount });
    }

    [HttpDelete("all")]
    public Task<ActionResult<object>> DeleteAllAtExplicitRoute(CancellationToken cancellationToken)
    {
        return DeleteAll(cancellationToken);
    }

    [HttpPost("delete-all")]
    public Task<ActionResult<object>> DeleteAllViaPost(CancellationToken cancellationToken)
    {
        return DeleteAll(cancellationToken);
    }

    [HttpGet("{id:length(24)}")]
    public async Task<ActionResult<InvoiceResponse>> GetById(string id, CancellationToken cancellationToken)
    {
        var invoice = await _repository.GetByIdAsync(id, cancellationToken);
        if (invoice is null)
        {
            return NotFound();
        }

        return Ok(ToResponse(invoice));
    }

    private static InvoiceResponse ToResponse(InvoiceRecord record)
    {
        return new InvoiceResponse
        {
            Id = record.Id ?? string.Empty,
            CreatedAtUtc = record.CreatedAtUtc,
            Invoice = record.Invoice
        };
    }
}
