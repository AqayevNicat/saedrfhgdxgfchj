using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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
    [Area("Manage")]
    public class UserController : Controller
    {
        private readonly JuanDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public UserController(JuanDbContext context,UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<ActionResult> Index(int page = 1)
        {
            List<AppUser> appUsers = await _context.AppUsers.ToListAsync();
            ViewBag.PageIndex = page;
            ViewBag.PageCount = Math.Ceiling((double)appUsers.Count() / 5);

            return View(appUsers.Skip((page - 1) * 5).Take(5));
        }
        public async Task<ActionResult> Detail(string email)
        {
            if (email == null) return BadRequest();
            AppUser appUser = await _context.AppUsers
                .Include(p=>p.Orders).ThenInclude(p=>p.OrderItems).ThenInclude(p=>p.Product)
                .FirstOrDefaultAsync(a=>a.Email.ToLower() == email);
            if (appUser == null) return NotFound();
            return View(appUser);
        }

    }
}
