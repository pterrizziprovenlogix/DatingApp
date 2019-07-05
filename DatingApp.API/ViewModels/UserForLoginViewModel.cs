using System.ComponentModel.DataAnnotations;

namespace DatingApp.API.ViewModels
{
    public class UserForLoginViewModel
    {
        [Required(ErrorMessage = "Username is required")]
        [StringLength(50, MinimumLength = 4, ErrorMessage = "4-50 characters allowed for username")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(50, MinimumLength = 8, ErrorMessage = "8-50 characters allowed for password")]
        public string Password { get; set; }
    }
}
