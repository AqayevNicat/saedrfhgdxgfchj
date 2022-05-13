using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.DAL;
using WebApplication1.Extensions;
using WebApplication1.Models;

namespace WebApplication1.Areas.Manage.Controllers
{
    [Area("manage")]
    public class ColorController : Controller
    {
        private readonly JuanDbContext _context;

        public ColorController(JuanDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(int page = 1)
        {
            IEnumerable<Color> colors = await _context.Colors
              .ToListAsync();
            ViewBag.PageIndex = page;
            ViewBag.PageCount = Math.Ceiling((double)colors.Count() / 5);

            return View(colors.Skip((page - 1) * 5).Take(5));

            return View(colors);
        }
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null) return BadRequest();

            Color dbColor = await _context.Colors.FirstOrDefaultAsync(t => t.Id == id);

            if (dbColor == null) return NotFound();
            return View(dbColor);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, Color Color)
        {
            if (!ModelState.IsValid) return View();

            if (id == null) return BadRequest();

            if (id != Color.Id) return NotFound();

            Color dbColor = await _context.Colors.FirstOrDefaultAsync(t => t.Id == id);

            if (dbColor == null) return NotFound();

            if (string.IsNullOrWhiteSpace(Color.Name))
            {
                ModelState.AddModelError("Name", "Bosluq Olmamalidir");
                return View();
            }

            if (Color.Name.CheckString())
            {
                ModelState.AddModelError("Name", "Yalniz Herf Ola Biler");
                return View();
            }

            if (await _context.Categories.AnyAsync(t => t.Name.ToLower() == Color.Name.ToLower()))
            {
                ModelState.AddModelError("Name", "Alreade Exists");
                return View();
            }
            dbColor.Name = Color.Name;
            dbColor.UpdatedAt = DateTime.UtcNow.AddHours(4);

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Color Color)
        {
            if (!ModelState.IsValid) return View();

            if (string.IsNullOrWhiteSpace(Color.Name))
            {
                ModelState.AddModelError("Name", "Bosluq Olmamalidir");
                return View();
            }

            if (Color.Name.CheckString())
            {
                ModelState.AddModelError("Name", "Yalniz Herf Ola Biler");
                return View();
            }

            if (await _context.Categories.AnyAsync(t => t.Name.ToLower() == Color.Name.ToLower()))
            {
                ModelState.AddModelError("Name", "Alreade Exists");
                return View();
            }
            Color.CreatedAt = DateTime.UtcNow.AddHours(4);

            await _context.Colors.AddAsync(Color);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Delete(int? id, bool? status,int page =1)
        {
            if (id == null) return BadRequest();
            Color Color = await _context.Colors.FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);
            if (Color == null) return NotFound();
            Color.IsDeleted = true;
            Color.DeletedAt = DateTime.UtcNow.AddHours(4);
            await _context.SaveChangesAsync();
            return RedirectToAction("index",new {status,page });
        }
        public async Task<IActionResult> Restore(int? id, bool? status, int page = 1)
        {
            if (id == null) return BadRequest();
            Color Color = await _context.Colors.FirstOrDefaultAsync(t => t.Id == id && t.IsDeleted);
            if (Color == null) return NotFound();
            Color.IsDeleted = false;
            await _context.SaveChangesAsync();
            return RedirectToAction("index", new { status, page });
        }
    }
}
