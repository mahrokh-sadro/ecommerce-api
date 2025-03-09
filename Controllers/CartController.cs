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
            try
            {
                var cart = await _cartService.GetCart("id");
                if (cart == null) return NotFound(new { message = "Cart not found" });
                return Ok(cart);
            }
            catch (Exception e)
            {
                return StatusCode(500, "CartController->Test"+e);
            }
        
        }


        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetCart(string id)
        {
            try
            {
                var cart = await _cartService.GetCart(id);
                if (cart == null) return NotFound(new { message = "Cart not found" });
                return Ok(cart);
            }
            catch (Exception e)
            {
                return StatusCode(500, "CartController->GetCart" + e);
            }
        }

        [HttpPost]
        public async Task<IActionResult> SetCart([FromBody] ShoppingCartView cart)
        {
            try
            {
                if (cart == null || string.IsNullOrEmpty(cart.Id))
                    return BadRequest(new { message = "Invalid cart data" });

                var updatedCart = await _cartService.SetCart(cart);
                return  Ok(updatedCart);
            }
            catch (Exception e)
            {
                return StatusCode(500, "CartController->SetCart" + e);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCart(string id)
        {
            try
            {
                return Ok(await _cartService.DeleteCart(id));
            }
            catch (Exception e)
            {
                return StatusCode(500, "CartController->DeleteCart" + e);
            }
        }
    }
}
