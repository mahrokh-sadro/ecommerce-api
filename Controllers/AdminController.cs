using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;
using WebApplication1.Interfaces;
using WebApplication1.Models;
using WebApplication1.Views;

//using WebApplication1.Services;
using AppContext = WebApplication1.Models.AppContext;


namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/admin")]
    public class AdminController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly AppContext _dbContext;
        private readonly IAdminService _adminService;

        public AdminController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, AppContext dbContext,
            IAdminService adminService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _dbContext = dbContext;
            _adminService = adminService;
        }

        [Authorize(Roles = "admin")]
        [HttpGet("orders")]
        public async Task<IActionResult> GetOrders()
        {
            try
            {
                var cart = await _adminService.GetOrders();
                return Ok(cart);
            }
            catch (Exception e)
            {
                return StatusCode(500, "AdminController->GetOrders" + e);
            }
        }

        [HttpPost("refund/{orderId}")]
        public async Task<IActionResult> RefundPaymentAsync(int orderId)
        {
            try
            {
                var refund = await _adminService.RefundPaymentAsync(orderId);
                return Ok(new { message = "Refund successful", refundId = refund.Id });
            }
            catch (Exception e)
            {
                return StatusCode(500, "AdminController->refund" + e);
            }
        }
    }
}
