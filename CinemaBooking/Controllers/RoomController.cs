using CinemaBooking.Data;
using CinemaBooking.Models;
using Microsoft.AspNetCore.Mvc;

namespace CinemaBooking.Controllers
{
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
        public IActionResult Create(Room room)
        {
            _context.Rooms.Add(room);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
