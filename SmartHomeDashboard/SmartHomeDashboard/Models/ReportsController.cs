using Microsoft.AspNetCore.Mvc;

namespace SmartHomeDashboard.Models
{
    public class ReportsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
