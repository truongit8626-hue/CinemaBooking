namespace CinemaBooking.Models
{
    public class Room
    {
        public int Id { get; set; }
        public string Name { get; set; } // ví dụ: A1, B2
        public ICollection<Showtime> Showtimes { get; set; }
        public int TotalSeats { get; set; }

        public ICollection<Seat> Seats { get; set; }
    }
}
