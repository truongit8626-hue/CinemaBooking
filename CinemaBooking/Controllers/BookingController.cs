using CinemaBooking.Data;
using CinemaBooking.Models;
using CinemaBooking.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;

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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Book(int showtimeId, int[] seatIds)
        {
            if (seatIds == null || seatIds.Length == 0 || seatIds.Distinct().Count() != seatIds.Length)
            {
                TempData["Error"] = "❌ Vui lòng chọn ghế hợp lệ!";
                return RedirectToAction("SelectSeats", new { showtimeId });
            }

            var showtime = await _context.Showtimes
                .Include(x => x.Movie)
                .FirstOrDefaultAsync(x => x.Id == showtimeId && !x.IsDeleted);
            if (showtime == null)
            {
                return NotFound();
            }
            if (showtime.Movie == null)
            {
                return NotFound();
            }

            var validSeatIds = await _context.Seats
                .Where(x => x.ShowtimeId == showtimeId && seatIds.Contains(x.Id))
                .Select(x => x.Id)
                .ToListAsync();
            if (validSeatIds.Count != seatIds.Length)
            {
                TempData["Error"] = "❌ Ghế không hợp lệ cho suất chiếu này.";
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
            var seats = await _context.Seats
                .Where(x => seatIds.Contains(x.Id))
                .Select(x => x.SeatNumber)
                .ToListAsync();

            var ticket = new TicketViewModel
            {
                MovieName = showtime.Movie.Title,
                Showtime = showtime.StartTime?.ToString("HH:mm dd/MM/yyyy") ?? "Chưa xác định",
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmPayment(
            int showtimeId,
            string seatIds)
        {
            if (showtimeId <= 0 || string.IsNullOrWhiteSpace(seatIds))
            {
                return Content("❌ Dữ liệu thanh toán không hợp lệ");
            }

            var seatIdValues = seatIds
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .ToList();
            if (seatIdValues.Count == 0 || seatIdValues.Any(value => !int.TryParse(value, out _)))
            {
                return BadRequest("❌ Dữ liệu ghế không hợp lệ.");
            }

            var seatIdList = seatIdValues.Select(int.Parse).ToList();
            if (seatIdList.Distinct().Count() != seatIdList.Count)
            {
                return BadRequest("❌ Dữ liệu ghế không hợp lệ.");
            }

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Challenge();
            }

            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                var showtimeExists = await _context.Showtimes
                    .AnyAsync(x => x.Id == showtimeId && !x.IsDeleted);
                var validSeatCount = await _context.Seats
                    .CountAsync(x => x.ShowtimeId == showtimeId && seatIdList.Contains(x.Id));
                if (!showtimeExists || validSeatCount != seatIdList.Count)
                {
                    await transaction.RollbackAsync();
                    return BadRequest("❌ Dữ liệu suất chiếu hoặc ghế không hợp lệ.");
                }

                // Ràng buộc unique ở CSDL (khi được áp dụng) là lớp bảo vệ cuối
                // cùng đối với các request đến đồng thời.
                var alreadyBookedSeatIds = await _context.BookingDetails
                    .Where(x => x.Booking.ShowtimeId == showtimeId && seatIdList.Contains(x.SeatId))
                    .Select(x => x.SeatId)
                    .ToListAsync();
                if (alreadyBookedSeatIds.Count > 0)
                {
                    await transaction.RollbackAsync();
                    TempData["Error"] = "❌ Một hoặc nhiều ghế đã được người khác đặt. Vui lòng chọn lại.";
                    return RedirectToAction("SelectSeats", new { showtimeId });
                }

                var booking = new Booking
                {
                    UserId = user.Id,
                    ShowtimeId = showtimeId,
                    BookingTime = DateTime.Now
                };

                _context.Bookings.Add(booking);
                await _context.SaveChangesAsync();

                foreach (var seatId in seatIdList)
                {
                    _context.BookingDetails.Add(new BookingDetail
                    {
                        BookingId = booking.Id,
                        SeatId = seatId,
                        Price = 50000
                    });
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                // Session chỉ còn là dữ liệu tạm, xóa sau khi đặt vé thành công
                HttpContext.Session.Remove("seatIds");
                HttpContext.Session.Remove("showtimeId");

                return RedirectToAction(
                    "Ticket",
                    new { id = booking.Id });
            }
            catch (DbUpdateException)
            {
                await transaction.RollbackAsync();
                TempData["Error"] =
                    "❌ Một hoặc nhiều ghế vừa được người khác đặt. Vui lòng chọn lại.";

                return RedirectToAction("SelectSeats", new { showtimeId });
            }
            catch
            {
                await transaction.RollbackAsync();

                return Content("❌ Lỗi thanh toán");
            }
        }
        // 🔹 Vé
        public async Task<IActionResult> Ticket(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var booking = await _context.Bookings
                .Include(x => x.Showtime)
                    .ThenInclude(s => s.Movie)
                .Include(x => x.BookingDetails)
                    .ThenInclude(d => d.Seat)
                .FirstOrDefaultAsync(x => x.Id == id && x.UserId == user.Id);

            if (booking?.Showtime?.Movie == null) return NotFound();

            var ticket = new TicketViewModel
            {
                MovieName = booking.Showtime.Movie.Title,
                Showtime = booking.Showtime.StartTime?.ToString("HH:mm dd/MM/yyyy") ?? "Chưa xác định",
                Seats = string.Join(", ", booking.BookingDetails
                    .Where(x => x.Seat != null)
                    .Select(x => x.Seat == null ? string.Empty : x.Seat.SeatNumber)),
                Total = booking.BookingDetails.Sum(x => x.Price),
                BookingId = booking.Id,
                Poster = booking.Showtime.Movie.Poster ?? string.Empty,
                QRCode = $"https://api.qrserver.com/v1/create-qr-code/?size=200x200&data=BOOKING_{booking.Id}"
            };

            return View(ticket);
        }
        // 🔹 Lịch sử
        public async Task<IActionResult> History()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();
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
        
        public async Task<IActionResult> CheckTicket(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();
            var booking = await _context.Bookings.FirstOrDefaultAsync(x => x.Id == id && x.UserId == user.Id);

            if (booking == null)
                return Content("❌ Không có vé");

            return Content("✅ Vé hợp lệ - ID: " + id);
        }
        public async Task<IActionResult> MyTickets()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();
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
                Poster = b.Showtime?.Movie?.Poster ?? string.Empty,
                QRCode = $"https://api.qrserver.com/v1/create-qr-code/?size=200x200&data=Booking:{b.Id}"
            }).ToList();

            return View(tickets); // ✅ đúng model
        }

        public async Task<IActionResult> TicketDetail(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();
            var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.Id == id && b.UserId == user.Id);

            if (booking == null) return NotFound();

            var showtime = await _context.Showtimes
                .Include(s => s.Movie)
                .FirstOrDefaultAsync(s => s.Id == booking.ShowtimeId);
            if (showtime?.Movie == null) return NotFound();

            var seats = await _context.BookingDetails
                .Include(x => x.Seat)
                .Where(x => x.BookingId == id)
                .Select(x => x.Seat.SeatNumber)
                .ToListAsync();

            var total = await _context.BookingDetails
                .Where(x => x.BookingId == id)
                .SumAsync(x => x.Price);

            var model = new TicketViewModel
            {
                MovieName = showtime.Movie.Title,
                Showtime = showtime.StartTime?.ToString("HH:mm dd/MM/yyyy") ?? "Chưa xác định",
                Seats = string.Join(", ", seats),
                Total = total,
                BookingId = booking.Id,
                Poster = showtime.Movie.Poster ?? string.Empty,
                QRCode = $"https://api.qrserver.com/v1/create-qr-code/?size=200x200&data=Booking:{booking.Id}"
            };

            return View("Ticket", model);
        }

        
    }
}
