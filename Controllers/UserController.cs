using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        private readonly AppContext _dbContext;

        public UserController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, AppContext dbContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _dbContext = dbContext;
        }

        [HttpPost("register")]
        public async Task<ActionResult> AddUser([FromBody]UserRegisterView user)
        {
            try
            {
                Address? newAddress =null;
                if (user.Address!=null)
                {
                    newAddress = new Address 
                    {
                        Name=user.Address.Name,
                        Line1 = user.Address.Line1,
                        Line2 = user.Address.Line2,
                        City = user.Address.City,
                        Country = user.Address.Country,
                        State = user.Address.State,
                        PostalCode = user.Address.PostalCode
                    };

                    _dbContext.Address.Add(newAddress);
                    await _dbContext.SaveChangesAsync();
                }

                var newUser = new AppUser
                {
                    Email = user.Email,
                    UserName = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Address= newAddress
                };

                var result = await _signInManager.UserManager.CreateAsync(newUser, user.Password);

                if (!result.Succeeded)
                {
                    //return BadRequest(result.Errors);
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(error.Code, error.Description);
                    }

                    return ValidationProblem();
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

        [Authorize]
        [HttpPost("address")]
        public async Task<ActionResult<Address>> AddOrUpdateAddress(Address address)
        {
            if (User.Identity?.IsAuthenticated == false)
                return NoContent();

            // Retrieve the email of the authenticated user from claims
            var email = User.FindFirstValue(ClaimTypes.Email);

            if (string.IsNullOrEmpty(email))
                return NotFound("User email not found in claims.");

            // Fetch user by email
            var user = await _signInManager.UserManager.Users
                 .Include(u => u.Address)  
                 .FirstOrDefaultAsync(u => u.Email == email);

            if (user.Address == null)
            {
                user.Address = new Address
                {
                    Name = address.Name,    
                    Line1 = address.Line1,
                    Line2 = address.Line2,
                    City = address.City,
                    Country = address.Country,
                    State = address.State,
                    PostalCode = address.PostalCode
                };

                _dbContext.Address.Add(user.Address);
            }
            else
            {
                //user.Address.UpdateFromDto(addressDto);
                user.Address.Line1= address.Line1;
                user.Address.Line2= address.Line2;
                user.Address.City= address.City;
                user.Address.Country= address.Country;  
                user.Address.State= address.State;
                user.Address.PostalCode= address.PostalCode;
            }
            await _dbContext.SaveChangesAsync();
            var result = await _signInManager.UserManager.UpdateAsync(user);

            if (!result.Succeeded) return BadRequest("Problem updating user address");

            return Ok(user.Address);
        }

        [HttpGet("user-info")]
        public async Task<ActionResult> GetUserInfo()
        {
            if (User.Identity?.IsAuthenticated == false)
                return NoContent();

            // Retrieve the email of the authenticated user from claims
            var email = User.FindFirstValue(ClaimTypes.Email);

            if (string.IsNullOrEmpty(email))
                return NotFound("User email not found in claims.");

            // Fetch user by email
            //var user = await _signInManager.UserManager.FindByEmailAsync(email);
            // Fetch user by email, including the Address
            var user = await _signInManager.UserManager.Users
                .Include(u => u.Address)  // Ensure Address is loaded
                .FirstOrDefaultAsync(u => u.Email == email);


            if (user == null)
                return NotFound("User not found.");

            //var role = User.FindFirstValue(ClaimTypes.Role);

            return Ok(new
            {
                user.FirstName,
                user.LastName,
                user.Email,
                user.Address,
                role= User.FindFirstValue(ClaimTypes.Role)

            });
        }

        [HttpGet("auth-status")]
        public ActionResult GetAuthState()
        {
            return Ok(User.Identity?.IsAuthenticated ?? false);
        }


    }
}
