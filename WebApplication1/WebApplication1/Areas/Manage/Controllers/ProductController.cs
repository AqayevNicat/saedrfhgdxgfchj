using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.DAL;
using WebApplication1.Extensions;
using WebApplication1.Helpers;
using WebApplication1.Models;

namespace WebApplication1.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class ProductController : Controller
    {
        private readonly JuanDbContext _context;
        private readonly IWebHostEnvironment _env;
        public ProductController(JuanDbContext context,IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public async Task<IActionResult> Index(bool? status, int page = 1)
        {
            ViewBag.Status = status;
            IEnumerable<Product> products = new List<Product>();

            if(status != null)
            {
                products = await _context.Products
                    .Include(p => p.Category)
                    .Include(p => p.ProductColorSizes).ThenInclude(p => p.Color)
                    .Include(p => p.ProductColorSizes).ThenInclude(p => p.Size)
                    .Include(p => p.ProductTags).ThenInclude(pt => pt.Tag)
                    .Include(p => p.ProductImages)
                    .Where(t => t.IsDeleted == status)
                    .ToListAsync();
            }
            else
            {
                 products = await _context.Products
                    .Include(p => p.Category)
                    .Include(p => p.ProductColorSizes).ThenInclude(p => p.Color)
                    .Include(p => p.ProductColorSizes).ThenInclude(p => p.Size)
                    .Include(p => p.ProductTags).ThenInclude(pt => pt.Tag)
                    .Include(p => p.ProductImages)
                    .ToListAsync();
            }

            ViewBag.PageIndex = page;
            ViewBag.PageCount = Math.Ceiling((double)products.Count() / 5);
            return View(products.Skip((page - 1) * 5).Take(5));
        }
        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _context.Categories.Where(c => !c.IsDeleted).ToListAsync();
            ViewBag.Tags = await _context.Tags.Where(t => !t.IsDeleted).ToListAsync();
            ViewBag.Colors = await _context.Colors.ToListAsync();
            ViewBag.Sizes = await _context.Sizes.ToListAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            ViewBag.Categories = await _context.Categories.Where(c=>!c.IsDeleted).ToListAsync();
            ViewBag.Tags = await _context.Tags.Where(t=>!t.IsDeleted).ToListAsync();
            ViewBag.Colors = await _context.Colors.ToListAsync();
            ViewBag.Sizes = await _context.Sizes.ToListAsync();

            if (!ModelState.IsValid) return View();

            if (!await _context.Categories.AnyAsync(b => b.Id == product.CategoryId && !b.IsDeleted))
            {
                ModelState.AddModelError("CategoryId", "Duzgun Category Secin ");
                return View();
            }

            if (string.IsNullOrWhiteSpace(product.Name))
            {
                ModelState.AddModelError("Name", "Bosluq Olmamalidir");
                return View();
            }

            if (product.Name.CheckString())
            {
                ModelState.AddModelError("Name", "Yalniz Herf Ola Biler");
                return View();
            }

            if (await _context.Categories.AnyAsync(t => t.Name.ToLower() == product.Name.ToLower()))
            {
                ModelState.AddModelError("Name", "Alreade Exists");
                return View();
            }

            if (product.ImageFile != null)
            {
                if (!product.ImageFile.CheckFileContentType("image/jpeg"))
                {
                    ModelState.AddModelError("ImageFile", "Sonu .jpeg olan fayl daxil edin");
                    return View();
                }
                if (!product.ImageFile.CheckFileSize(300))
                {
                    ModelState.AddModelError("ImageFile", "Seklin olcusu 30 Kb-den cox olmamalidir");
                    return View();
                }
                product.Image = product.ImageFile.CreateFile(_env, "assets", "img", "product");
            }
            else
            {
                ModelState.AddModelError("ImageFile", "Sekil mutleq secilmelidir");
                return View();
            }

            if (product.ProductImageFiles != null && product.ProductImageFiles.Count() > 0 && product.ProductImageFiles.Count() <=4)
            {
                List<ProductImage> productImages = new List<ProductImage>();

                foreach (IFormFile item in product.ProductImageFiles)
                {
                    if (!item.CheckFileContentType("image/jpeg"))
                    {
                        ModelState.AddModelError("ProductImageFiles", "The image type does not match");
                        return View();
                    }
                    if (!item.CheckFileSize(100))
                    {
                        ModelState.AddModelError("ProductImageFiles", "Image size can be up to 100kb");
                        return View();
                    }

                    ProductImage productImage = new ProductImage
                    {
                        Image = item.CreateFile(_env, "assets", "img", "product"),
                        CreatedAt = DateTime.UtcNow.AddHours(4)
                    };

                    productImages.Add(productImage);

                }
                product.ProductImages = productImages;
            }
            else
            {
                ModelState.AddModelError("ProductImageFiles", "Minimum 1 maksimum 4 sekil yukleye bilersiniz");
                return View();
            }

            if (product.TagIds.Count > 0)
            {
                List<ProductTag> productTags = new List<ProductTag>();

                foreach (int item in product.TagIds)
                {
                    if (!await _context.Tags.AnyAsync(s => s.Id == item))
                    {
                        ModelState.AddModelError("TagIds", $"Secilen Id {item} - li Tag Yanlisdir");
                        return View();
                    }
                    if (!await _context.Tags.AnyAsync(t => t.Id != item && !t.IsDeleted))
                    {
                        ModelState.AddModelError("TagIds", $"Secilen Id {item} - li Tag Yanlisdir");
                        return View();
                    }

                    ProductTag productTag = new ProductTag
                    {
                        TagId = item
                    };

                    productTags.Add(productTag);
                }

                product.ProductTags = productTags;
            }

            if (product.SizeIds.Count != product.ColorIds.Count || product.SizeIds.Count != product.Counts.Count || product.Counts.Count != product.ColorIds.Count)
            {
                ModelState.AddModelError("", "Incorect");
                return View();
            }
            foreach (int item in product.SizeIds)
            {
                if (!await _context.Sizes.AnyAsync(s => s.Id == item))
                {
                    ModelState.AddModelError("", "Incorect Size Id");
                    return View();
                }
            }

            foreach (int item in product.ColorIds)
            {
                if (!await _context.Colors.AnyAsync(s => s.Id == item))
                {
                    ModelState.AddModelError("", "Incorect Color Id");
                    return View();
                }
            }

            if(product.DiscountPrice != null && product.DiscountPrice > product.Price)
            {
                ModelState.AddModelError("DiscountPrice", "DiscountPrice deyeri Price deyerinden kicik olmalidir");
                return View();
            }
            else if (product.DiscountPrice == null)
            {
                product.DiscountPrice = 0;
            }

            List<ProductColorSize> productColorSizes = new List<ProductColorSize>();

            for (int i = 0; i < product.ColorIds.Count; i++)
            {
                ProductColorSize productColorSize = new ProductColorSize
                {
                    ColorId = product.ColorIds[i],
                    SizeId = product.SizeIds[i],
                    Count = product.Counts[i]
                };

                productColorSizes.Add(productColorSize);
            }

            product.ProductColorSizes = productColorSizes;

            product.Availability = product.Count > 0 ? true : false;
            product.CreatedAt = DateTime.UtcNow.AddHours(4);
            product.Count = product.Counts.Sum();

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            return RedirectToAction("index");
        }
        public async Task<IActionResult> Update(int? id)
        {
            ViewBag.Categories = await _context.Categories.Where(c => !c.IsDeleted).ToListAsync();
            ViewBag.Tags = await _context.Tags.Where(t => !t.IsDeleted).ToListAsync();
            ViewBag.Colors = await _context.Colors.ToListAsync();
            ViewBag.Sizes = await _context.Sizes.ToListAsync();

            if (id == null) return BadRequest();

            Product dbProduct = await _context.Products
                .Include(p => p.ProductTags).ThenInclude(p => p.Tag)
                .Include(p => p.ProductColorSizes).ThenInclude(p => p.Size)
                .Include(p => p.ProductColorSizes).ThenInclude(p => p.Color)
                .Include(p => p.ProductImages)
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);

            if (dbProduct == null) return NotFound();

            return View(dbProduct);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id,Product product)
        {
            ViewBag.Categories = await _context.Categories.Where(c => !c.IsDeleted).ToListAsync();
            ViewBag.Tags = await _context.Tags.Where(t => !t.IsDeleted).ToListAsync();
            ViewBag.Colors = await _context.Colors.ToListAsync();
            ViewBag.Sizes = await _context.Sizes.ToListAsync();

            if (id == null) return BadRequest();

            Product dbProduct = await _context.Products
                .Include(p=>p.ProductTags).ThenInclude(p=>p.Tag)
                .Include(p=>p.ProductColorSizes).ThenInclude(p=>p.Size)
                .Include(p=>p.ProductColorSizes).ThenInclude(p=>p.Color)
                .Include(p=>p.ProductImages)
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);

            if (product == null) return NotFound();

            if (!ModelState.IsValid) return View();

            if (!await _context.Categories.AnyAsync(b => b.Id == product.CategoryId && !b.IsDeleted))
            {
                ModelState.AddModelError("CategoryId", "Duzgun Category Secin ");
                return View();
            }

            if (string.IsNullOrWhiteSpace(product.Name))
            {
                ModelState.AddModelError("Name", "Bosluq Olmamalidir");
                return View();
            }

            if (product.Name.CheckString())
            {
                ModelState.AddModelError("Name", "Yalniz Herf Ola Biler");
                return View();
            }

            if (await _context.Categories.AnyAsync(t => t.Name.ToLower() == product.Name.ToLower()))
            {
                ModelState.AddModelError("Name", "Alreade Exists");
                return View();
            }

            if (product.ImageFile != null)
            {
                if (!product.ImageFile.CheckFileContentType("image/jpeg"))
                {
                    ModelState.AddModelError("ImageFile", "Secilen Seklin Novu Uygun");
                    return View();
                }

                if (!product.ImageFile.CheckFileSize(300))
                {
                    ModelState.AddModelError("ImageFile", "Secilen Seklin Olcusu Maksimum 300 Kb Ola Biler");
                    return View();
                }
                Helper.DeleteFile(_env, dbProduct.Image, "assets", "img", "product");

                dbProduct.Image = product.ImageFile.CreateFile(_env, "assets", "img", "product");
            }

            int canuploadimage = 4 - (int)(dbProduct.ProductImages?.Where(p => !p.IsDeleted).Count());
            if (product.ProductImageFiles != null && product.ProductImageFiles.Length > canuploadimage)
            {
                ModelState.AddModelError("ProductImageFiles", $"maksimum yukleyebileceyin say {canuploadimage}");
                return View(dbProduct);
            }

            if (product.ProductImageFiles != null && product.ProductImageFiles.Count() > 0)
            {
                foreach (IFormFile item in product.ProductImageFiles)
                {
                    if (!item.CheckFileContentType("image/jpeg"))
                    {
                        ModelState.AddModelError("ProductImageFiles", "The image type does not match");
                        return View(dbProduct);
                    }
                    if (!item.CheckFileSize(100))
                    {
                        ModelState.AddModelError("ProductImageFiles", "Image size can be up to 100kb");
                        return View(dbProduct);
                    }

                    ProductImage productImage = new ProductImage
                    {
                        Image = item.CreateFile(_env, "assets", "img", "product"),
                        UpdatedAt = DateTime.UtcNow.AddHours(4)
                    };

                    dbProduct.ProductImages.Add(productImage);
                }
            }
           
            if (product.TagIds.Count > 0)
            {
                _context.ProductTags.RemoveRange(dbProduct.ProductTags);

                List<ProductTag> productTags = new List<ProductTag>();

                foreach (int item in product.TagIds)
                {
                    if (!await _context.Tags.AnyAsync(t => t.Id != item && !t.IsDeleted))
                    {
                        ModelState.AddModelError("TagIds", $"Secilen Id {item} - li Tag Yanlisdir");
                        return View(product);
                    }

                    ProductTag productTag = new ProductTag
                    {
                        TagId = item
                    };

                    productTags.Add(productTag);
                }

                dbProduct.ProductTags = productTags;
            }
            else
            {
                _context.ProductTags.RemoveRange(dbProduct.ProductTags);
            }

            foreach (int item in product.SizeIds)
            {
                if (!await _context.Sizes.AnyAsync(s => s.Id == item))
                {
                    ModelState.AddModelError("", "Incorect Size Id");
                    return View();
                }
            }

            foreach (int item in product.ColorIds)
            {
                if (!await _context.Colors.AnyAsync(s => s.Id == item))
                {
                    ModelState.AddModelError("", "Incorect Color Id");
                    return View();
                }
            }

            if (product.DiscountPrice != null && product.DiscountPrice > product.Price)
            {
                ModelState.AddModelError("DiscountPrice", "DiscountPrice deyeri Price deyerinden kicik olmalidir");
                return View();
            }
            else if (product.DiscountPrice == null)
            {
                product.DiscountPrice = 0;
            }
            List<ProductColorSize> productColorSizes = new List<ProductColorSize>();

            for (int i = 0; i < product.ColorIds.Count; i++)
            {
                ProductColorSize productColorSize = new ProductColorSize
                {
                    ColorId = product.ColorIds[i],
                    SizeId = product.SizeIds[i],
                    Count = product.Counts[i]
                };

                productColorSizes.Add(productColorSize);
            }

            product.ProductColorSizes = productColorSizes;

            dbProduct.Name = product.Name;
            dbProduct.IsDeleted = product.IsDeleted;
            dbProduct.Price = product.Price;
            dbProduct.DiscountPrice = product.DiscountPrice == null ? 0 : product.DiscountPrice;
            dbProduct.Description = product.Description;
            dbProduct.CategoryId = product.CategoryId;
            dbProduct.ExTax = product.ExTax;
            dbProduct.Count = product.ProductColorSizes.Sum(p=>p.Count);
            dbProduct.Availability = dbProduct.Count > 0 ? true : false;
            dbProduct.ProductColorSizes = product.ProductColorSizes;
            dbProduct.UpdatedAt = DateTime.UtcNow.AddHours(4);


            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Delete(int? id, bool? status, int page = 1)
        {
            if (id == null) return BadRequest();
            Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
            if (product == null) return NotFound();
            product.IsDeleted = true;
            product.DeletedAt = DateTime.UtcNow.AddHours(4);
            await _context.SaveChangesAsync();

            return RedirectToAction("index", new { status, page });
        }
        public async Task<IActionResult> Restore(int? id,bool? status,int page = 1)
        {
            if (id == null) return BadRequest();
            Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id && p.IsDeleted);
            if (product == null) return NotFound();
            product.IsDeleted = false;
            await _context.SaveChangesAsync();


            return RedirectToAction("index", new { status, page });
        }
        public IActionResult Detail(int? id)
        {
            if (id == null) return BadRequest();

            Product product = _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductColorSizes).ThenInclude(pc => pc.Color)
                .Include(p => p.ProductColorSizes).ThenInclude(ps => ps.Size)
                .Include(p => p.ProductTags).ThenInclude(pt => pt.Tag)
                .FirstOrDefault(p => p.Id == id);

            if (product == null) return NotFound();

            return View(product);
        }
        public async Task<IActionResult> GetFormColoRSizeCount()
        {
            ViewBag.Colors = await _context.Colors.ToListAsync();
            ViewBag.Sizes = await _context.Sizes.ToListAsync();

            return PartialView("_ProductColorSizePartial");
        }
        public async Task<IActionResult> DeleteImage(int? id)
        {
            ViewBag.Tags = await _context.Tags.Where(pt => !pt.IsDeleted).ToListAsync();
            ViewBag.Colors = await _context.Colors.Where(pt => !pt.IsDeleted).ToListAsync();
            ViewBag.Sizes = await _context.Sizes.Where(pt => !pt.IsDeleted).ToListAsync();
            ViewBag.Categories = await _context.Categories.Where(pt => !pt.IsDeleted).ToListAsync();

            if (id == null) return BadRequest();

            Product product = await _context.Products
                .Include(p => p.ProductTags).ThenInclude(p => p.Tag)
                .Include(p => p.ProductImages)
                .Include(p => p.ProductColorSizes).ThenInclude(p => p.Size)
                .Include(p => p.ProductColorSizes).ThenInclude(p => p.Color)
                .FirstOrDefaultAsync(p => p.ProductImages.Any(pm => pm.Id == id) && !p.IsDeleted);

            if (product == null) return NotFound();


            product.ProductImages.FirstOrDefault(pi => pi.Id == id).IsDeleted = true;
            product.ProductImages.FirstOrDefault(pi => pi.Id == id).DeletedAt = DateTime.UtcNow.AddHours(4);
            await _context.SaveChangesAsync();
            product.TagIds = product.ProductTags.Select(pt => pt.Tag.Id).ToList();
            product.ColorIds = product.ProductColorSizes.Select(pt => pt.Color.Id).ToList();
            product.SizeIds = product.ProductColorSizes.Select(pt => pt.Size.Id).ToList();

            return PartialView("_ProductUpdateImagePartial", product);
        }
    }
}
