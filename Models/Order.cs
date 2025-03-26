namespace WebApplication1.Models
{/// <summary>
/// email is set to email in user table
/// 
/// </summary>
    public class Order
    {
        public int Id { get; set; }
        public string? ShippingEmail { get; set; }
        public string? UserId { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public int ShippingAddressId { get; set; }
        public int DeliveryMethodId { get; set; }
        public int PaymentSummaryId { get; set; }
        //public List<CartItem> CartItems { get; set; } = new List<CartItem>();
        public decimal? Subtotal { get; set; }
        public decimal? Discount { get; set; }
        public decimal? TaxAmount { get; set; }
        public decimal? Total { get; set; }

        public string Status { get; set; } = "Pending";

        public string? PaymentIntentId { get; set; } 

        //public Order(string email,string paymentIntentId)
        //{
        //    ShippingEmail = email;  
        //    PaymentIntentId = paymentIntentId;
        //}
    }
}
