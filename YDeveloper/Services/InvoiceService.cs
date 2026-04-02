using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using YDeveloper.Models;

namespace YDeveloper.Services
{
    public interface IInvoiceService
    {
        byte[] GenerateInvoice(PaymentTransaction transaction, ApplicationUser user);
    }

    public class InvoiceService : IInvoiceService
    {
        public byte[] GenerateInvoice(PaymentTransaction transaction, ApplicationUser user)
        {
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Arial")); // Use standard font

                    page.Header()
                        .Text("FATURA")
                        .SemiBold().FontSize(30).FontColor(Colors.Blue.Medium);

                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .Column(x =>
                        {
                            x.Spacing(20);

                            // User Info
                            x.Item().Row(row =>
                            {
                                row.RelativeItem().Column(c =>
                                {
                                    c.Item().Text("Sayın,");
                                    c.Item().Text(user.FullName).Bold();
                                    c.Item().Text(user.Email);
                                    if (!string.IsNullOrEmpty(user.Address)) c.Item().Text(user.Address);
                                    if (!string.IsNullOrEmpty(user.City) && !string.IsNullOrEmpty(user.Country)) c.Item().Text($"{user.City}, {user.Country}");
                                });

                                row.RelativeItem().AlignRight().Column(c =>
                                {
                                    c.Item().Text("yaptik.com");
                                    c.Item().Text("Teknoloji A.Ş.");
                                    c.Item().Text("Maslak, İstanbul");
                                    c.Item().Text("Vergi No: 1234567890");
                                });
                            });

                            x.Item().LineHorizontal(1).LineColor(Colors.Grey.Medium);

                            // Transaction Details
                            x.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(3); // Description
                                    columns.RelativeColumn();  // Date
                                    columns.RelativeColumn();  // Amount
                                });

                                table.Header(header =>
                                {
                                    header.Cell().Element(CellStyle).Text("Açıklama");
                                    header.Cell().Element(CellStyle).Text("Tarih");
                                    header.Cell().Element(CellStyle).AlignRight().Text("Tutar");

                                    static IContainer CellStyle(IContainer container)
                                    {
                                        return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Grey.Medium);
                                    }
                                });

                                table.Cell().Element(CellStyle).Text($"Hizmet Bedeli - İşlem ID: {transaction.TransactionId}");
                                table.Cell().Element(CellStyle).Text(transaction.Timestamp.ToString("dd.MM.yyyy"));
                                table.Cell().Element(CellStyle).AlignRight().Text($"{transaction.Amount:N2} {transaction.Currency}");

                                static IContainer CellStyle(IContainer container)
                                {
                                    return container.PaddingVertical(5);
                                }
                            });

                            x.Item().AlignRight().Text($"Toplam: {transaction.Amount:N2} {transaction.Currency}").FontSize(14).Bold();
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Bu belge elektronik olarak üretilmiştir. ");
                            x.CurrentPageNumber();
                        });
                });
            })
            .GeneratePdf();
        }
    }
}
