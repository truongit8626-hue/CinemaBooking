namespace CinemaBooking.ViewModels
{
    public class TicketViewModel
    {
        public string MovieName { get; set; } = string.Empty;
        public string Showtime { get; set; } = string.Empty;
        public string Seats { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public string Poster { get; set; } = string.Empty;
        public int BookingId { get; set; }
        public string QRCode { get; set; } = string.Empty;
        public string SeatNumber { get; set; } = string.Empty;
        public decimal TotalPrice { get; set; }
        public int Id { get; set; }

    }
}
