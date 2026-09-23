using CinemaBooking.Models;

namespace CinemaBooking.ViewModels
{
    public class RecommendationViewModel
    {
        public string PreferredGenres { get; set; } = string.Empty;

        public string AIResult { get; set; } = string.Empty;

        public List<AIRecommendationItem> Recommendations { get; set; }
            = new List<AIRecommendationItem>();

        public List<Movie> Movies { get; set; }
            = new List<Movie>();
    }
}