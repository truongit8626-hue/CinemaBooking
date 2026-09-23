using CinemaBooking.Data;
using CinemaBooking.Models;
using CinemaBooking.Prompts;
using CinemaBooking.Services.AI;
using CinemaBooking.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace CinemaBooking.Controllers
{
    [Authorize]
    public class MovieController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IGeminiService _geminiService;
        public MovieController(
    ApplicationDbContext context,
    UserManager<ApplicationUser> userManager,
    IGeminiService geminiService)
        {
            _context = context;
            _userManager = userManager;
            _geminiService = geminiService;
        }

        public IActionResult Index()
        {
            return View(_context.Movies.ToList());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Movie movie)
        {
            _context.Movies.Add(movie);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult GetMovie(int id)
        {
            var movie = _context.Movies.FirstOrDefault(m => m.Id == id);

            if (movie == null)
                return NotFound();

            return Json(movie);
        }
        public IActionResult Details(int id)
        {
            var movie = _context.Movies.FirstOrDefault(m => m.Id == id);

            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }
        private async Task<List<string>> GetUserBookingGenresAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return new List<string>();
            }

            var genres = await _context.Bookings
                .Where(b => b.UserId == user.Id)
                .SelectMany(b => b.BookingDetails)
                .Select(bd => bd.Booking.Showtime.Movie.Genre)
                .Where(g => !string.IsNullOrWhiteSpace(g))
                .ToListAsync();

            return genres;
        }
        private async Task<List<string>> GetUserPreferenceGenresAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return new List<string>();
            }

            return await _context.UserPreferences
                .Where(p => p.UserId == user.Id)
                .Select(p => p.Genre)
                .ToListAsync();
        }
        private async Task<string> GetMovieCatalogAsync()
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

            if (movies.Count == 0)
            {
                return "Hiện chưa có phim nào trong hệ thống.";
            }

            var catalog = movies.Select(m =>
                $"""
        ID: {m.Id}
        Tên phim: {m.Title}
        Thể loại: {m.Genre}
        Thời lượng: {m.Duration} phút
        Mô tả: {m.Description}
        """
            );

            return string.Join("\n\n", catalog);
        }
        [Authorize]
        public async Task<IActionResult> Recommendations()
        {

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Challenge();
            }

            // ================================
            // 1. Lấy sở thích người dùng
            // ================================
            var selectedGenres = await GetUserPreferenceGenresAsync();

            // ================================
            // 2. Lấy lịch sử đặt vé
            // ================================
            var bookingGenres = await GetUserBookingGenresAsync();

            // ================================
            // 3. Tạo nội dung sở thích gửi cho AI
            // ================================
            var preferenceLines = new List<string>();

            if (selectedGenres.Any())
            {
                preferenceLines.Add(
                    "Sở thích người dùng đã chọn khi đăng ký: "
                    + string.Join(", ", selectedGenres));
            }
            else
            {
                preferenceLines.Add(
                    "Người dùng chưa chọn sở thích.");
            }

            if (bookingGenres.Any())
            {
                var bookingSummary = bookingGenres
                    .GroupBy(g => g)
                    .OrderByDescending(g => g.Count())
                    .Select(g => $"{g.Key} ({g.Count()} lần)")
                    .ToList();

                preferenceLines.Add(
                    "Thể loại từ lịch sử đặt vé: "
                    + string.Join(", ", bookingSummary));
            }
            else
            {
                preferenceLines.Add(
                    "Người dùng chưa có lịch sử đặt vé.");
            }

            var preferredGenresText =
                string.Join("\n", preferenceLines);

            // ================================
            // 4. Lấy danh sách phim
            // ================================
            var movieCatalog = await GetMovieCatalogAsync();

            // ================================
            // 5. Tạo prompt cho Gemini
            // ================================
            var userPrompt = RecommendationPrompt.BuildUserPrompt(
                preferredGenresText,
                movieCatalog);

            // 6. Gọi Gemini
            string aiResult;

            try
            {
                aiResult = await _geminiService.GenerateAsync(
                    RecommendationPrompt.SystemPrompt,
                    userPrompt);

            }
            catch (TimeoutException)
            {
                TempData["AIError"] =
                    "⚠️ AI phản hồi quá lâu. Vui lòng thử lại sau.";

                return View(
                    "~/Views/Movie/Recommendations/Index.cshtml",
                    new RecommendationViewModel
                    {
                        PreferredGenres = preferredGenresText,
                        AIResult = "",
                        Recommendations = new List<AIRecommendationItem>(),
                        Movies = new List<Models.Movie>()
                    });
            }
            catch (HttpRequestException ex)
                when (ex.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
            {
                TempData["AIError"] =
                    "⚠️ AI đang nhận quá nhiều yêu cầu. Vui lòng thử lại sau.";

                return View(
                    "~/Views/Movie/Recommendations/Index.cshtml",
                    new RecommendationViewModel
                    {
                        PreferredGenres = preferredGenresText,
                        AIResult = "",
                        Recommendations = new List<AIRecommendationItem>(),
                        Movies = new List<Models.Movie>()
                    });
            }
            catch (InvalidOperationException ex)
                when (ex.Message.Contains("quá dài"))
            {
                TempData["AIError"] =
                    "⚠️ Dữ liệu gửi đến AI quá lớn. Vui lòng thử lại sau.";

                return View(
                    "~/Views/Movie/Recommendations/Index.cshtml",
                    new RecommendationViewModel
                    {
                        PreferredGenres = preferredGenresText,
                        AIResult = "",
                        Recommendations = new List<AIRecommendationItem>(),
                        Movies = new List<Models.Movie>()
                    });
            }
            catch (Exception)
            {
                TempData["AIError"] =
                    "⚠️ Không thể xử lý đề xuất phim bằng AI lúc này. " +
                    "Vui lòng thử lại sau.";

                return View(
                    "~/Views/Movie/Recommendations/Index.cshtml",
                    new RecommendationViewModel
                    {
                        PreferredGenres = preferredGenresText,
                        AIResult = "",
                        Recommendations = new List<AIRecommendationItem>(),
                        Movies = new List<Models.Movie>()
                    });
            }

            // 7. Đọc JSON AI trả về
            List<AIRecommendationItem> recommendations;

            try
            {
                recommendations =
                    System.Text.Json.JsonSerializer.Deserialize<
                        List<AIRecommendationItem>>(
                            aiResult,
                            new System.Text.Json.JsonSerializerOptions
                            {
                                PropertyNameCaseInsensitive = true
                            })
                        ?? throw new System.Text.Json.JsonException();
            }
            catch (System.Text.Json.JsonException)
            {
                TempData["AIError"] =
                    "⚠️ AI trả về dữ liệu không đúng định dạng. " +
                    "Vui lòng thử lại sau.";

                return View(
                    "~/Views/Movie/Recommendations/Index.cshtml",
                    new RecommendationViewModel
                    {
                        PreferredGenres = preferredGenresText,
                        AIResult = "",
                        Recommendations = new List<AIRecommendationItem>(),
                        Movies = new List<Models.Movie>()
                    });
            }
            // ================================
            // 8. Chỉ lấy MovieId tồn tại trong DB
            // ================================
            var movieIds = recommendations
                .Select(r => r.MovieId)
                .Distinct()
                .Take(5)
                .ToList();

            var movies = await _context.Movies
                .AsNoTracking()
                .Where(m => movieIds.Contains(m.Id))
                .ToListAsync();

            // ================================
            // 9. Lọc recommendation hợp lệ
            // ================================
            var validRecommendations = recommendations
                .Where(r => movies.Any(m => m.Id == r.MovieId))
                .ToList();

            // ================================
            // 10. ViewModel
            // ================================
            var model = new RecommendationViewModel
            {
                PreferredGenres = preferredGenresText,
                AIResult = aiResult,
                Recommendations = validRecommendations,
                Movies = movies
            };

            return View(
                "~/Views/Movie/Recommendations/Index.cshtml",
                model);
        }
    }

}
