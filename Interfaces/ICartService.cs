using WebApplication1.Views;

namespace WebApplication1.Interfaces
{
    public interface ICartService
    {
        Task<ShoppingCartView?> GetCart(string key);
        Task<ShoppingCartView?> SetCart(ShoppingCartView cart);
        Task<bool> DeleteCart(string key);
    }
}
