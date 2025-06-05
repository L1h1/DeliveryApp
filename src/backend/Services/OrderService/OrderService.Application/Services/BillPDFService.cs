using Microsoft.Extensions.Configuration;
using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using OrderService.Application.Interfaces.Services;
using OrderService.Domain.Entities;

namespace OrderService.Application.Services
{
    public class BillPDFService : IPDFService
    {
        private readonly IConfiguration _configuration;

        public BillPDFService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> CreateDocumentAsync(Order order, CancellationToken cancellationToken = default)
        {
            var document = new Document();
            var pdfRenderer = new PdfDocumentRenderer();

            BuildDocument(order, document);

            pdfRenderer.Document = document;
            pdfRenderer.RenderDocument();

            var path = Path.Combine(_configuration["BillStorage"], $"{order.Id}.pdf");
            await pdfRenderer.PdfDocument.SaveAsync(path);

            return path;
        }

        private void BuildDocument(Order order, Document document)
        {
            var section = document.AddSection();

            var paragraph = section.AddParagraph();
            paragraph.AddText($"Order: {order.Id}");
            paragraph.AddLineBreak();
            paragraph.AddText($"Creation date (UTC): {order.CreatedAt}");
            paragraph.AddLineBreak();
            paragraph.AddText($"Address: {order.Address}");
            paragraph.Format.SpaceAfter = 40;

            var table = section.AddTable();
            table.Borders.Width = Unit.FromPoint(1);
            table.AddColumn("2cm");
            table.AddColumn("9cm");
            table.AddColumn("2cm");
            table.AddColumn("2cm");

            var row = table.AddRow();
            row.HeadingFormat = true;
            row.Format.Font.Bold = true;
            row.Cells[0].AddParagraph("No");
            row.Cells[1].AddParagraph("Product name");
            row.Cells[2].AddParagraph("Unit price");
            row.Cells[3].AddParagraph("Quantity");

            foreach (var item in order.Items.Select((value, index) => new { index, value }))
            {
                row = table.AddRow();
                row.Cells[0].AddParagraph($"{item.index + 1}");
                row.Cells[1].AddParagraph(item.value.Title);
                row.Cells[2].AddParagraph($"{item.value.Price}");
                row.Cells[3].AddParagraph(item.value.Quantity.ToString());
            }

            row = table.AddRow();
            row.HeadingFormat = true;
            row.Format.Font.Bold = true;
            row.Cells[0].AddParagraph("TOTAL");
            row.Cells[2].AddParagraph(order.TotalPrice.ToString());
        }
    }
}
