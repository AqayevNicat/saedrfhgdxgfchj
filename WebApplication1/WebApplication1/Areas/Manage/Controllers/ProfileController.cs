using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.DAL;
using WebApplication1.Models;
using WebApplication1.ViewModels.Account;

namespace WebApplication1.Areas.Manage.Controllers
{
    [Area("manage")]
    public class ProfileController : Controller
    {
        private readonly JuanDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public ProfileController(JuanDbContext context,UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<ActionResult> Index()
        {
            AppUser appUser = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName.ToLower() == User.Identity.Name.ToLower() && !u.IsAdmin);
            if (appUser == null) return NotFound();
            return View(appUser);
        }
        public async Task<IActionResult> Profile()
        {
            AppUser appUser = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name && !u.IsAdmin);

            if (appUser == null) return RedirectToAction("index", "home");

            MemberProfileVM memberProfileVM = new MemberProfileVM
            {
                Member = new MemberUpdateVM
                {
                    Address = appUser.Address,
                    City = appUser.City,
                    Country = appUser.Country,
                    FullName = appUser.FullName,
                    PhoneNumber = appUser.PhoneNumber,
                    State = appUser.State,
                    UserName = appUser.UserName,
                    ZipCode = appUser.ZipCode,
                    Email = appUser.Email
                },
                Orders = await _context.Orders
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
                .Include(o => o.AppUser)
                .Where(o => !o.IsDeleted && o.AppUserId == appUser.Id).ToListAsync()

            };

            return View(memberProfileVM);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(MemberUpdateVM member)
        {
            AppUser appUser = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name && !u.IsAdmin);

            if (appUser == null) return RedirectToAction("index", "home");
            MemberProfileVM memberProfileVM = new MemberProfileVM
            {
                Member = member
            };
            TempData["ProfileTab"] = "Account";

            if (!ModelState.IsValid)
            {
                return View("Profile", memberProfileVM);
            }

            if (appUser.NormalizedUserName != member.UserName.ToUpper() && await _userManager.Users.AnyAsync(u => u.NormalizedUserName == member.UserName.ToUpper()))
            {
                ModelState.AddModelError("UserName", "UserName Alreade Exist");

                return View("Profile", memberProfileVM);
            }

            if (appUser.NormalizedEmail != member.Email.ToUpper() && await _userManager.Users.AnyAsync(u => u.NormalizedEmail == member.Email.ToUpper()))
            {
                ModelState.AddModelError("Email", "Email Alreade Exist");
                return View("Profile", memberProfileVM);
            }

            appUser.FullName = member.FullName;
            appUser.UserName = member.UserName;
            appUser.Email = member.Email;
            appUser.Address = member.Address;
            appUser.Country = member.Country;
            appUser.City = member.City;
            appUser.State = member.State;
            appUser.ZipCode = member.ZipCode;
            appUser.PhoneNumber = member.PhoneNumber;

            IdentityResult identityResult = await _userManager.UpdateAsync(appUser);

            if (!identityResult.Succeeded)
            {
                foreach (var item in identityResult.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
                return View("Profile", memberProfileVM);
            }

            if (member.Password != null)
            {
                if (string.IsNullOrWhiteSpace(member.CurrentPassword))
                {
                    ModelState.AddModelError("CurrentPassword", "CurrentPassword Is Requered");
                    return View("Profile", memberProfileVM);
                }

                if (!await _userManager.CheckPasswordAsync(appUser, member.CurrentPassword))
                {
                    ModelState.AddModelError("CurrentPassword", "CurrentPassword Is InCorrect");
                    return View("Profile", memberProfileVM);
                }

                string token = await _userManager.GeneratePasswordResetTokenAsync(appUser);
                identityResult = await _userManager.ResetPasswordAsync(appUser, token, member.Password);
                if (!identityResult.Succeeded)
                {
                    foreach (var item in identityResult.Errors)
                    {
                        ModelState.AddModelError("", item.Description);
                    }
                    return View("Profile", memberProfileVM);
                }
            }

            return RedirectToAction("Profile");
        }
    }
}
