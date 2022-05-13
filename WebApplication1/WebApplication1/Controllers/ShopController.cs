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
    public class ShopController : Controller
    {
        private readonly JuanDbContext _context;
        public ShopController(JuanDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(string sortby, int page = 1)
        {
            List<Product> products = new List<Product>();
            ViewBag.Categories = await _context.Categories.ToListAsync();
            ViewBag.Colors = await _context.Colors.ToListAsync();
            ViewBag.Sizes = await _context.Sizes.ToListAsync();
            switch (sortby)
            {
                case "AZ":
                    products = await _context.Products
                        .Include(p => p.ProductColorSizes).ThenInclude(p => p.Color)
                        .Include(p => p.ProductColorSizes).ThenInclude(p => p.Size)
                        .OrderBy(p => p.Name).ToListAsync();
                    break;
                case "ZA":
                    products = await _context.Products
                        .Include(p => p.ProductColorSizes).ThenInclude(p => p.Color)
                        .Include(p => p.ProductColorSizes).ThenInclude(p => p.Size)
                        .OrderBy(p => p.Name).OrderByDescending(p => p.Name).ToListAsync();
                    break;
                case "HL":
                    products = await _context.Products
                        .Include(p => p.ProductColorSizes).ThenInclude(p => p.Color)
                        .Include(p => p.ProductColorSizes).ThenInclude(p => p.Size)
                        .OrderBy(p => p.Name).OrderBy(p => p.Price).ToListAsync();
                    break;
                case "LH":
                    products = await _context.Products
                        .Include(p => p.ProductColorSizes).ThenInclude(p => p.Color)
                        .Include(p => p.ProductColorSizes).ThenInclude(p => p.Size)
                        .OrderBy(p => p.Name).OrderByDescending(p => p.Price).ToListAsync();
                    break;
                case "PAZ":
                    products = await _context.Products
                        .Include(p => p.ProductColorSizes).ThenInclude(p => p.Color)
                        .Include(p => p.ProductColorSizes).ThenInclude(p => p.Size)
                        .OrderBy(p => p.Name).OrderByDescending(p => p.Price).ToListAsync();
                    break;
                case "PZA":
                    products = await _context.Products
                        .Include(p => p.ProductColorSizes).ThenInclude(p => p.Color)
                        .Include(p => p.ProductColorSizes).ThenInclude(p => p.Size)
                        .OrderByDescending(p => p.Name).OrderByDescending(p => p.Price).ToListAsync();
                    break;
                default:
                    products = await _context.Products
                        .Include(p => p.ProductColorSizes).ThenInclude(p => p.Color)
                        .Include(p => p.ProductColorSizes).ThenInclude(p => p.Size)
                        .OrderBy(p => p.Name).ToListAsync();
                    break;
            }
            ViewBag.PageIndex = page;
            ViewBag.PageCount = Math.Ceiling((double)products.Count() / 5);
            return View(products.Skip((page - 1) * 5).Take(4));
        }
    }
}
