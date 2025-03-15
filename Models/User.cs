using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;
using System.Text;

namespace animal_shelter_app.Models
{
    [Table("user")]
    public class User
    {
        [Key]
        [Column("user_id")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "First name is required.")]
        [Column("user_name")]
        [StringLength(50)]
        public required string UserName { get; set; }

        [Required(ErrorMessage = "Surname is required.")]
        [Column("user_surname")]
        [StringLength(50)]
        public required string UserSurname { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [Column("user_email")]
        [StringLength(100)]
        public required string UserEmail { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
        [Column("user_password")]
        [StringLength(100)]
        public required string UserPassword { get; set; }

        [Column("user_role")]
        public bool UserRole { get; set; } // false => user, true => admin

        [Column("present_animals")]
        public int PresentAnimals { get; set; }

        // SHA256 hashing. 
        public static string HashPassword(string password)
        {
            using SHA256 sha256 = SHA256.Create();
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
        }
    }
}
