using WebApplication1.Models;

namespace WebApplication1.Views
{
    public class ShoppingCartView
    {
        public string Id { get; set; }

        public List<CartItemView> CartItems { get; set; }
    }
}
