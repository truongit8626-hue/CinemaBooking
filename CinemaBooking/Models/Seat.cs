namespace CinemaBooking.Models
{
    public class Seat
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public string SeatNumber { get; set; }
        public bool IsBooked { get; set; }

        public int ShowtimeId { get; set; }
        public Showtime Showtime { get; set; }

    }
}
