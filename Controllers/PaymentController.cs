using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using Stripe;
using Stripe.Climate;
using System.Security.Claims;
using WebApplication1.Interfaces;
using WebApplication1.Models;
using WebApplication1.Services;
using WebApplication1.Views;
using Address = WebApplication1.Models.Address;
using AppContext = WebApplication1.Models.AppContext;
using Order = WebApplication1.Models.Order;

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

        public PaymentController(IPaymentService paymentService, ICartService cartService, AppContext dbContext, SignInManager<AppUser> signInManager, IProductService productService)
        {
            _paymentService = paymentService;
            _cartService = cartService;
            _dbContext = dbContext;
            _signInManager = signInManager;
            _productService = productService;
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
        public async Task<ActionResult<bool>> CreateOrder([FromBody] OrderRequest request)
        {
            UserInfo userInfo = await GetUserInfo();
            if (userInfo == null)
                return BadRequest("User address not found.");

            var payment = request.Payment;
            var billingDetails = request.BillingDetails;
            var cartItems = request.CartItems;

            await _dbContext.PaymentSummary.AddAsync(payment);
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
                ShippingEmail= billingDetails.Email,
                PaymentIntentId= billingDetails.PaymentIntentId,
                ShippingAddressId = userInfo.Address.Id,
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

            return Ok(true);
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
            var shippingAddress = await _dbContext.Address.FirstOrDefaultAsync(a => a.Id == order.Id);
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
                Address = user.Address
            };
        }

    }
}
