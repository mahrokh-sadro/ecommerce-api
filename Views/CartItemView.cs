namespace WebApplication1.Views
{
    public class CartItemView
    {
        public string ShoppingCartId { get; set; } = null!;

        public int ProductId { get; set; }

        public int Quantity { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public decimal? Price { get; set; }

        public string? Image { get; set; }
    }
}
