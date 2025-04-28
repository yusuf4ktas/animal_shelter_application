using System.ComponentModel.DataAnnotations;

namespace animal_shelter_app.Models.ViewModels
{
    public class EditProfileViewModel
    {
        public int UserId { get; set; }

        [Required(ErrorMessage = "First name is required.")]
        public required string UserName { get; set; }

        [Required(ErrorMessage = "Surname is required.")]
        public required string UserSurname { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public required string UserEmail { get; set; }

        [Display(Name = "Present Animals")]
        public int PresentAnimals { get; set; }

        // Password change – not required.
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
        [Display(Name = "New Password (leave blank to keep current)")]
        public string? NewPassword { get; set; }
    }
}
