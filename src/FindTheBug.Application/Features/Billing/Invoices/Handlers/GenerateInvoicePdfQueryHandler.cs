using ErrorOr;
using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Features.Billing.Invoices.Queries;
using FindTheBug.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using PuppeteerSharp;
using PuppeteerSharp.Media;

namespace FindTheBug.Application.Features.Billing.Invoices.Handlers;

public class GenerateInvoicePdfQueryHandler(IUnitOfWork unitOfWork, ITemplateRenderService templateService) : IQueryHandler<GenerateInvoicePdfQuery, byte[]>
{
    public async Task<ErrorOr<byte[]>> Handle(GenerateInvoicePdfQuery request, CancellationToken cancellationToken)
    {
        // Fetch invoice with related data
        var invoice = await unitOfWork.Repository<Invoice>()
            .GetQueryable()
            .Include(i => i.Patient)
            .Include(i => i.InvoiceItems)
            .FirstOrDefaultAsync(i => i.Id == request.InvoiceId, cancellationToken);

        if (invoice == null)
        {
            return Error.NotFound("Invoice.NotFound", "Invoice not found");
        }

        // Generate HTML content
        var htmlContent = await GenerateInvoiceHtml(invoice);

        // Download Chromium browser if not already downloaded
        var browserFetcher = new BrowserFetcher();
        await browserFetcher.DownloadAsync();

        // Generate PDF using PuppeteerSharp
        await using var browser = await Puppeteer.LaunchAsync(new LaunchOptions
        {
            Headless = true,
            Args = new[] { "--no-sandbox", "--disable-setuid-sandbox" }
        });

        await using var page = await browser.NewPageAsync();
        await page.SetContentAsync(htmlContent);

        var pdfBytes = await page.PdfDataAsync(new PdfOptions
        {
            Format = PaperFormat.A4,
            PrintBackground = true
        });

        return pdfBytes;
    }

    private async Task<string> GenerateInvoiceHtml(Invoice invoice)
    {
        // Prepare data
        var data = new
        {
            InvoiceNumber = invoice.InvoiceNumber,
            Status = invoice.Status,
            StatusLower = invoice.Status.ToLower(),
            PatientName = $"{invoice.Patient.FirstName} {invoice.Patient.LastName}",
            PatientAddress = string.IsNullOrEmpty(invoice.Patient.Address) ? "N/A" : invoice.Patient.Address,
            PatientMobile = invoice.Patient.MobileNumber,
            PatientEmail = string.IsNullOrEmpty(invoice.Patient.Email) ? "N/A" : invoice.Patient.Email,
            InvoiceDate = invoice.InvoiceDate.ToString("MMM dd, yyyy"),
            DueDate = invoice.DueDate?.ToString("MMM dd, yyyy"),
            PaymentMethod = invoice.PaymentMethod,
            PaymentDate = invoice.PaymentDate?.ToString("MMM dd, yyyy"),
            Items = invoice.InvoiceItems.Select((item, index) => new
            {
                Index = index + 1,
                Description = item.Description,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice.ToString("F2"),
                DiscountPercentage = item.DiscountPercentage.ToString("F1"),
                Total = (item.Amount - item.DiscountAmount).ToString("F2")
            }).ToList(),
            SubTotal = invoice.SubTotal.ToString("F2"),
            HasDiscount = invoice.DiscountAmount > 0,
            DiscountAmount = invoice.DiscountAmount.ToString("F2"),
            HasTax = invoice.TaxAmount > 0,
            TaxAmount = invoice.TaxAmount.ToString("F2"),
            TotalAmount = invoice.TotalAmount.ToString("F2"),
            Notes = invoice.Notes
        };

        // Render template
        return await templateService.RenderAsync("InvoiceTemplate.html", data);
    }
}
