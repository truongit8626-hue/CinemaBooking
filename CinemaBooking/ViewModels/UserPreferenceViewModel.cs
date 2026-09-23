using System.ComponentModel.DataAnnotations;

namespace CinemaBooking.ViewModels
{
    public class UserPreferenceViewModel
    {
        public List<string> AvailableGenres { get; set; } = new();

        [MinLength(1, ErrorMessage = "Vui lòng chọn ít nhất 1 thể loại phim.")]
        public List<string> SelectedGenres { get; set; } = new();
    }
}