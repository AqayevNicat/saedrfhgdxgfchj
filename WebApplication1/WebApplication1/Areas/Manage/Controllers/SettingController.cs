using Microsoft.AspNetCore.Hosting;
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
    [Area("manage")]
    public class SettingController : Controller
    {
        private readonly JuanDbContext _context;
        private readonly IWebHostEnvironment _env;
        public SettingController(JuanDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _context.Settings.ToListAsync());
        }
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null) return BadRequest();
            Setting dbSetting = await _context.Settings.FirstOrDefaultAsync(s => s.Id == id);
            if (dbSetting == null) return NotFound();

            return View(dbSetting);
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Update(int? id,Setting setting)
        {
            if (id == null) return BadRequest();
            Setting dbSetting = await _context.Settings.FirstOrDefaultAsync(s => s.Id == id);
            if (dbSetting == null) return NotFound();

            if (!ModelState.IsValid) return View();

            if (setting.LogoImageFile != null)
            {
                if (!setting.LogoImageFile.CheckFileContentType("image/jpeg"))
                {
                    ModelState.AddModelError("ImageFile", "Secilen Seklin Novu Uygun");
                    return View();
                }

                if (!setting.LogoImageFile.CheckFileSize(300))
                {
                    ModelState.AddModelError("ImageFile", "Secilen Seklin Olcusu Maksimum 300 Kb Ola Biler");
                    return View();
                }
                Helper.DeleteFile(_env, dbSetting.Logo, "assets", "img","logo");

                dbSetting.Logo = setting.LogoImageFile.CreateFile(_env, "assets", "img", "logo");
            }

            dbSetting.WelcomeMsg = setting.WelcomeMsg;
            dbSetting.Phone = setting.Phone;
            dbSetting.WorkDay = setting.WorkDay;
            dbSetting.Address = setting.Address;
            dbSetting.UpdatedAt = DateTime.UtcNow.AddHours(4);
            await _context.SaveChangesAsync();

            return RedirectToAction("index");
        }
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null) return BadRequest();
            Setting dbSetting = _context.Settings.FirstOrDefault(s => s.Id == id);
            if (dbSetting == null) return NotFound();
            return View(dbSetting);
        }
    }
}
