using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Climate;
using Stripe.V2;
using WebApplication1.Interfaces;
using WebApplication1.Models;
using WebApplication1.Views;
using AppContext = WebApplication1.Models.AppContext;

namespace WebApplication1.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IConfiguration _config;
        private readonly ICartService _cartService;
        private readonly IProductService _productService;

        private readonly AppContext _dbContext;

        public PaymentService(IConfiguration config, ICartService cartService, AppContext dbContext,
            IProductService productService
            )
        {
            _config = config;
            _cartService= cartService;
            _dbContext= dbContext;
            _productService = productService;
            StripeConfiguration.ApiKey = _config["StripeSettings:Secretkey"];

        }
        //set delivery type
        public async Task<ShoppingCartView?> AddOrUpdatePaymentIntent(string cartId)
        {
            var cart = await _cartService.GetCart(cartId);
            if (cart == null) return null;

            decimal shippingPrice = 0m;
            if (cart.DeliveryMethodId.HasValue)
            {
                var deliveryMethod = await _dbContext.DeliveryMethods
                    .Where(d => d.Id == cart.DeliveryMethodId)
                    .FirstOrDefaultAsync();
                if (deliveryMethod == null) return null;
                shippingPrice = deliveryMethod.ShippingPrice;
            }

            decimal amount = 0m;
            foreach (var item in cart.CartItems)
            {
                var product = await _productService.GetProductById(item.ProductId);
                if (product == null) continue;
                amount += (decimal)product.Price * item.Quantity;
            }

            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)((amount + shippingPrice) * 100), 
                Currency = "usd", 
                PaymentMethodTypes = new List<string> { "card" },
            };

            PaymentIntent paymentIntent;
            var service = new PaymentIntentService();
            cart.DeliveryMethodId = 1;

            if (string.IsNullOrEmpty(cart.PaymentIntentId))
            {
                paymentIntent = await service.CreateAsync(options);
                cart.PaymentIntentId = paymentIntent.Id;
                cart.ClientSecret = paymentIntent.ClientSecret;
                cart.DeliveryMethodId = 1;
            }
            else
            {
                var updateOptions = new PaymentIntentUpdateOptions
                {
                    Amount = (long)((amount + shippingPrice) * 100), 
                    Currency = "cad", 
                    PaymentMethodTypes = new List<string> { "card" }, 
                };

                paymentIntent = await service.UpdateAsync(cart.PaymentIntentId, updateOptions);
            }

            await _cartService.SetCart(cart);
            return cart;
        }

        public async Task<IEnumerable<DeliveryMethod>> GetDeliveryMethods()
        {
            return await _dbContext.DeliveryMethods.ToListAsync();
        }



    }
}
