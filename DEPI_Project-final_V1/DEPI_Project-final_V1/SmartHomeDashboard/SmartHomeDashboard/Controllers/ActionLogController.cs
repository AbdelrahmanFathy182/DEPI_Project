using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartHomeDashboard.Data;

namespace SmartHomeDashboard.Controllers
{
    public class ActionLogController : Controller
    {
        private readonly AppDbContext _context;

        public ActionLogController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string startDate, string endDate)
        {
            var query = _context.actions
            .Include(a => a.device) // <- This is key
            .AsQueryable();
            // Query to fetch the actions based on startDate and endDate if provided
            //var query = _context.actions.AsQueryable();

            if (!string.IsNullOrEmpty(startDate))
            {
                var parsedStartDate = DateTime.Parse(startDate);
                query = query.Where(a => a.Created_at >= parsedStartDate);
            }

            if (!string.IsNullOrEmpty(endDate))
            {
                var parsedEndDate = DateTime.Parse(endDate);
                query = query.Where(a => a.Created_at <= parsedEndDate);
            }

            var actions = await query.ToListAsync();

            // Return the view with the actions data
            return View(actions);
        }
    }
}
