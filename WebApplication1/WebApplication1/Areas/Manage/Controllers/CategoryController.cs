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
    public class CategoryController : Controller
    {
        private readonly JuanDbContext _context;

        public CategoryController(JuanDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(int page = 1)
        {
            IEnumerable<Category> categories = await _context.Categories
                .ToListAsync();
            ViewBag.PageIndex = page;
            ViewBag.PageCount = Math.Ceiling((double)categories.Count() / 5);

            return View(categories.Skip((page - 1) * 5).Take(5));
        }
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null) return BadRequest();

            Category dbCategory = await _context.Categories.FirstOrDefaultAsync(t => t.Id == id);

            if (dbCategory == null) return NotFound();
            return View(dbCategory);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, Category Category)
        {
            if (!ModelState.IsValid) return View();

            if (id == null) return BadRequest();

            if (id != Category.Id) return NotFound();

            Category dbCategory = await _context.Categories.FirstOrDefaultAsync(t => t.Id == id);

            if (dbCategory == null) return NotFound();
            if (string.IsNullOrWhiteSpace(Category.Name))
            {
                ModelState.AddModelError("Name", "Bosluq Olmamalidir");
                return View();
            }

            if (Category.Name.CheckString())
            {
                ModelState.AddModelError("Name", "Yalniz Herf Ola Biler");
                return View();
            }

            if (await _context.Categories.AnyAsync(t => t.Name.ToLower() == Category.Name.ToLower()))
            {
                ModelState.AddModelError("Name", "Alreade Exists");
                return View();
            }
            dbCategory.Name = Category.Name;
            dbCategory.UpdatedAt = DateTime.UtcNow.AddHours(4);

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category Category)
        {
            if (!ModelState.IsValid) return View();

            if (string.IsNullOrWhiteSpace(Category.Name))
            {
                ModelState.AddModelError("Name", "Bosluq Olmamalidir");
                return View();
            }

            if (Category.Name.CheckString())
            {
                ModelState.AddModelError("Name", "Yalniz Herf Ola Biler");
                return View();
            }

            if (await _context.Categories.AnyAsync(t => t.Name.ToLower() == Category.Name.ToLower()))
            {
                ModelState.AddModelError("Name", "Alreade Exists");
                return View();
            }

            Category.CreatedAt = DateTime.UtcNow.AddHours(4);

            await _context.Categories.AddAsync(Category);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return BadRequest();
            Category Category = await _context.Categories.FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);
            if (Category == null) return NotFound();
            Category.IsDeleted = true;
            Category.DeletedAt = DateTime.UtcNow.AddHours(4);
            await _context.SaveChangesAsync();
            return RedirectToAction("index");
        }
        public async Task<IActionResult> Restore(int? id)
        {
            if (id == null) return BadRequest();
            Category Category = await _context.Categories.FirstOrDefaultAsync(t => t.Id == id && t.IsDeleted);
            if (Category == null) return NotFound();
            Category.IsDeleted = false;
            await _context.SaveChangesAsync();
            return RedirectToAction("index");
        }
    }
}
