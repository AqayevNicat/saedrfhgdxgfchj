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

namespace WebApplication1.Controllers
{
    public class BasketController : Controller
    {
        private readonly JuanDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public BasketController(JuanDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            ViewBag.Colors = await _context.Colors.ToListAsync();
            ViewBag.Sizes = await _context.Sizes.ToListAsync();

            string cookie = HttpContext.Request.Cookies["basket"];

            List<BasketVM> basketVMs = null;

            if (cookie != null)
            {
                basketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(cookie);
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
            }
            return View(basketVMs);
        }

        public async Task<IActionResult> GetBasket()
        {
            string cookieBasket = HttpContext.Request.Cookies["basket"];

            List<BasketVM> basketVMs = null;

            if (cookieBasket != null)
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
            }

            return PartialView("_BasketPartial", basketVMs);
        }

        public async Task<IActionResult> Update(int? id, int? count,int color = 1,int size = 1)
        {
            if (id == null) return BadRequest();

            Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return NotFound();

            string cookieBasket = HttpContext.Request.Cookies["basket"];

            List<BasketVM> basketVMs = null;

            if (cookieBasket != null)
            {
                basketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(cookieBasket);

                if (!basketVMs.Any(b => b.ProductId == id && b.Color == (int)color && b.Size == (int)size))
                {
                    return NotFound();
                }

                basketVMs.Find(b => b.ProductId == id && b.Color == (int)color && b.Size == (int)size).Count = (int)count;
            }
            else
            {
                return BadRequest();
            }


            foreach (BasketVM basketVM in basketVMs)
            {
                Product dbProduct = await _context.Products.FirstOrDefaultAsync(p => p.Id == basketVM.ProductId);
                basketVM.Image = dbProduct.Image;
                basketVM.Price = dbProduct.DiscountPrice > 0 ? dbProduct.DiscountPrice : dbProduct.Price;
                basketVM.Name = dbProduct.Name;
                basketVM.ExTax = dbProduct.ExTax;
            }
            cookieBasket = JsonConvert.SerializeObject(basketVMs);
            HttpContext.Response.Cookies.Append("basket", cookieBasket);

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
                            CreatedAt = DateTime.UtcNow.AddHours(4)
                        };
                        _context.Baskets.RemoveRange(existedBasket);
                        baskets.Add(basket);
                    }


                }

                if (baskets.Count > 0)
                {
                    await _context.Baskets.AddRangeAsync(baskets);
                }
                await _context.SaveChangesAsync();
            }

            return PartialView("_BasketIndexPartial", basketVMs);
        }

        public async Task<IActionResult> GetCard()
        {
            string cookieBasket = HttpContext.Request.Cookies["basket"];

            List<BasketVM> basketVMs = null;

            if (cookieBasket != null)
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
            }

            return PartialView("_BasketIndexPartial", basketVMs);
        }

        public async Task<IActionResult> DeleteCard(int? id, int color = 1, int size = 1)
        {
            if (id == null) return BadRequest();

            Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return NotFound();

            string cookieBasket = HttpContext.Request.Cookies["basket"];

            List<BasketVM> basketVMs = null;

            if (cookieBasket != null)
            {
                basketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(cookieBasket);

                BasketVM basketVM = basketVMs.FirstOrDefault(b => b.ProductId == id && b.Color == color && b.Size == size);

                if (basketVM == null)
                {
                    return NotFound();
                }

                basketVMs.Remove(basketVM);
            }
            else
            {
                return BadRequest();
            }
            
            cookieBasket = JsonConvert.SerializeObject(basketVMs);
            HttpContext.Response.Cookies.Append("basket", cookieBasket);

            foreach (BasketVM basketVM in basketVMs)
            {
                Product dbProduct = await _context.Products.FirstOrDefaultAsync(p => p.Id == basketVM.ProductId);
                basketVM.Image = dbProduct.Image;
                basketVM.Price = dbProduct.DiscountPrice > 0 ? dbProduct.DiscountPrice : dbProduct.Price;
                basketVM.Name = dbProduct.Name;
                basketVM.ExTax = dbProduct.ExTax;
            }


            //if (User.Identity.IsAuthenticated)
            //{
            //    AppUser appUser = await _userManager.Users.FirstOrDefaultAsync(u => u.FullName.ToLower() == User.Identity.Name.ToLower() && !u.IsAdmin);
            //    List<Basket> existedBasket = await _context.Baskets.Where(b => b.AppUserId == appUser.Id).ToListAsync();
            //    List<Basket> newBasket = new List<Basket>() ;
            //    Basket basket = await _context.Baskets.FirstOrDefaultAsync(p =>p.AppUserId == appUser.Id && p.ProductId == id && p.ColorId == color && p.SizeId == size);
            //    _context.RemoveRange(existedBasket);
            //    existedBasket.Remove(basket);
            //    foreach (Basket basket1 in (List<Basket>)existedBasket)
            //    {
            //        if(basket1 != null)
            //        {
            //            newBasket.Add(new Basket()
            //            {
            //                AppUserId = appUser.Id,
            //                ProductId = basket1.ProductId,
            //                Count = basket1.Count,
            //                ColorId = color,
            //                SizeId = size,
            //                CreatedAt = DateTime.UtcNow.AddHours(4)
            //            });
            //        }
            //    }
            //    await _context.Baskets.AddRangeAsync(existedBasket);
            //    await _context.SaveChangesAsync();
            //}



            return PartialView("_BasketIndexPartial", basketVMs);
        }

        public async Task<IActionResult> DeleteBasket(int? id,int color = 1,int size = 1)
        {
            if (id == null) return BadRequest();

            Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return NotFound();

            string cookieBasket = HttpContext.Request.Cookies["basket"];

            List<BasketVM> basketVMs = null;

            if (cookieBasket != null)
            {
                basketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(cookieBasket);

                BasketVM basketVM = basketVMs.FirstOrDefault(b => b.ProductId == id && b.Color == color && b.Size == size);

                if (basketVM == null)
                {
                    return NotFound();
                }

                basketVMs.Remove(basketVM);
            }
            else
            {
                return BadRequest();
            }

            cookieBasket = JsonConvert.SerializeObject(basketVMs);
            HttpContext.Response.Cookies.Append("basket", cookieBasket);

            foreach (BasketVM basketVM in basketVMs)
            {
                Product dbProduct = await _context.Products.FirstOrDefaultAsync(p => p.Id == basketVM.ProductId);
                basketVM.Image = dbProduct.Image;
                basketVM.Price = dbProduct.DiscountPrice > 0 ? dbProduct.DiscountPrice : dbProduct.Price;
                basketVM.Name = dbProduct.Name;
            }

            return PartialView("_BasketPartial", basketVMs);
        }
    }
}
