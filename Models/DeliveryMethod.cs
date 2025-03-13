namespace WebApplication1.Models
{
    public class DeliveryMethod
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string DeliveryTime { get; set; }
        public decimal ShippingPrice { get; set; }
    }
}
