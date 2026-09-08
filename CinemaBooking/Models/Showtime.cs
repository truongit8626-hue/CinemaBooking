using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace CinemaBooking.Models
{
    public class Showtime
    {
        [Key]
        public int Id { get; set; }


        [ValidateNever]
        public Movie? Movie { get; set; }

        [ValidateNever]
        public Room? Room { get; set; }


        [Required(ErrorMessage = "Vui lòng chọn phim")]
        public int MovieId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn phòng")]
        public int RoomId { get; set; }
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-ddTHH:mm}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "Vui lòng nhập thời gian bắt đầu")]
        public DateTime? StartTime { get; set; }
        
       
         public DateTime? EndTime { get; set; }
        public bool IsDeleted { get; set; } = false;
    }

}
