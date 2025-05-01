using System.ComponentModel.DataAnnotations;

namespace animal_shelter_app.Models.ViewModels
{
    public class AddArticleViewModel
    {
        [Required] public string BlogTitle { get; set; } = "";
        [Required] public string BlogContent { get; set; } = "";
        [Required] public IFormFile ImageFile { get; set; } = default!;   // slider image
        public string? BlogUrl { get; set; }
    }
}
