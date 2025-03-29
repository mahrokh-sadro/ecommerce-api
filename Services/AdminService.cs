using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;
using WebApplication1.Interfaces;
using WebApplication1.Models;
using WebApplication1.Views;
using AppContext = WebApplication1.Models.AppContext;

namespace WebApplication1.Services
{
    public class AdminService : IAdminService
    {
        private readonly IConfiguration _config;


        private readonly AppContext _dbContext;

        public AdminService(IConfiguration config, ICartService cartService, AppContext dbContext,
            IProductService productService
            )
        {
            _config = config;
            _dbContext= dbContext;
            StripeConfiguration.ApiKey = _config["StripeSettings:Secretkey"];

        }

        public async Task<List<OrderView>> GetOrders()
        {
            var orders = await _dbContext.Order.ToListAsync();
            List<OrderView> orderList = new List<OrderView>();
            foreach (var order in orders)
            {
                CartItem item = await _dbContext.CartItems.FirstOrDefaultAsync(i => i.OrderId == order.Id);
                if (item == null) continue;
                var obj = new OrderView(order, item.Image);
                orderList.Add(obj);
            }

            return orderList;
        }

        public async Task<Refund?> RefundPaymentAsync(int orderId)
        {
            try
            {
                var order = await _dbContext.Order.FirstOrDefaultAsync(o => o.Id == orderId);
                if (order == null)
                {
                    throw new Exception("Order not found.");
                }

                if (order.Status != "Succeeded")
                {
                    throw new Exception("Order is not eligible for a refund.");
                }

                if (string.IsNullOrEmpty(order.PaymentIntentId))
                {
                    throw new Exception("Invalid PaymentIntentId. Cannot process refund.");
                }

                var service = new RefundService();
                var refundOptions = new RefundCreateOptions
                {
                    PaymentIntent = order.PaymentIntentId
                };

                Refund refund = await service.CreateAsync(refundOptions);

                // Update the order status in the database (optional)
                order.Status = "Refunded";
                await _dbContext.SaveChangesAsync();

                return refund;
            }
            catch (StripeException ex)
            {
                throw new Exception($"Stripe Refund Error: {ex.Message}");
            }
        }

    }
}
