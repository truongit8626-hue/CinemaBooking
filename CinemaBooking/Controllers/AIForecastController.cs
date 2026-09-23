using CinemaBooking.Data;
using CinemaBooking.Services.AI;
using CinemaBooking.Prompts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CinemaBooking.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AIForecastController : Controller
    {
        private readonly IGeminiService _geminiService;
        private readonly ApplicationDbContext _context;

        public AIForecastController(
            IGeminiService geminiService,
            ApplicationDbContext context)
        {
            _geminiService = geminiService;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Forecast(int days = 30)
        {
            if (days != 7 && days != 14 && days != 30)
            {
                days = 30;
            }

            var fromDate = DateTime.Now.AddDays(-30);

            var bookingData = await _context.BookingDetails
                .AsNoTracking()
                .Include(bd => bd.Booking)
                    .ThenInclude(b => b.Showtime)
                        .ThenInclude(s => s.Movie)
                .Include(bd => bd.Booking)
                    .ThenInclude(b => b.Showtime)
                        .ThenInclude(s => s.Room)
                .Where(bd => bd.Booking.BookingTime >= fromDate)
                .ToListAsync();

            var dailyData = bookingData
                .GroupBy(bd => bd.Booking.BookingTime.Date)
                .OrderBy(g => g.Key)
                .Select(g => new
                {
                    Date = g.Key.ToString("dd/MM/yyyy"),
                    Tickets = g.Count(),
                    Revenue = g.Sum(x => x.Price),
                    Occupancy = g
                        .GroupBy(x => x.Booking.ShowtimeId)
                        .Average(showtime =>
                        {
                            var totalSeats =
                                showtime.First().Booking.Showtime.Room?.TotalSeats ?? 0;

                            return totalSeats > 0
                                ? (double)showtime.Count() / totalSeats * 100
                                : 0;
                        })
                })
                .ToList();

            if (!dailyData.Any())
            {
                ViewBag.AIError =
                    "⚠️ Chưa có đủ dữ liệu đặt vé trong 30 ngày gần nhất để thực hiện dự báo.";

                ViewBag.Days = days;
                ViewBag.DataCount = 0;

                return View("Index");
            }

            var dataText = string.Join(
                "\n",
                dailyData.Select(d =>
                    $"{d.Date} | Vé: {d.Tickets} | " +
                    $"Doanh thu: {d.Revenue:N0} VND | " +
                    $"Lấp đầy: {d.Occupancy:F2}%"));

            var userPrompt = ForecastPrompt.BuildUserPrompt(
                dataText,
                days);

            string aiResult;

            try
            {
                aiResult = await _geminiService.GenerateAsync(
                    ForecastPrompt.SystemPrompt,
                    userPrompt);
            }
            catch (TimeoutException)
            {
                ViewBag.AIError =
                    "⚠️ AI phản hồi quá lâu. Vui lòng thử lại sau.";

                ViewBag.Days = days;
                ViewBag.DataCount = dailyData.Count;

                return View("Index");
            }
            catch (HttpRequestException ex)
                when (ex.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
            {
                ViewBag.AIError =
                    "⚠️ AI đang nhận quá nhiều yêu cầu. " +
                    "Vui lòng thử lại sau.";

                ViewBag.Days = days;
                ViewBag.DataCount = dailyData.Count;

                return View("Index");
            }
            catch (InvalidOperationException ex)
                when (ex.Message.Contains("quá dài"))
            {
                ViewBag.AIError =
                    "⚠️ Dữ liệu gửi đến AI quá lớn. " +
                    "Vui lòng thử lại sau.";

                ViewBag.Days = days;
                ViewBag.DataCount = dailyData.Count;

                return View("Index");
            }
            catch (Exception)
            {
                ViewBag.AIError =
                    "⚠️ Không thể tạo dự báo bằng AI lúc này. " +
                    "Vui lòng thử lại sau.";

                ViewBag.Days = days;
                ViewBag.DataCount = dailyData.Count;

                return View("Index");
            }

            ViewBag.AIResult = aiResult;
            ViewBag.Days = days;
            ViewBag.DataCount = dailyData.Count;

            return View("Index");
        }
    }
}