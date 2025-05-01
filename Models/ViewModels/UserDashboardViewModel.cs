using System.Collections.Generic;
using System.Linq;

namespace animal_shelter_app.Models.ViewModels
{
    public record UserDashboardViewModel(
        List<string> SliderUrls,   //  – e.g. "/slider/pic.jpg"
        IEnumerable<ArticleInformations> Articles);
}
