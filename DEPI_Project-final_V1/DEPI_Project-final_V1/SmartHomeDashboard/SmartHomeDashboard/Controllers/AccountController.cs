using System.Runtime.CompilerServices;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using SmartHomeDashboard.Data;
using SmartHomeDashboard.Models;

namespace SmartHomeDashboard.Controllers
{
    public class AccountController : Controller
    {

        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var accountId = HttpContext.Session.GetInt32("AccountId");

            if (accountId == null)
            {
                // Session expired or user not logged in
                return RedirectToAction("Login", "Account");
            }
            // Fetch the account from your database
            var user = await _context.Accounts.FirstOrDefaultAsync(u => u.Id == accountId);

            if (user == null)
            {
                // Account not found (e.g., deleted)
                return RedirectToAction("Login", "Account");
            }
            return View(user);
            // Extract email from the logged-in user's claims
        }


        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (_context.Accounts.Any(a => a.Email == model.Email))
                {
                    ModelState.AddModelError("Email", "Email is already in use.");
                    return View(model);
                }

                var newUser = new Account
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    HashPassword = HashPassword(model.Password),
                    Created_at = DateTime.Now
                };

                _context.Accounts.Add(newUser);
                _context.SaveChanges();

                HttpContext.Session.SetInt32("AccountId", newUser.Id);
                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var hashed = HashPassword(model.Password);
                var user = _context.Accounts
                    .FirstOrDefault(a => a.Email == model.Email && a.HashPassword == hashed);

                if (user != null)
                {
                    HttpContext.Session.SetInt32("AccountId", user.Id);
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("Password", "Invalid email or password.");
                
            }
             
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditEmail(Account model)
        {
            ModelState.Remove("HashPassword");
            ModelState.Remove("UserName");
            ModelState.Remove("Rooms");

            if (!ModelState.IsValid)
            {
                return View("_EditEmailModal", model); // or however you're rendering the view
            }

            var account = await _context.Accounts.FindAsync(model.Id);
            if (account == null)
            {
                return NotFound();
            }

            account.Email = model.Email;
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", new { id = model.Id});
        }

        public async Task<IActionResult> EditName(Account model)
        {
            ModelState.Remove("HashPassword");
            ModelState.Remove("Email");
            ModelState.Remove("Rooms");

            if (!ModelState.IsValid)
            {
                return View("_EditNameModal", model); // or however you're rendering the view
            }

            var account = await _context.Accounts.FindAsync(model.Id);
            if (account == null)
            {
                return NotFound();
            }

            account.UserName = model.UserName;
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", new { id = model.Id });
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> EditPassword(Account model, string oldPassword, string newPassword)
        //{
        //    // Skip validation for unrelated fields
        //    ModelState.Remove("Email");
        //    ModelState.Remove("UserName");
        //    ModelState.Remove("Rooms");
        //    ModelState.Remove("HashPassword");

        //    if (!ModelState.IsValid)
        //    {
        //        return View("Index", model);
        //    }

        //    var account = await _context.Accounts.FindAsync(model.Id);
        //    if (account == null)
        //    {
        //        return NotFound();
        //    }

        //    if (HashPassword(oldPassword) != account.HashPassword)
        //    {
        //        ModelState.AddModelError("HashPassword", "Current Password is incorrect");
        //        return View("_EditPasswordModal", model);
        //    }

        //    account.HashPassword = HashPassword(newPassword);
        //    await _context.SaveChangesAsync();

        //    return RedirectToAction("Index", new { id = model.Id });
        //}





        // 👇 PLACE THIS AT THE END, INSIDE THE CLASS
        private string HashPassword(string password)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var bytes = System.Text.Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }
    }
}
