using WebApplication1.Models;

namespace WebApplication1.Views
{
    public class BillingDetails
    {
        //public Address ShippingAddress { get; set; }
        public string? Email { get; set; }
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public int? deliveryMethodId { get; set; }
        public string? PaymentIntentId { get; set; }

    }
}
