using System.ComponentModel.DataAnnotations;
using WebApplication1.Models;

namespace WebApplication1.Views
{
    public class UserRegisterView
    {
        [Required]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
        [Required]
        public string FirstName { get; set; } = string.Empty;
        [Required]
        public string LastName { get; set; } = string.Empty;
        public Address? Address { get; set; }

    }
}
