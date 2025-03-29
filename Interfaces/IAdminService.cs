using Stripe;
using WebApplication1.Views;

namespace WebApplication1.Interfaces
{
    public interface IAdminService
    {
        Task<List<OrderView>> GetOrders();
        Task<Refund?> RefundPaymentAsync(int orderId);
    }
}
