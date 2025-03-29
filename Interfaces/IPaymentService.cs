using Stripe;
using WebApplication1.Models;
using WebApplication1.Views;

namespace WebApplication1.Interfaces
{
    public interface IPaymentService
    {
        Task<ShoppingCartView?> AddOrUpdatePaymentIntent(string cartId);
        Task<IEnumerable<DeliveryMethod>> GetDeliveryMethods();
    }
}
