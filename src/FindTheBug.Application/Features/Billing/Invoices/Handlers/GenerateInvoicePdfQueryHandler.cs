using ErrorOr;
using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Common.Models;
using FindTheBug.Application.Features.Billing.Invoices.Queries;

namespace FindTheBug.Application.Features.Billing.Invoices.Handlers;

public class GenerateInvoicePdfQueryHandler(IUnitOfWork unitOfWork, ITemplateRenderService templateService) : IQueryHandler<GenerateInvoicePdfQuery, byte[]>
{
    public async Task<ErrorOr<Result<byte[]>>> Handle(GenerateInvoicePdfQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
