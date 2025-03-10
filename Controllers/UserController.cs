using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.Design;
using System.Security.Claims;
using WebApplication1.Models;
using WebApplication1.Views;

//using WebApplication1.Services;
using AppContext = WebApplication1.Models.AppContext;


namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public UserController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager=userManager;
            _signInManager=signInManager;
        }

        [HttpPost("register")]
        public async Task<ActionResult> AddUser([FromBody]UserRegisterView user)
        {
            try
            {
                var newUser = new AppUser
                {
                    Email = user.Email,
                    UserName = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName
                };

                var result = await _signInManager.UserManager.CreateAsync(newUser, user.Password);

                if (!result.Succeeded)
                {
                    return BadRequest(result.Errors);
                }
                return Ok(newUser);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<ActionResult> Logout()
        {
            try
            {
                await _signInManager.SignOutAsync();
                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

       




    }
}
