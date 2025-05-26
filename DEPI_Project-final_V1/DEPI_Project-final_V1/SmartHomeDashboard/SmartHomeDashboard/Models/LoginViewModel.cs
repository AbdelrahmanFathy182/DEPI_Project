using System.ComponentModel.DataAnnotations;

namespace SmartHomeDashboard.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{6,}$",
            ErrorMessage = "Password must be at least 6 characters and include letters and numbers.")]
        public string Password { get; set; }
    }
}
