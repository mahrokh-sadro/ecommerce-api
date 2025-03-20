using WebApplication1.Models;

namespace WebApplication1.Views
{
    public class OrderRequest
    {
        public PaymentSummary Payment { get; set; } = null!;
        public BillingDetails BillingDetails { get; set; } = null!;
        public List<CartItem> CartItems { get; set; } = new();
    }
}
