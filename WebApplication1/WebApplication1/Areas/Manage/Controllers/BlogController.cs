using WebApplication1.DAL;
using WebApplication1.Extensions;
using WebApplication1.Helpers;
using WebApplication1.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class BlogController : Controller
    {
        private readonly JuanDbContext _context;
        private readonly IWebHostEnvironment _env;
        public BlogController(JuanDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public async Task<IActionResult> Index(bool? status ,int page = 1)
        {

            ViewBag.Status = status;
            IEnumerable < Blog> blogs = new List<Blog>();

            if (status != null)
            {
                blogs = await _context.Blogs
                    .Where(t => t.IsDeleted == status)
                    .ToListAsync();
            }
            else
            {
                blogs = await _context.Blogs
                   .ToListAsync();
            }
            ViewBag.PageIndex = page;
            ViewBag.PageCount = Math.Ceiling((double)blogs.Count() / 5);

            return View(blogs.Skip((page - 1) * 5).Take(5));
        }
        public async Task<IActionResult> Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Blog blog)
        {
            if (!ModelState.IsValid) return View();

            if (string.IsNullOrWhiteSpace(blog.Title))
            {
                ModelState.AddModelError("Title", "Blog title is required");
                return View();
            }
            if (string.IsNullOrWhiteSpace(blog.PublisherName))
            {
                ModelState.AddModelError("PublisherName", "Publisher name is required");
                return View();
            }
            if (string.IsNullOrWhiteSpace(blog.Description))
            {
                ModelState.AddModelError("Description", "Publisher name is required");
                return View();
            }

            if (blog.BlogImage != null)
            {
                if (!blog.BlogImage.CheckFileContentType("image/jpeg"))
                {
                    ModelState.AddModelError("BlogImage", "The image type does not match");
                    return View();
                }
                if (!blog.BlogImage.CheckFileSize(100))
                {
                    ModelState.AddModelError("BlogImage", "Image size can be up to 100kb");
                    return View();
                }
                blog.Image = blog.BlogImage.CreateFile(_env, "assets", "img", "blog");
            }

            blog.CreatedAt = DateTime.UtcNow.AddHours(4);

            await _context.Blogs.AddAsync(blog);
            await _context.SaveChangesAsync();

            return RedirectToAction("index");
        }
        public async Task<IActionResult> Update(int? id)
        {
            return View(await _context.Blogs.FirstOrDefaultAsync(b => b.Id == id));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, Blog blog, int page = 1)
        {
            if (!ModelState.IsValid) return View();

            if (id == null) return BadRequest();

            Blog dbblog = await _context.Blogs.FirstOrDefaultAsync(b => b.Id == id);

            if (dbblog == null) return NotFound();

            if (string.IsNullOrWhiteSpace(blog.Title))
            {
                ModelState.AddModelError("Title", "Blog title is required");
                return View(dbblog);
            }
            if (string.IsNullOrWhiteSpace(blog.PublisherName))
            {
                ModelState.AddModelError("PublisherName", "Publisher name is required");
                return View(dbblog);
            }
            if (string.IsNullOrWhiteSpace(blog.Description))
            {
                ModelState.AddModelError("Description", "Publisher name is required");
                return View(dbblog);
            }

            dbblog.Image = blog.Image;

            if (blog.BlogImage != null)
            {
                if (!blog.BlogImage.CheckFileContentType("image/jpeg"))
                {
                    ModelState.AddModelError("BlogImage", "The image type does not match");
                    return View();
                }
                if (!blog.BlogImage.CheckFileSize(100))
                {
                    ModelState.AddModelError("BlogImage", "Image size can be up to 100kb");
                    return View();

                }

                if (dbblog.Image != null)
                {
                    Helper.DeleteFile(_env, dbblog.Image, "assets", "img", "blog");
                }

                dbblog.Image = blog.BlogImage.CreateFile(_env, "assets", "img", "blog");
            }

            dbblog.Title = blog.Title;
            dbblog.PublisherName = blog.PublisherName;
            dbblog.Description = blog.Description;
            dbblog.UpdatedAt = DateTime.UtcNow.AddHours(4);

            await _context.SaveChangesAsync();

            return RedirectToAction("index", new { page = page });
        }
        public async Task<IActionResult> Detail(int? id, int page = 1)
        {
            if (id == null) return BadRequest();

            Blog blog = await _context.Blogs.FirstOrDefaultAsync(b => b.Id == id);

            if (blog == null) return NotFound();

            return View(blog);

        }

        public async Task<IActionResult> Delete(int? id, int page = 1)
        {
            if (id == null) return BadRequest();

            Blog dbblog = await _context.Blogs.FirstOrDefaultAsync(b => b.Id == id);

            if (dbblog == null) return NotFound();

            dbblog.IsDeleted = true;

            dbblog.DeletedAt = DateTime.UtcNow.AddHours(4);

            await _context.SaveChangesAsync();

            IEnumerable<Blog> blogs = await _context.Blogs
               .OrderByDescending(c => c.CreatedAt)
               .ToListAsync();

            ViewBag.PageIndex = page;
            ViewBag.PageCount = Math.Ceiling((double)blogs.Count() / 5);

            return PartialView("_BlogIndexPartial", blogs.Skip((page - 1) * 5).Take(5));
        }
        public async Task<IActionResult> Restore(int? id, int page = 1)
        {
            if (id == null) return BadRequest();

            Blog dbblog = await _context.Blogs.FirstOrDefaultAsync(b => b.Id == id);

            if (dbblog == null) return NotFound();

            dbblog.IsDeleted = false;

            dbblog.DeletedAt = DateTime.UtcNow.AddHours(4);

            await _context.SaveChangesAsync();

            IEnumerable<Blog> blogs = await _context.Blogs
               .OrderByDescending(c => c.CreatedAt)
               .ToListAsync();

            ViewBag.PageIndex = page;
            ViewBag.PageCount = Math.Ceiling((double)blogs.Count() / 5);

            return PartialView("_BlogIndexPartial", blogs.Skip((page - 1) * 5).Take(5));
        }

        


    }
}
