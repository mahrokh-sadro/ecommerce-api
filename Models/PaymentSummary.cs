namespace WebApplication1.Models
{
    public class PaymentSummary
    {
        public int Id { get; set; }
        public int Last4 { get; set; }
        public required string Brand { get; set; }
        public int ExpMonth { get; set; }
        public int ExpYear { get; set; }
    }
}
