using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;
using System.Security.Claims;
using WebApplication1.Interfaces;
using WebApplication1.Models;
using WebApplication1.Services;
using WebApplication1.Views;
using AppContext = WebApplication1.Models.AppContext;
using Order = WebApplication1.Models.Order;
using Newtonsoft.Json;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly ICartService _cartService;
        private readonly AppContext _dbContext;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IProductService _productService;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(IPaymentService paymentService, ICartService cartService, AppContext dbContext, SignInManager<AppUser> signInManager, IProductService productService,
            ILogger<PaymentController> logger)
        {
            _paymentService = paymentService;
            _cartService = cartService;
            _dbContext = dbContext;
            _signInManager = signInManager;
            _productService = productService;
            _logger = logger;
        }

        // Add or update the payment intent
        [HttpPost("{cartId}")]
        public async Task<ActionResult<ShoppingCartView>> AddOrUpdatePaymentIntent(string cartId)
        {
            try
            {
                var cart = await _paymentService.AddOrUpdatePaymentIntent(cartId);
                if (cart == null)
                {
                    return BadRequest("Unable to create or update payment intent.");
                }
                return Ok(cart);
            }
            catch (Exception e)
            {
                return StatusCode(500, "PaymentController->AddOrUpdatePaymentIntent"+e.Message);
            }
        }

        [HttpGet("delivery-methods")]
        public async Task<ActionResult<IEnumerable<DeliveryMethod>>> GetDeliveryMethods()
        {
            try
            {
                var deliveryMethods = await _paymentService.GetDeliveryMethods();
                return Ok(deliveryMethods);
            }
            catch (Exception e)
            {
                return StatusCode(500, "PaymentController->GetDeliveryMethods: " + e.Message);
            }
        }

        [HttpPost("order")]
        public async Task<ActionResult<Object>> CreateOrder([FromBody] OrderRequest request)
        {
            UserInfo userInfo = await GetUserInfo();
            if (userInfo == null)
                return BadRequest("User address not found.");

            var payment = request.Payment;
            var billingDetails = request.BillingDetails;
            var cartItems = request.CartItems;
            var ShippingAddress = request.ShippingAddress;

            await _dbContext.PaymentSummary.AddAsync(payment);
            await _dbContext.Address.AddAsync(ShippingAddress);
            await _dbContext.SaveChangesAsync();

            var deliveryMethod = await _dbContext.DeliveryMethods
                .FirstOrDefaultAsync(d => d.Id == billingDetails.deliveryMethodId);

            if (deliveryMethod == null)
                return BadRequest("Invalid delivery method.");

            decimal subtotal = 0m;
            decimal taxRate = 0.13m;
            decimal discount = 0m;

            foreach (var item in cartItems)
            {
                var product = await _productService.GetProductById(item.ProductId);
                if (product == null) continue;
                subtotal += (decimal)product.Price * item.Quantity;
            }

            // create order
            var order = new Order()
            {
                ShippingEmail= string.IsNullOrEmpty(billingDetails?.Email) ? userInfo.Email:billingDetails.Email,
                PaymentIntentId = billingDetails.PaymentIntentId,
                ShippingAddressId = ShippingAddress.Id,
                UserId = userInfo.Id,
                DeliveryMethodId = deliveryMethod.Id,
                PaymentSummaryId = payment.Id,
                Subtotal = subtotal,
                TaxAmount = taxRate * subtotal,
                Total = subtotal * (1 + taxRate) - discount,
                Discount = discount
            };

            await _dbContext.Order.AddAsync(order);
            await _dbContext.SaveChangesAsync();

            var cartItemsToAdd = cartItems.Select(item => new CartItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                Image = item.Image,
                OrderId = order.Id,
            }).ToList();

            await _dbContext.CartItems.AddRangeAsync(cartItemsToAdd);
            await _dbContext.SaveChangesAsync(); 

            return Ok(new
            {
                id=order.Id,
                total=order.Total
            });
        }

        [HttpGet("orders")]
        public async Task<IActionResult> GetOrders()
        {
            if (User.Identity?.IsAuthenticated == false)
                return null;

            // Retrieve the email of the authenticated user from claims
            var email = User.FindFirstValue(ClaimTypes.Email);

            if (string.IsNullOrEmpty(email))
                return null;

            var user = await _signInManager.UserManager.Users
                .Include(u => u.Address)
                .FirstOrDefaultAsync(u => u.Email == email);


            if (user == null)
                return null;

            var orders=await _dbContext.Order.Where(o=>o.UserId == user.Id).ToListAsync();
            List<OrderView> orderList = new List<OrderView>();
            foreach (var order in orders)
            {
                CartItem item = await _dbContext.CartItems.FirstOrDefaultAsync(i => i.OrderId == order.Id);
                var obj = new OrderView(order,item.Image);
                orderList.Add(obj);
            }

            return Ok(orderList);
        }

        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrder(int orderId)
        {
            var order = await _dbContext.Order.FirstOrDefaultAsync(o => o.Id == orderId);
            if (order == null)
            {
                return NotFound("Order not found.");
            }
            var shippingAddress = await _dbContext.Address.FirstOrDefaultAsync(a => a.Id == order.ShippingAddressId);
            var deliveryMethod = await _dbContext.DeliveryMethods.FirstOrDefaultAsync(d=>d.Id==order.DeliveryMethodId);
            var items = await _dbContext.CartItems.Where(c => c.OrderId == order.Id).ToListAsync();

            return Ok(new
            {
                order= order,
                shippingAddress= shippingAddress,
                deliveryMethod= deliveryMethod,
                items= items
            });
        }

        public async Task<UserInfo> GetUserInfo()
        {
            if (User.Identity?.IsAuthenticated == false)
                return null;

            // Retrieve the email of the authenticated user from claims
            var email = User.FindFirstValue(ClaimTypes.Email);

            if (string.IsNullOrEmpty(email))
                return null;

            var user = await _signInManager.UserManager.Users
                .Include(u => u.Address)
                .FirstOrDefaultAsync(u => u.Email == email);


            if (user == null)
                return null;


            return new UserInfo
            {
                Id = user.Id,
                Email= user.Email,
                Address = user.Address
            };
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> StripeWebhookAsync()
        {
            var json = await new System.IO.StreamReader(Request.Body).ReadToEndAsync();
            var stripeEvent = EventUtility.ConstructEvent(json, Request.Headers["Stripe-Signature"],
                "whsec_7c3c41430936344237fb4b64a481b0fa617d600a570a6258d4058cee3b1bb86f");
            // Handle the event using if-else statements
            if (stripeEvent.Type == "payment_intent.succeeded")
            {
                var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                _logger.LogInformation("PaymentIntent succeeded: {0}", paymentIntent.Id);
                await UpdateOrderStatus(paymentIntent);
            }
            else if (stripeEvent.Type == "payment_intent.payment_failed")
            {
                var failedPaymentIntent = stripeEvent.Data.Object as PaymentIntent;
                _logger.LogInformation("PaymentIntent failed: {0}", failedPaymentIntent.Id);
                await UpdateOrderStatus(failedPaymentIntent, "Failed");
            }
            // Add more event types as needed (e.g., for refunds, cancellations, etc.)
            else
            {
                _logger.LogWarning("Unhandled event type: {0}", stripeEvent.Type);
            }

            return Ok();
        }

        private async Task UpdateOrderStatus(PaymentIntent paymentIntent, string status = "Succeeded")
        {
            // Find your order using the paymentIntentId (you should have stored this in your order record)
            var order = await _dbContext.Order.FirstOrDefaultAsync(o => o.PaymentIntentId == paymentIntent.Id);

            if (order != null)
            {
                //order.Status = status;  // Update the order status (e.g., "Succeeded", "Failed", etc.)
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<Event> StripeWebhook()
        {
            var json = await new System.IO.StreamReader(Request.Body).ReadToEndAsync();
            var stripeSignature = Request.Headers["Stripe-Signature"];

            var secret = "whsec_7c3c41430936344237fb4b64a481b0fa617d600a570a6258d4058cee3b1bb86f"; // Your secret from Stripe Dashboard

            try
            {
                var stripeEvent = EventUtility.ConstructEvent(json, stripeSignature, secret);
                return stripeEvent;
            }
            catch (StripeException e)
            {
                _logger.LogError("Webhook signature verification failed: {0}", e.Message);
                return null;
            }
        }


    }
}
