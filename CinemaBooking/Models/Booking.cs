namespace CinemaBooking.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public int ShowtimeId { get; set; }
        public DateTime BookingTime { get; set; }
        // FK ShowtimeId is required; EF Core populates this navigation when it is loaded.
        public Showtime Showtime { get; set; } = null!;
        

        public ICollection<BookingDetail> BookingDetails { get; set; } = new List<BookingDetail>();


    }
}
