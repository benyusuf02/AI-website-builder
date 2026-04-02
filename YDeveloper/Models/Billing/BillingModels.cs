namespace YDeveloper.Models.Billing
{
    public class Invoice
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string InvoiceNumber { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string Currency { get; set; } = "TRY";
        public DateTime IssuedDate { get; set; } = DateTime.UtcNow;
        public DateTime DueDate { get; set; }
        public bool IsPaid { get; set; }
        public string Status { get; set; } = "Pending";
    }

    public class BillingCycle
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalAmount { get; set; }
        public bool IsProcessed { get; set; }
    }
}
