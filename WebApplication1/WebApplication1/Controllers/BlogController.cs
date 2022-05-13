using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.DAL;
using WebApplication1.Models;
using WebApplication1.ViewModels.Blog;

namespace WebApplication1.Controllers
{
    public class BlogController : Controller
    {
        private readonly JuanDbContext _context;

        private readonly UserManager<AppUser> _userManager;

        public BlogController(JuanDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index(int page = 1)
        {
            ViewBag.Categories = await _context.Categories.ToListAsync();
            ViewBag.Tags = await _context.Tags.ToListAsync();

            IEnumerable<Blog> blogs = await _context.Blogs
               .OrderByDescending(c => c.CreatedAt)
               .ToListAsync();

            ViewBag.PageIndex = page;
            ViewBag.PageCount = Math.Ceiling((double)blogs.Count() / 5);
            return View(blogs.Skip((page - 1) * 5).Take(5));
        }
        public async Task<IActionResult> Detail(int? bid)
        {
            ViewBag.Categories = await _context.Categories.ToListAsync();
            ViewBag.Tags = await _context.Tags.ToListAsync();

            ViewBag.Blogs = await _context.Blogs.OrderByDescending(b=>b.CreatedAt).Take(4).ToListAsync();

            if (bid == null) return BadRequest();
            Blog blog = await _context.Blogs
                .Include(b=>b.Reviews)
                .FirstOrDefaultAsync(p => p.Id == (int)bid);
            if (blog == null) return NotFound();


            return View(blog);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddReview(Review review,int? bid)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("login", "account");
            }
            if (bid == null) return View();

            AppUser appUser = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name && !u.IsAdmin);
            review.Email = appUser.Email;
            review.Name = appUser.UserName;
            

            if (review.Message == null || review.Email == null || review.Name == null) return RedirectToAction("detail", new { bid });

            if (review.Star == null || review.Star < 0 || review.Star > 5)
            {
                review.Star = 1;
            }
            review.BlogId = (int)bid;
            review.CreatedAt = DateTime.UtcNow.AddHours(4);
            await _context.Reviews.AddAsync(review);
            await _context.SaveChangesAsync();
            return RedirectToAction("detail",new { bid });
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return BadRequest();
            Review dbReview = await _context.Reviews
                .Include(r=>r.Blog)
                .FirstOrDefaultAsync(r => r.Id == id);
            if (dbReview == null) return NotFound();

            return View(dbReview);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id,Review review)
        {
            if (id == null) return BadRequest();
            Review dbReview = await _context.Reviews.FirstOrDefaultAsync(r => r.Id == id);
            if (dbReview == null) return NotFound();

            if (review.Id != id) return BadRequest();

            Blog blog = await _context.Blogs.FirstOrDefaultAsync(b => b.Reviews.FirstOrDefault(r => r.Id == id).Id == id);

            dbReview.Message = review.Message;
            dbReview.UpdatedAt = DateTime.UtcNow.AddHours(4);
            int bid = blog.Id;
            await _context.SaveChangesAsync();
            return RedirectToAction("detail", new { bid });
        }
    }
}
