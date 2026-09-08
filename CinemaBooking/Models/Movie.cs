using System.ComponentModel.DataAnnotations;

namespace CinemaBooking.Models
{
    public class Movie
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "❌ Vui lòng nhập tên phim")]
        public string Title { get; set; }

        [Required(ErrorMessage = "❌ Vui lòng nhập thể loại")]
        public string Genre { get; set; }

        [Required(ErrorMessage = "❌ Vui lòng nhập thời lượng")]
        [Range(1, 500, ErrorMessage = "❌ Thời lượng phải > 0")]
        public int Duration { get; set; }

        [Required(ErrorMessage = "❌ Vui lòng nhập mô tả")]
        public string Description { get; set; }

        [Required(ErrorMessage = "❌ Vui lòng chọn poster")]
        public string Poster { get; set; }
    }
}
