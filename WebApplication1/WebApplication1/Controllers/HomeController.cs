using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.DAL;
using WebApplication1.Models;
using WebApplication1.ViewModels.Basket;
using WebApplication1.ViewModels.Home;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly JuanDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        public HomeController(JuanDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            List<HomeSlider> homeSliders = _context.HomeSliders.Where(s => !s.IsDeleted).ToList();
            List<Product> products = _context.Products
                .Include(p=>p.ProductColorSizes).ThenInclude(p=>p.Color)
                .Include(p=>p.ProductColorSizes).ThenInclude(p=>p.Size)
                .Where(s => !s.IsDeleted).ToList();
            HomeVM homeVM = new HomeVM()
            {
                HomeSliders = homeSliders,
                Products = products,
                Blogs = _context.Blogs.ToList()
            };
            return View(homeVM);
        }
        public async Task<IActionResult> AddToBasket(int? id, int count = 1, int colorid = 1, int sizeid = 1)
        {
            if (id == null) return BadRequest();
            Product dBproduct = await _context.Products.FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
            if (dBproduct == null) return NotFound();

            //List<Product> products = null;
            List<BasketVM> basketVMs = null;

            string cookie = HttpContext.Request.Cookies["basket"];

            if(cookie != "" && cookie != null)
            {
                basketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(cookie);
                if (basketVMs.Any(b => b.ProductId == id && b.Color == colorid && b.Size == sizeid))
                {
                    basketVMs.Find(b => b.ProductId == id && b.Color == colorid && b.Size == sizeid).Count += count;
                }
                else
                {
                    basketVMs.Add(new BasketVM
                    {
                        ProductId = (int)id,
                        Count = count,
                        Color = colorid,
                        Size = sizeid,
                    });
                }
            }
            else
            {
                basketVMs = new List<BasketVM>();

                basketVMs.Add(new BasketVM()
                {
                    ProductId = (int)id,
                    Count = count,
                    Color = colorid,
                    Size = sizeid
                });
            }



            foreach (BasketVM basketVM in basketVMs)
            {
                Product dbProduct = await _context.Products
                    .FirstOrDefaultAsync(p => p.Id == basketVM.ProductId);
                basketVM.Image = dbProduct.Image;
                basketVM.Price = dbProduct.DiscountPrice > 0 ? dbProduct.DiscountPrice : dbProduct.Price;
                basketVM.Name = dbProduct.Name;
                basketVM.ExTax = dbProduct.ExTax;
            }
            HttpContext.Response.Cookies.Append("basket", JsonConvert.SerializeObject(basketVMs));




            if (User.Identity.IsAuthenticated)
            {
                AppUser appUser = await _userManager.Users.FirstOrDefaultAsync(u => u.FullName.ToLower() == User.Identity.Name.ToLower() && !u.IsAdmin);
                List<Basket> existedBasket = await _context.Baskets.Where(b => b.AppUserId == appUser.Id).ToListAsync();
                List<Basket> baskets = new List<Basket>();
                foreach (BasketVM basketVM in basketVMs)
                {
                    if (existedBasket.Any(b => b.ProductId == basketVM.ProductId && b.SizeId == basketVM.Size && b.ColorId == basketVM.Color))
                    {
                        existedBasket.Find(b => b.ProductId == basketVM.ProductId && b.SizeId == basketVM.Size && b.ColorId == basketVM.Color).Count = basketVM.Count;
                    }
                    else
                    {
                        Basket basket = new Basket
                        {
                            AppUserId = appUser.Id,
                            ProductId = basketVM.ProductId,
                            Count = basketVM.Count,
                            ColorId = basketVM.Color,
                            SizeId = basketVM.Size,
                            CreatedAt = DateTime.UtcNow.AddHours(4)
                        };
                        //_context.Baskets.RemoveRange(existedBasket);
                        baskets.Add(basket);
                    }


                }

                if (baskets.Count > 0)
                {
                    await _context.Baskets.AddRangeAsync(baskets);
                }
                await _context.SaveChangesAsync();
            }



            return PartialView("_BasketPartial", basketVMs);
        }

        public async Task<IActionResult> DetailModal(int? id,int? color = 1,int? size=1,int count = 0)
        {
            ViewBag.Colors = await _context.Colors.ToListAsync();
            ViewBag.Sizes = await _context.Sizes.ToListAsync();

            ViewBag.Colorid = color;
            ViewBag.Sizeid = size;
            ViewBag.Count = count;

            if (id == null) return BadRequest();

            Product product = await _context.Products
                .Include(p=>p.ProductColorSizes).ThenInclude(p=>p.Color)
                .Include(p=>p.ProductColorSizes).ThenInclude(p=>p.Size)
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);

            if (product == null) return NotFound();

            return PartialView("_ProductModalPartial", product);
        }

        public async Task<IActionResult> SearchInput(string key)
        {
            List<Product> products = new List<Product>();
            if (key != null)
            {
                    products = await _context.Products
                    .Where(p=>p.Name.Contains(key)
                    || p.Description.Contains(key)
                    || p.Price.ToString().Contains(key)
                    || p.DiscountPrice.ToString().Contains(key)
                    || p.Category.Name.Contains(key)
                    || p.ProductTags.Any(p=>p.Tag.Name.Contains(key))
                    || p.ProductColorSizes.Any(p=>p.Color.Name.Contains(key))
                    || p.ProductColorSizes.Any(p=>p.Size.Name.Contains(key))
                    )
                    .ToListAsync();
            }
            return PartialView("_ProductListPartial", products);
        }
        public async Task<int> Count()
        {
            string cookieBasket = HttpContext.Request.Cookies["basket"];

            List<BasketVM> basketVMs = null;

            if (!string.IsNullOrWhiteSpace(cookieBasket))
            {
                basketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(cookieBasket);
            }
            else
            {
                basketVMs = new List<BasketVM>();
            }

            foreach (BasketVM basketVM in basketVMs)
            {
                Product dbProduct = await _context.Products.FirstOrDefaultAsync(p => p.Id == basketVM.ProductId);
                basketVM.Image = dbProduct.Image;
                basketVM.Price = dbProduct.DiscountPrice > 0 ? dbProduct.DiscountPrice : dbProduct.Price;
                basketVM.Name = dbProduct.Name;
                basketVM.ExTax = dbProduct.ExTax;
            }

            return basketVMs.Count();
        }
    }
}
