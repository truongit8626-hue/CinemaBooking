using CinemaBooking.Data;
using CinemaBooking.Models;
using CinemaBooking.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CinemaBooking.Controllers
{
    [Authorize]
    public class PreferenceController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        // Các thể loại cho người dùng lựa chọn
        private readonly List<string> _genres = new()
        {
            "Action",
            "Adventure",
            "Animation",
            "Anime",
            "Comedy",
            "Drama",
            "Horror",
            "Romance",
            "Sci-Fi",
            "Mystery",
            "Fantasy",
            "Thriller"
        };

        public PreferenceController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /Preference/Setup
        [HttpGet]
        public async Task<IActionResult> Setup(string? returnUrl = null)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Challenge();
            }

            var selectedGenres = await _context.UserPreferences
                .Where(p => p.UserId == user.Id)
                .Select(p => p.Genre)
                .ToListAsync();

            var model = new UserPreferenceViewModel
            {
                AvailableGenres = _genres,
                SelectedGenres = selectedGenres
            };

            ViewBag.ReturnUrl = returnUrl;

            return View(model);
        }

        // POST: /Preference/Setup
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Setup(
            UserPreferenceViewModel model,
            string? returnUrl = null)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Challenge();
            }

            // Luôn gán lại danh sách thể loại để khi validation lỗi
            // trang vẫn hiển thị đầy đủ checkbox.
            model.AvailableGenres = _genres;

            // Lọc các thể loại hợp lệ
            var selectedGenres = model.SelectedGenres?
                .Where(g => _genres.Contains(g))
                .Distinct()
                .ToList()
                ?? new List<string>();

            if (selectedGenres.Count == 0)
            {
                ModelState.AddModelError(
                    nameof(model.SelectedGenres),
                    "Vui lòng chọn ít nhất 1 thể loại phim.");

                model.SelectedGenres = selectedGenres;

                return View(model);
            }

            // Xóa sở thích cũ
            var oldPreferences = await _context.UserPreferences
                .Where(p => p.UserId == user.Id)
                .ToListAsync();

            _context.UserPreferences.RemoveRange(oldPreferences);

            // Thêm sở thích mới
            foreach (var genre in selectedGenres)
            {
                _context.UserPreferences.Add(new UserPreference
                {
                    UserId = user.Id,
                    Genre = genre
                });
            }

            await _context.SaveChangesAsync();

            // Nếu có returnUrl hợp lệ thì quay lại đó
            if (!string.IsNullOrWhiteSpace(returnUrl)
                && Url.IsLocalUrl(returnUrl))
            {
                return LocalRedirect(returnUrl);
            }

            return RedirectToAction(
                "Recommendations",
                "Movie");
        }
    }
}