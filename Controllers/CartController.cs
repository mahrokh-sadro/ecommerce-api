using Microsoft.AspNetCore.Mvc;
using WebApplication1.Interfaces;
using WebApplication1.Views;

namespace WebApplication1.Controllers
{
    [Route("api/cart")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet]
        [Route("test")]
        public async Task<IActionResult> Test()
        {
            var cart = await _cartService.GetCart("id");
            if (cart == null) return NotFound(new { message = "Cart not found" });
            return Ok(cart);
        
        }


        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetCart(string id)
        {
            var cart = await _cartService.GetCart(id);
            if (cart == null) return NotFound(new { message = "Cart not found" });
            return Ok(cart);
        }

        [HttpPost]
        public async Task<IActionResult> SetCart([FromBody] ShoppingCartView cart)
        {
            if (cart == null || string.IsNullOrEmpty(cart.Id))
                return BadRequest(new { message = "Invalid cart data" });

            var updatedCart = await _cartService.SetCart(cart);
            return updatedCart != null ? Ok(updatedCart) : StatusCode(500, "Error saving cart");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCart(string id)
        {
            var success = await _cartService.DeleteCart(id);
            return success ? Ok(new { message = "Cart deleted" }) : NotFound(new { message = "Cart not found" });
        }
    }
}
