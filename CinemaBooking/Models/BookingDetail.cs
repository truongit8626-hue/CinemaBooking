using System.ComponentModel.DataAnnotations.Schema;
namespace CinemaBooking.Models
{
    public class BookingDetail
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public int SeatId { get; set; }
        public Seat Seat { get; set; }
        public Booking Booking { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }
    }
}
