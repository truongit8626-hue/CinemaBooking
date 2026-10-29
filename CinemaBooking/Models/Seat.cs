namespace CinemaBooking.Models
{
    public class Seat
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public string SeatNumber { get; set; } = string.Empty;
        public bool IsBooked { get; set; }

        public int ShowtimeId { get; set; }
        // FK ShowtimeId is required; EF Core populates this navigation when it is loaded.
        public Showtime Showtime { get; set; } = null!;

    }
}
