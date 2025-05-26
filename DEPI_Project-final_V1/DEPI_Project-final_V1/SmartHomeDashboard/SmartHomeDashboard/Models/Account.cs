using System.ComponentModel.DataAnnotations;

namespace SmartHomeDashboard.Models
{
    public class Account
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string UserName { get; set; }
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [Required]
        public string Email { get; set; }
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{6,}$", ErrorMessage = "Password must be at least 6 characters and include letters and numbers.")]
        [Required]
        public string HashPassword { get; set; }
        public DateTime Created_at { get; set; }=DateTime.Now;

        public ICollection<Room> Rooms { get; set; }

    }
}
