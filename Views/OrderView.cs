using WebApplication1.Models;

namespace WebApplication1.Views
{
    public class OrderView
    {
        public int Id { get; set; }
        public string? ShippingEmail { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public decimal? Subtotal { get; set; }
        public decimal? Total { get; set; }

        public string Image { get; set; }

        public OrderView(Order order,string image) { 
            Id = order.Id;
            ShippingEmail = order.ShippingEmail;    
            OrderDate = order.OrderDate;    
            Subtotal = order.Subtotal;
            Total = order.Total;
            Image = image;
        }  
    }
}
