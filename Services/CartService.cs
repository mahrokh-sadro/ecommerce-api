using StackExchange.Redis;
using WebApplication1.Interfaces;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public class CartService(IConnectionMultiplexer redis) : ICartService
    {
        public Task<ShoppingCart?> DeleteCart(string key)
        {
            throw new NotImplementedException();
        }

        public Task<ShoppingCart?> GetCart(string key)
        {
            throw new NotImplementedException();
        }

        public Task<ShoppingCart?> SetCart(ShoppingCart cart)
        {
            throw new NotImplementedException();
        }
    }
}
