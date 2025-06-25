using System.Globalization;
using System.Text;
using Microsoft.Extensions.Logging;
using OrderService.Application.Interfaces.Services;
using OrderService.Domain.Entities;

namespace OrderService.Application.Services
{
    public class BillService : IBillService
    {
        private const int ItemColumnWidth = 20;
        private const int QuantityColumnWidth = 5;
        private const int PriceColumnWidth = 10;
        private const int SumColumnWidth = 10;
        private const int TotalWidth = ItemColumnWidth + QuantityColumnWidth + PriceColumnWidth + SumColumnWidth + 3;

        private readonly ILogger<BillService> _logger;

        public BillService(ILogger<BillService> logger)
        {
            _logger = logger;
        }

        public async Task<string> CreateDocumentAsync(Order order, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Creating bill document for order @{id}", order.Id);

            var sb = new StringBuilder();
            var culture = CultureInfo.InvariantCulture;

            sb.AppendLine(CenterText("Delivery App", TotalWidth));
            sb.AppendLine(new string('-', TotalWidth));
            sb.AppendLine(
                $"{Pad("Item", ItemColumnWidth)} " +
                $"{Pad("Qty", QuantityColumnWidth, alignRight: true)} " +
                $"{Pad("Price", PriceColumnWidth, alignRight: true)} " +
                $"{Pad("Sum", SumColumnWidth, alignRight: true)}");
            sb.AppendLine(new string('-', TotalWidth));

            foreach (var item in order.Items)
            {
                var name = Truncate(item.Title, ItemColumnWidth);
                var qty = item.Quantity.ToString();
                var price = item.Price.ToString("F2", culture);
                var sum = (item.Price * item.Quantity).ToString("F2", culture);

                sb.AppendLine(
                    $"{Pad(name, ItemColumnWidth)} " +
                    $"{Pad(qty, QuantityColumnWidth, alignRight: true)} " +
                    $"{Pad(price, PriceColumnWidth, alignRight: true)} " +
                    $"{Pad(sum, SumColumnWidth, alignRight: true)}");
            }

            sb.AppendLine(new string('-', TotalWidth));
            var total = order.TotalPrice.ToString("F2", culture).PadLeft(SumColumnWidth);
            sb.AppendLine(Pad("TOTAL:", TotalWidth - SumColumnWidth) + total);
            sb.AppendLine("Thank you for your order!");

            _logger.LogInformation("Successfully created bill document for order @{id}", order.Id);

            return sb.ToString();
        }

        private static string Pad(string text, int width, bool alignRight = false)
        {
            if (text.Length > width)
            {
                text = Truncate(text, width);
            }

            return alignRight ? text.PadLeft(width) : text.PadRight(width);
        }

        private static string Truncate(string text, int maxLength)
        {
            return string.IsNullOrEmpty(text) || text.Length <= maxLength
                ? text
                : text.Substring(0, maxLength - 1) + "…";
        }

        private static string CenterText(string text, int width)
        {
            if (text.Length >= width)
            {
                return text;
            }

            var padding = (width - text.Length) / 2;
            return new string(' ', padding) + text;
        }
    }
}