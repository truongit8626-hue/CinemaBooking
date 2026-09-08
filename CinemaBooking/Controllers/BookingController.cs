using CinemaBooking.Data;
using CinemaBooking.Models;
using CinemaBooking.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Net.Sockets;

namespace CinemaBooking.Controllers
{
    [Authorize]
    public class BookingController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public BookingController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // 🔹 Danh sách suất chiếu
        public IActionResult Showtimes(int movieId)
        {
            var data = _context.Showtimes
                .Where(x => x.MovieId == movieId && !x.IsDeleted)
                .ToList();

            return View(data);
        }

        // 🔹 Chọn ghế
        public IActionResult SelectSeats(int showtimeId)
        {

            var showtime = _context.Showtimes
                .Include(x => x.Movie)
                .Include(x => x.Room)
                .FirstOrDefault(x => x.Id == showtimeId && !x.IsDeleted);
            if (showtime == null)
            {
                return Content("KHÔNG CÓ SHOWTIME");
            }

            if (showtime.Room == null)
            {
                return Content("KHÔNG CÓ ROOM");
            }

           

            ViewBag.Room = showtime.Room?.Name; // 👈 lấy từ DB
            var seats = _context.Seats
            .Where(s => s.ShowtimeId == showtimeId)
            .ToList();
            var booked = _context.BookingDetails
                 .Include(x => x.Booking)
                 .Where(x => x.Booking.ShowtimeId == showtimeId)
                 .Select(x => x.SeatId)
                 .ToList();

            ViewBag.Booked = booked;
            ViewBag.ShowtimeId = showtimeId;

            return View(seats);
        }

        // 🔹 Đặt vé
        [HttpPost]
        public IActionResult Book(int showtimeId, int[] seatIds)
        {
            if (seatIds == null || seatIds.Length == 0)
            {
                TempData["Error"] = "❌ Vui lòng chọn ghế!";
                return RedirectToAction("SelectSeats", new { showtimeId });
            }

            // 🔥 check ghế đã bị đặt
            var bookedSeats = _context.BookingDetails
                .Include(x => x.Booking)
                .Where(x => x.Booking.ShowtimeId == showtimeId)
                .Select(x => x.SeatId)
                .ToList();

            if (seatIds.Any(id => bookedSeats.Contains(id)))
            {
                TempData["Error"] = "❌ Ghế đã có người đặt!";
                return RedirectToAction("SelectSeats", new { showtimeId });
            }

            // 🔥 LƯU TẠM VÀO SESSION
            HttpContext.Session.SetString("seatIds", string.Join(",", seatIds));
            HttpContext.Session.SetInt32("showtimeId", showtimeId);

            // 🔥 tạo view model để hiển thị payment
            var showtime = _context.Showtimes
                .Include(x => x.Movie)
                .FirstOrDefault(x => x.Id == showtimeId);

            var seats = _context.Seats
                .Where(x => seatIds.Contains(x.Id))
                .Select(x => x.SeatNumber)
                .ToList();

            var ticket = new TicketViewModel
            {
                MovieName = showtime.Movie.Title,
                Showtime = showtime.StartTime?.ToString("HH:mm dd/MM/yyyy"),
                Seats = string.Join(", ", seats),
                Total = seatIds.Length * 50000
            };

            return View("Payment", ticket);
        }

        // 🔹 Trang thanh toán
        public IActionResult Payment()
        {
            return View();
        }

        // 🔹 Xác nhận thanh toán (fake)
        [HttpPost]
        public async Task<IActionResult> ConfirmPayment()
        {
            var seatIdsStr = HttpContext.Session.GetString("seatIds");
            var showtimeId = HttpContext.Session.GetInt32("showtimeId");

            if (seatIdsStr == null || showtimeId == null)
            {
                return Content("❌ Session hết hạn");
            }

            var seatIds = seatIdsStr.Split(',').Select(int.Parse).ToList();

            var user = await _userManager.GetUserAsync(User);

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var booking = new Booking
                    {
                        UserId = user.Id,
                        ShowtimeId = showtimeId.Value,
                        BookingTime = DateTime.Now
                    };

                    _context.Bookings.Add(booking);
                    _context.SaveChanges();

                    foreach (var seatId in seatIds)
                    {
                        _context.BookingDetails.Add(new BookingDetail
                        {
                            BookingId = booking.Id,
                            SeatId = seatId,
                            Price = 50000
                        });
                    }

                    _context.SaveChanges();
                    transaction.Commit();

                    // 🔥 clear session
                    HttpContext.Session.Remove("seatIds");
                    HttpContext.Session.Remove("showtimeId");

                    return RedirectToAction("Ticket", new { id = booking.Id });
                }
                catch
                {
                    transaction.Rollback();
                    return Content("❌ Lỗi thanh toán");
                }
            }
        }
        // 🔹 Vé
        public IActionResult Ticket(int id)
        {
            var booking = _context.Bookings
                .Include(x => x.Showtime)
                    .ThenInclude(s => s.Movie)
                .Include(x => x.BookingDetails)
                    .ThenInclude(d => d.Seat)
                .FirstOrDefault(x => x.Id == id);

            if (booking == null) return NotFound();

            var ticket = new TicketViewModel
            {
                MovieName = booking.Showtime.Movie.Title,
                Showtime = booking.Showtime.StartTime?.ToString("HH:mm dd/MM/yyyy"),
                Seats = string.Join(", ", booking.BookingDetails.Select(x => x.Seat.SeatNumber)),
                Total = booking.BookingDetails.Sum(x => x.Price),
                BookingId = booking.Id,
                Poster = booking.Showtime.Movie.Poster,
                QRCode = $"https://api.qrserver.com/v1/create-qr-code/?size=200x200&data=BOOKING_{booking.Id}"
            };

            return View(ticket);
        }
        // 🔹 Lịch sử
        public async Task<IActionResult> History()
        {
            var user = await _userManager.GetUserAsync(User);
            var now = DateTime.Now;

            var history = _context.Bookings
                .Include(x => x.Showtime)
                    .ThenInclude(s => s.Movie) // 👈 thêm luôn Movie cho tiện
                .Include(x => x.BookingDetails)
                    .ThenInclude(d => d.Seat) // 👈 lấy luôn ghế
                .Where(x => x.UserId == user.Id && x.Showtime.StartTime <= now)
                .ToList();

            return View(history);
        }

        // 🔹 Thành công
        public IActionResult Success()
        {
            return View();
        }
        [HttpGet]
        
        public IActionResult CheckTicket(int id)
        {
            var booking = _context.Bookings.FirstOrDefault(x => x.Id == id);

            if (booking == null)
                return Content("❌ Không có vé");

            return Content("✅ Vé hợp lệ - ID: " + id);
        }
        public async Task<IActionResult> MyTickets()
        {
            var user = await _userManager.GetUserAsync(User);
            var now = DateTime.Now;

            var bookings = _context.Bookings
                .Include(x => x.Showtime)
                    .ThenInclude(s => s.Movie)
                .Include(x => x.BookingDetails)
                    .ThenInclude(d => d.Seat)
                .Where(x => x.UserId == user.Id && x.Showtime.StartTime > now)
                .ToList();

            // 🔥 convert sang TicketViewModel
            var tickets = bookings.Select(b => new TicketViewModel
            {
                MovieName = b.Showtime?.Movie?.Title ?? "N/A",
                Showtime = b.Showtime?.StartTime?.ToString("HH:mm dd/MM/yyyy") ?? "N/A",
                Seats = b.BookingDetails != null && b.BookingDetails.Any()
                    ? string.Join(", ", b.BookingDetails
                        .Where(x => x.Seat != null)
                        .Select(x => x.Seat.SeatNumber))
                    : "Chưa chọn ghế",

                 Total = b.BookingDetails != null && b.BookingDetails.Any()
                    ? b.BookingDetails.Sum(x => x.Price)
                    : 0,
                BookingId = b.Id,
                Poster = b.Showtime?.Movie?.Poster,
                QRCode = $"https://api.qrserver.com/v1/create-qr-code/?size=200x200&data=Booking:{b.Id}"
            }).ToList();

            return View(tickets); // ✅ đúng model
        }

        public IActionResult TicketDetail(int id)
        {
            var booking = _context.Bookings.FirstOrDefault(b => b.Id == id);

            if (booking == null) return NotFound();

            var showtime = _context.Showtimes
                .Include(s => s.Movie)
                .FirstOrDefault(s => s.Id == booking.ShowtimeId);

            var seats = _context.BookingDetails
                .Include(x => x.Seat)
                .Where(x => x.BookingId == id)
                .Select(x => x.Seat.SeatNumber)
                .ToList();

            var total = _context.BookingDetails
                .Where(x => x.BookingId == id)
                .Sum(x => x.Price);

            var model = new TicketViewModel
            {
                MovieName = showtime.Movie.Title,
                Showtime = showtime.StartTime?.ToString("HH:mm dd/MM/yyyy"),
                Seats = string.Join(", ", seats),
                Total = total,
                BookingId = booking.Id,    Poster = showtime.Movie.Poster,
                QRCode = $"https://api.qrserver.com/v1/create-qr-code/?size=200x200&data=Booking:{booking.Id}"
            };

            return View("Ticket", model);
        }

        
    }
}