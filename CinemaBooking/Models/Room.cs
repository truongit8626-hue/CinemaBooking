namespace CinemaBooking.Models
{
    public class Room
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty; // ví dụ: A1, B2
        public ICollection<Showtime> Showtimes { get; set; } = new List<Showtime>();
        public int TotalSeats { get; set; }

        public ICollection<Seat> Seats { get; set; } = new List<Seat>();
    }
}
