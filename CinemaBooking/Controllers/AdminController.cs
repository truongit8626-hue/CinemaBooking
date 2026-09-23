using CinemaBooking.Data;
using CinemaBooking.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CinemaBooking.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ✅ DANH SÁCH PHIM
        public IActionResult Index()
        {
            var movies = _context.Movies.ToList();
            return View(movies);
        }
        public async Task<IActionResult> Dashboard()
        {
            var totalMovies = await _context.Movies.CountAsync();

            var totalRooms = await _context.Rooms.CountAsync();

            var totalShowtimes = await _context.Showtimes
                .Where(s => !s.IsDeleted)
                .CountAsync();

            var totalTickets = await _context.BookingDetails.CountAsync();
            var totalRevenue = await _context.BookingDetails
    .Select(b => (decimal?)b.Price)
    .SumAsync() ?? 0m;

            ViewBag.TotalRevenue = totalRevenue;

            ViewBag.TotalMovies = totalMovies;
            ViewBag.TotalRooms = totalRooms;
            ViewBag.TotalShowtimes = totalShowtimes;
            ViewBag.TotalTickets = totalTickets;

            var ticketsByMovie = await _context.BookingDetails
                .GroupBy(b => b.Booking.Showtime.Movie.Title)
                .Select(g => new
                {
                    Movie = g.Key,
                    Tickets = g.Count()
                })
                .OrderByDescending(x => x.Tickets)
                .ToListAsync();

            ViewBag.TicketsByMovie = ticketsByMovie;

            return View();
        }


        // ✅ GET: HIỂN THỊ FORM
        public IActionResult CreateMovie()
        {
            return View();
        }

        // ✅ POST: XỬ LÝ THÊM PHIM
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateMovie(Movie movie, IFormFile posterFile)
        {
            ModelState.Remove("Poster");

            if (posterFile != null && posterFile.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(posterFile.FileName);

                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await posterFile.CopyToAsync(stream);
                }

                movie.Poster = "/images/" + fileName;
            }

            if (ModelState.IsValid)
            {
                _context.Add(movie);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "✅ Thêm phim thành công!";

                return RedirectToAction("Index", "Admin");
            }

            return View(movie);
        }

        // GET: CreateMovie
        public IActionResult CreateShowtime(int movieId)
        {
            ViewBag.Rooms = new SelectList(_context.Rooms, "Id", "Name");


            return View(new Showtime
            {
                MovieId = movieId,
             
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateShowtime(Showtime showtime)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Rooms = new SelectList(_context.Rooms, "Id", "Name", showtime.RoomId);
                return View(showtime);
            }

            // ✅ lấy phim (1 lần duy nhất)
            var movie = _context.Movies.Find(showtime.MovieId);

            if (movie == null)
            {
                TempData["Error"] = "Không tìm thấy phim";
                return RedirectToAction("Index");
            }

            // ❗ nếu bạn dùng DateTime thì KHÔNG cần check null nữa

            int bufferMinutes = 15;

            DateTime newStart = showtime.StartTime.Value;

            // 👉 đúng: KHÔNG cộng buffer
            DateTime newEnd = newStart.AddMinutes(movie.Duration);

            // 👉 lưu DB
            showtime.EndTime = newEnd;

            // 👉 check trùng
            var isConflict = _context.Showtimes
                .Where(s => s.RoomId == showtime.RoomId && !s.IsDeleted)
                .Any(s =>
                    newStart <= s.EndTime.Value.AddMinutes(bufferMinutes) &&
                    newEnd > s.StartTime.Value
                );

            if (isConflict)
            {
                TempData["Error"] = "❌ Trùng lịch (đã tính thời gian dọn phòng 15 phút)";

                ViewBag.Rooms = new SelectList(_context.Rooms, "Id", "Name", showtime.RoomId);
                return View(showtime);
            }

            // ✅ lưu showtime
            _context.Showtimes.Add(showtime);
            _context.SaveChanges();

            // ✅ tạo ghế
            var seats = new List<Seat>();
            string[] rows = { "A", "B", "C", "D", "E" };

            foreach (var row in rows)
            {
                for (int i = 1; i <= 10; i++)
                {
                    seats.Add(new Seat
                    {
                        SeatNumber = row + i,
                        RoomId = showtime.RoomId,
                        ShowtimeId = showtime.Id,
                        IsBooked = false
                    });
                }
            }

            _context.Seats.AddRange(seats);
            _context.SaveChanges();

            return RedirectToAction("ManageShowtimes", new { movieId = showtime.MovieId });
        }
        [HttpGet]
        public async Task<IActionResult> EditMovie(int id)
        {
            var movie = await _context.Movies.FindAsync(id);

            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditMovie(Movie movie)
        {
            if (!ModelState.IsValid)
            {
                return View(movie);
            }

            _context.Movies.Update(movie);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "✅ Cập nhật phim thành công!";

            return RedirectToAction(nameof(Index));
        }

        public IActionResult DeleteMovie(int id)
        {
            var movie = _context.Movies.Find(id);

            if (movie != null)
            {
                // Xóa showtime trước
                var showtimes = _context.Showtimes
                    .Where(s => s.MovieId == id)
                    .ToList();

                foreach (var show in showtimes)
                {
                    var seats = _context.Seats
                        .Where(s => s.ShowtimeId == show.Id)
                        .ToList();

                    _context.Seats.RemoveRange(seats);
                }

                _context.Showtimes.RemoveRange(showtimes);
                _context.Movies.Remove(movie);

                _context.SaveChanges();

                TempData["SuccessMessage"] = "✅ Xóa phim thành công!";
            }

            return RedirectToAction("Index");
        }
        public IActionResult ManageShowtimes(int movieId)
        {
            var showtimes = _context.Showtimes
                .Include(s => s.Room)
                .Include(s => s.Movie)
                .Where(s => s.MovieId == movieId && !s.IsDeleted)
                .ToList();

            ViewBag.MovieId = movieId;
            return View(showtimes);
        }

        public async Task<IActionResult> DeleteShowtime(int id)
        {
            var showtime = await _context.Showtimes
                .FirstOrDefaultAsync(s => s.Id == id);

            if (showtime == null)
            {
                TempData["Error"] = "Không tìm thấy suất chiếu!";
                return RedirectToAction("Index");
            }

            int movieId = showtime.MovieId;

            try
            {
                // Nếu suất chiếu đã diễn ra thì chỉ ẩn, không xóa dữ liệu lịch sử
                if (showtime.StartTime < DateTime.Now)
                {
                    showtime.IsDeleted = true;

                    await _context.SaveChangesAsync();

                    TempData["Warning"] =
                        "Suất chiếu đã diễn ra nên chỉ được ẩn khỏi danh sách.";

                    return RedirectToAction(
                        "ManageShowtimes",
                        new { movieId = movieId });
                }

                // Kiểm tra suất chiếu đã có booking hay chưa
                bool hasBookings = await _context.Bookings
                    .AnyAsync(b => b.ShowtimeId == id);

                if (hasBookings)
                {
                    TempData["Error"] =
                        "Không thể xóa! Suất chiếu này đã có vé được đặt.";

                    return RedirectToAction(
                        "ManageShowtimes",
                        new { movieId = movieId });
                }

                // Lấy các ghế được tự động tạo cho suất chiếu
                var seats = await _context.Seats
                    .Where(s => s.ShowtimeId == id)
                    .ToListAsync();

                // Xóa ghế trước vì Seat -> Showtime đang dùng DeleteBehavior.Restrict
                if (seats.Any())
                {
                    _context.Seats.RemoveRange(seats);
                }

                // Sau khi không còn ghế liên kết thì mới xóa Showtime
                _context.Showtimes.Remove(showtime);

                await _context.SaveChangesAsync();

                TempData["Success"] =
                    "✅ Xóa suất chiếu thành công!";
            }
            catch (Exception)
            {
                TempData["Error"] =
                    "Không thể xóa suất chiếu do có dữ liệu liên quan.";
            }

            return RedirectToAction(
                "ManageShowtimes",
                new { movieId = movieId });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Movie movie, IFormFile posterFile)
        {
            ModelState.Remove("Poster");

            if (posterFile != null && posterFile.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(posterFile.FileName);

                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await posterFile.CopyToAsync(stream);
                }

                movie.Poster = "/images/" + fileName;
            }

            if (ModelState.IsValid)
            {
                _context.Add(movie);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(movie);
        }
        public IActionResult GetShowtimesByRoom(int roomId)
        {
            var showtimes = _context.Showtimes
                .Include(s => s.Movie)
                .Where(s => s.RoomId == roomId && !s.IsDeleted)
                .Select(s => new {
                    start = s.StartTime,
                    end = s.EndTime,
                    title = s.Movie.Title
                })
                .ToList();

            return Json(showtimes);
        }
        public IActionResult EditShowtime(int id)
        {
            var showtime = _context.Showtimes
                .Include(s => s.Movie)
                .FirstOrDefault(s => s.Id == id);

            if (showtime == null)
            {
                return NotFound();
            }

            ViewBag.Rooms = new SelectList(_context.Rooms, "Id", "Name", showtime.RoomId);

            // 🔥 load timeline luôn
            var showtimes = _context.Showtimes
                .Include(s => s.Movie)
                .Where(s => s.RoomId == showtime.RoomId && s.Id != showtime.Id && !s.IsDeleted)
                .Select(s => new {
                    start = s.StartTime,
                    end = s.EndTime,
                    title = s.Movie.Title
                })
                .ToList();

            ViewBag.Showtimes = showtimes;
            LoadViewBag(showtime.RoomId);

            return View(showtime);
        }
        [HttpPost]
        public IActionResult EditShowtime(Showtime model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var movie = _context.Movies.Find(model.MovieId);

            // thời lượng phim (phút)
            int duration = movie.Duration;

            DateTime newStart = model.StartTime.Value;
            DateTime newEnd = newStart.AddMinutes(duration);

            // lấy các suất khác cùng phòng (TRỪ chính nó ra)
            var showtimes = _context.Showtimes
                .Include(s => s.Movie)
                .Where(s => s.RoomId == model.RoomId
                         && s.Id != model.Id
                         && !s.IsDeleted)
                .ToList();

            foreach (var s in showtimes)
            {
                DateTime sStart = s.StartTime.Value;
                DateTime sEnd = sStart.AddMinutes(s.Movie.Duration);

                if (!(newEnd.AddMinutes(15) <= sStart || newStart >= sEnd.AddMinutes(15)))
                {
                    ModelState.AddModelError("", "❌ Phải cách nhau ít nhất 15 phút!");

                    ViewBag.Rooms = new SelectList(_context.Rooms, "Id", "Name", model.RoomId);

                    ViewBag.Showtimes = _context.Showtimes
                        .Include(x => x.Movie)
                        .Where(x => x.RoomId == model.RoomId && !x.IsDeleted)
                        .Select(x => new
                        {
                            title = x.Movie.Title,
                            start = x.StartTime,
                            end = x.StartTime.Value.AddMinutes(x.Movie.Duration)
                        }).ToList();

                    return View(model);
                }
            }

            // update
            var showtime = _context.Showtimes.Find(model.Id);
            showtime.StartTime = model.StartTime;
            showtime.RoomId = model.RoomId;

            _context.SaveChanges();

            return RedirectToAction("Index");
        }
        private void LoadViewBag(int roomId)
        {
            ViewBag.Rooms = new SelectList(_context.Rooms, "Id", "Name", roomId);

            ViewBag.Showtimes = _context.Showtimes
                .Where(s => s.RoomId == roomId && !s.IsDeleted)
                .Include(s => s.Movie) // 🔥 QUAN TRỌNG
                .Select(s => new
                {
                    title = s.Movie.Title,
                    start = s.StartTime,
                    end = s.StartTime.Value.AddMinutes(s.Movie.Duration + 15)
                })
                .ToList();
        }

    }
}