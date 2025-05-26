using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartHomeDashboard.Data;
using SmartHomeDashboard.Models;

namespace SmartHomeDashboard.Controllers
{
    public class RoomController : Controller
    {
        private readonly AppDbContext _db;

        public RoomController(AppDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            var accountId = HttpContext.Session.GetInt32("AccountId");
            var rooms = _db.rooms
                .Include(r => r.Devices)
                .Where(r => r.AccountId == accountId)
                .ToList();

            return View(rooms);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Room room)
        {
            // Manually set AccountId from session before validation
            var accountId = HttpContext.Session.GetInt32("AccountId");
            if (accountId == null)
            {
                TempData["Error"] = "You must be logged in to create a room";
                return RedirectToAction("Login", "Account");
            }
            room.AccountId = accountId.Value;

            // Set creation date
            room.Created_at = DateTime.Now;
            ModelState.Remove("Account");
            ModelState.Remove("Devices");
            if (ModelState.IsValid)
            {
                _db.rooms.Add(room);
                _db.SaveChanges();
                return RedirectToAction("Index", new { id = room.Id });
            }

            // If we get here, validation failed
            var errors = ModelState
                .Where(x => x.Value.Errors.Any())
                .SelectMany(kvp =>
                    kvp.Value.Errors.Select(e =>
                        $"{kvp.Key}: {e.ErrorMessage ?? "Invalid value"}"));

            TempData["Error"] = "Validation errors: " + string.Join("; ", errors);
            return View(room);  // Return to form with errors instead of redirecting
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Room room)
        {
            ModelState.Remove("Account");
            ModelState.Remove("Devices");
            if (!ModelState.IsValid)
            {
                return View("Index"); // Or wherever the modal is loaded from
            }

            Console.WriteLine($"Editing Room with ID: {room.Id}");
            var existing = await _db.rooms.FindAsync(room.Id);
            if (existing == null)
                return NotFound();

            existing.Name = room.Name;
            existing.Type = room.Type;

            await _db.SaveChangesAsync();

            return RedirectToAction("Index", new { id = room.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Room room)
        {
            ModelState.Remove("Account");
            ModelState.Remove("Devices");
            if (!ModelState.IsValid)
            {
                return View("Index");
            }

            // Verify room exists and get device IDs in one query
            var deviceIds = await _db.devices
                .Where(d => d.RoomId == room.Id)
                .Select(d => d.Id)
                .ToListAsync();

            if (!deviceIds.Any()) // If no devices, just delete the room
            {
                _db.rooms.Remove(room);
            }
            else
            {
                // Delete all related actions in one SQL command
                await _db.actions
                    .Where(al => deviceIds.Contains(al.DeviceId))
                    .ExecuteDeleteAsync();

                // Delete the room
                await _db.rooms.Where(r => r.Id == room.Id).ExecuteDeleteAsync();
            }

            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }


    }
}

