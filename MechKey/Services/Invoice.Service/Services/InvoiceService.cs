using InvoiceApi.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace InvoiceApi.Services;

public class InvoiceService
{
    public byte[] GenerateInvoicePdf(InvoiceRequest request)
    {
        QuestPDF.Settings.License = LicenseType.Community;
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(new PageSize(width: 80 * 2.83f, height: PageSizes.A4.Height));
                page.ContinuousSize(226);

                page.Margin(30);
                page.Header().Text($"Invoice #{request.InvoiceNumber}")
                    .FontSize(20).Bold();

                page.Content().Column(col =>
                {
                    col.Item().AlignCenter().Text("Cửa hàng ABC").FontSize(14).Bold();
                    col.Item().Text($"Customer: {request.CustomerName}");
                    col.Item().LineHorizontal(1);

                    // Table items
                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                            columns.ConstantColumn(50);
                            columns.ConstantColumn(80);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Text("Item").Bold();
                            header.Cell().Text("Qty").Bold();
                            header.Cell().Text("Price").Bold();
                        });

                        foreach (var item in request.Items)
                        {
                            table.Cell().Text(item.Name);
                            table.Cell().Text(item.Quantity.ToString());
                            table.Cell().Text($"{item.Price:C}");
                        }
                    });

                    var total = request.Items.Sum(x => x.Price * x.Quantity);
                    col.Item().LineHorizontal(1);
                    col.Item().AlignRight().Text($"Total: {total:C}").Bold();
                });

                page.Footer().AlignCenter().Text(x =>
                {
                    x.Span("Thank you for your purchase!");
                });
            });
        });

        return document.GeneratePdf();
    }
}
