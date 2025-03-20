using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;
using System.Security.Claims;
using WebApplication1.Interfaces;
using WebApplication1.Models;
using WebApplication1.Services;
using WebApplication1.Views;
using Address = WebApplication1.Models.Address;
using AppContext = WebApplication1.Models.AppContext;

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
        public async Task<ActionResult<Boolean>> CreateOrder([FromBody] OrderRequest request)
        {

            var address = await GetUserInfo();
            //ge t the adrs from user
            var payment = request.Payment;
            var billingDetails = request.BillingDetails;
            var cartItems = request.CartItems;
           
            //create payment
            _dbContext.PaymentSummary.Add(payment);
            await _dbContext.SaveChangesAsync();

            
            //get the id

            //get delivery method info
            var deliveryMethod= _dbContext.DeliveryMethods.Where(d=>d.Id==billingDetails.deliveryMethodId).FirstOrDefault();

            decimal subtotal = 0m;
            decimal taxRate = 0.13m;
            foreach (var item in cartItems)
            {
                var product = await _productService.GetProductById(item.ProductId);
                if (product == null) continue;
                subtotal += (decimal)product.Price * item.Quantity;
            }
            decimal discount = 0m;
            Order order = new Order(billingDetails.Email, billingDetails.PaymentIntentId)
            {
                ShippingAddressId = address.Id, 
                DeliveryMethodId = (int)billingDetails.deliveryMethodId, 
                PaymentSummaryId = payment.Id, 
                Subtotal = subtotal,
                TaxAmount=taxRate* subtotal,
                Total=subtotal*(1+ taxRate) - discount,
                //CartItems = cartItems.Select(item => new CartItem
                //{
                //    ProductId = item.ProductId,
                //    Quantity = item.Quantity
                //}).ToList(),

                Discount = 0
            };
            await _dbContext.Order.AddAsync(order);
            await _dbContext.SaveChangesAsync();

            foreach(var item in cartItems)
            {
                var cartItem=new CartItem();
                cartItem.ProductId= item.ProductId;
                cartItem.Quantity= item.Quantity;
                cartItem.OrderId= order.Id;

                await _dbContext.CartItems.AddAsync(cartItem);
                await _dbContext.SaveChangesAsync();    s
            }



            return true;
        }
        public async Task<Address?> GetUserInfo()
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


            return user?.Address;
        }

    }
}
