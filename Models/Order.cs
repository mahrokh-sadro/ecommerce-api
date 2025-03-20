namespace WebApplication1.Models
{/// <summary>
/// email is set to email in user table
/// 
/// </summary>
    public class Order
    {
        public int Id { get; set; }
        public string Email { get; set; } 
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public int ShippingAddressId { get; set; } 
        public int DeliveryMethodId { get; set; } 
        public int PaymentSummaryId { get; set; } 
        public List<CartItem> CartItems { get; set; } = new List<CartItem>();
        public decimal Subtotal { get; set; }
        public decimal Discount { get; set; }
        //public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public string PaymentIntentId { get; set; } 
        //public Order() { }

        public Order(string email,string paymentIntentId)
        {
            Email = email;  
            PaymentIntentId = paymentIntentId;
        }
    }
}
