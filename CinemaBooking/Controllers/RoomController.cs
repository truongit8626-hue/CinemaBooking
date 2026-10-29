using CinemaBooking.Data;
using CinemaBooking.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CinemaBooking.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RoomController : Controller
    {
        
        private readonly ApplicationDbContext _context;

        public RoomController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Room room)
        {
            if (!ModelState.IsValid)
            {
                return View(room);
            }

            _context.Rooms.Add(room);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
