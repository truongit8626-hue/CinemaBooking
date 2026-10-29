using System.ComponentModel.DataAnnotations.Schema;
namespace CinemaBooking.Models
{
    public class BookingDetail
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public int SeatId { get; set; }
        // Both foreign keys are required; navigations are populated by EF Core when loaded.
        public Seat Seat { get; set; } = null!;
        public Booking Booking { get; set; } = null!;
        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }
    }
}
