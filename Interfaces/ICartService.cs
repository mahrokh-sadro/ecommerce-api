using WebApplication1.Views;

namespace WebApplication1.Interfaces
{
    public interface ICartService
    {
        Task<ShoppingCart?> GetCart(string key);
        Task<bool> SetCart(ShoppingCart cart);
        Task<bool> DeleteCart(string key);
    }
}
