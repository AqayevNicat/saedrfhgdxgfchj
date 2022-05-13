using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.DAL;
using WebApplication1.Models;
using WebApplication1.ViewModels.Contact;

namespace WebApplication1.Controllers
{
    public class ContactController : Controller
    {
        private readonly JuanDbContext _context;

        private readonly UserManager<AppUser> _userManager;

        public ContactController(JuanDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            Setting setting = _context.Settings.FirstOrDefault();
            Contact contact = _context.Contacts.FirstOrDefault();
            ContactVM contactVM = new ContactVM()
            {
                Setting = setting,
                Contact = contact
            };
            return View(contactVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ContactMessage(Contact contact)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("login", "account");
            }

            AppUser appUser = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name && !u.IsAdmin);


            if (string.IsNullOrWhiteSpace(contact.Name))
            {
                ModelState.AddModelError("Name", "Bosluq Olmamalidir");
                return View();
            }

            if (string.IsNullOrWhiteSpace(contact.Message))
            {
                ModelState.AddModelError("Message", "Bosluq Olmamalidir");
                return View();
            }
            if (string.IsNullOrWhiteSpace(contact.Email))
            {
                ModelState.AddModelError("Email", "Bosluq Olmamalidir");
                return View();
            }
            contact.MainEmail = appUser.Email;

            contact.CreatedAt = DateTime.UtcNow.AddHours(4);
            
            await _context.Contacts.AddAsync(contact);
            await _context.SaveChangesAsync();

            return RedirectToAction("index");
        }
    }
}
