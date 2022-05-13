using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.DAL;
using WebApplication1.Models;

namespace WebApplication1.Areas.Manage.Controllers
{
    [Area("manage")]
    public class ContactController : Controller
    {
        private readonly JuanDbContext _context;
        public ContactController(JuanDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(int page = 1)
        {
            List<Contact> contacts = await _context.Contacts.ToListAsync();
            ViewBag.PageIndex = page;
            ViewBag.PageCount = Math.Ceiling((double)contacts.Count() / 5);
            return View(contacts.Skip((page - 1) * 5).Take(5));
        }

        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null) return BadRequest();

            Contact contact = await _context.Contacts
                .FirstOrDefaultAsync(r => r.Id == id);
            if (contact == null) return NotFound();

            return View(contact);
        }
    }
}
