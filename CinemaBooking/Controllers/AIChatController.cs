using CinemaBooking.Data;
using CinemaBooking.Prompts;
using CinemaBooking.Services.AI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace CinemaBooking.Controllers
{
    public class AIChatController : Controller
    {
        private readonly IGeminiService _geminiService;
        private readonly ApplicationDbContext _context;

        public AIChatController(
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
        public async Task<IActionResult> Ask(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return Json(new
                {
                    success = false,
                    message = "Vui lòng nhập câu hỏi."
                });
            }

            // Giới hạn độ dài câu hỏi người dùng.
            const int maxMessageLength = 2_000;

            if (message.Length > maxMessageLength)
            {
                return Json(new
                {
                    success = false,
                    message =
                        "Câu hỏi quá dài. Vui lòng rút gọn câu hỏi và thử lại."
                });
            }

            try
            {
                var movies = await _context.Movies
                    .AsNoTracking()
                    .Select(m => new
                    {
                        m.Id,
                        m.Title,
                        m.Genre,
                        m.Duration,
                        m.Description
                    })
                    .OrderBy(m => m.Title)
                    .ToListAsync();

                var showtimes = await _context.Showtimes
                    .AsNoTracking()
                    .Include(s => s.Movie)
                    .Include(s => s.Room)
                    .Where(s =>
                        !s.IsDeleted &&
                        s.StartTime >= DateTime.Now)
                    .OrderBy(s => s.StartTime)
                    .ToListAsync();

                var movieData = movies.Select(m =>
                    $"""
                    ID: {m.Id}
                    Tên phim: {m.Title}
                    Thể loại: {m.Genre}
                    Thời lượng: {m.Duration} phút
                    Mô tả: {m.Description}
                    """
                );

                var showtimeData = showtimes.Select(s =>
                    $"""
                    Phim: {s.Movie?.Title ?? "Không xác định"}
                    Phòng: {s.Room?.Name ?? "Không xác định"}
                    Bắt đầu: {s.StartTime:dd/MM/yyyy HH:mm}
                    Kết thúc: {s.EndTime:dd/MM/yyyy HH:mm}
                    """
                );

                var systemData = $"""
                DỮ LIỆU PHIM HIỆN CÓ TRONG CINEMABOOKING:

                {string.Join("\n\n", movieData)}

                CÁC SUẤT CHIẾU SẮP DIỄN RA:

                {string.Join("\n\n", showtimeData)}
                """;

                var userPrompt = ChatbotPrompt.BuildUserPrompt(
                    systemData,
                    message);

                var result = await _geminiService.GenerateAsync(
                    ChatbotPrompt.SystemPrompt,
                    userPrompt);

                return Json(new
                {
                    success = true,
                    message = result
                });
            }
            catch (TimeoutException)
            {
                return Json(new
                {
                    success = false,
                    message =
                        "⚠️ AI phản hồi quá lâu. Vui lòng thử lại sau."
                });
            }
            catch (HttpRequestException ex)
                when (ex.StatusCode == HttpStatusCode.TooManyRequests)
            {
                return Json(new
                {
                    success = false,
                    message =
                        "⚠️ AI đang nhận quá nhiều yêu cầu. " +
                        "Vui lòng thử lại sau."
                });
            }
            catch (InvalidOperationException ex)
                when (ex.Message.Contains("quá dài"))
            {
                return Json(new
                {
                    success = false,
                    message =
                        "⚠️ Dữ liệu gửi đến AI quá lớn. " +
                        "Vui lòng thử lại với câu hỏi ngắn hơn."
                });
            }
            catch (Exception)
            {
                return Json(new
                {
                    success = false,
                    message =
                        "⚠️ Không thể xử lý yêu cầu với AI lúc này. " +
                        "Vui lòng thử lại sau."
                });
            }
        }
    }
}