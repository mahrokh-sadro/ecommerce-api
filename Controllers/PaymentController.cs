using Microsoft.AspNetCore.Mvc;
using WebApplication1.Interfaces;
using WebApplication1.Models;
using WebApplication1.Services;
using WebApplication1.Views;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly ICartService _cartService;

        public PaymentController(IPaymentService paymentService, ICartService cartService)
        {
            _paymentService = paymentService;
            _cartService = cartService;
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
    }
}
