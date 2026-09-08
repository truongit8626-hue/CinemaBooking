namespace CinemaBooking.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int ShowtimeId { get; set; }
        public DateTime BookingTime { get; set; }
        public Showtime Showtime { get; set; }
        

        public ICollection<BookingDetail> BookingDetails { get; set; }


    }
}
