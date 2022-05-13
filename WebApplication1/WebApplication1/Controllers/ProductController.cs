using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.DAL;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class ProductController : Controller
    {
        private readonly JuanDbContext _context;
        public ProductController(JuanDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(int? id)
        {
            if (id == null) return BadRequest();
            Product product = await _context.Products
                .Include(p=>p.ProductColorSizes).ThenInclude(p=>p.Color)
                .Include(p=>p.ProductColorSizes).ThenInclude(p=>p.Size)
                .Include(p=>p.ProductImages)
                .FirstOrDefaultAsync(product => product.Id == id);
            if (product == null) return NotFound();

            return View(product);
        }
    }
}
