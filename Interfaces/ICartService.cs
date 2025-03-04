using WebApplication1.Models;

namespace WebApplication1.Interfaces
{
    public interface ICartService
    {
        Task<ShoppingCart?> GetCart(string key);
        Task<ShoppingCart?> SetCart(ShoppingCart cart);
        Task<ShoppingCart?> DeleteCart(string key);
    }
}
