using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FoodDelivery.Models;
using FoodDelivery.Models.ViewModels;
using FoodDelivery.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using FoodDelivery.Utility;

namespace FoodDelivery.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;

        public HomeController(ApplicationDbContext db)
        {
            _db = db;
        }

        //GET - MenuItem
        public async Task<IActionResult> Index()
        {
            IndexViewModel IndexVM = new IndexViewModel()
            {
                MenuItems = await _db.MenuItem.Include(m => m.Category).Include(m => m.SubCategory).ToListAsync(),
                Category = await _db.Category.ToListAsync(),
                Coupon = await _db.Coupon.Where(c => c.IsActive == true).ToListAsync()
            };

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claim != null)
            {
                var count = _db.ShoppingCart.Where(u => u.ApplicationUserId == claim.Value).ToList().Count;
                HttpContext.Session.SetInt32(StaticDetail.ssShoppingCartCount, count);
            }

            return View(IndexVM);
        }
        
        //GET - Detail
        [Authorize]
        public async Task<IActionResult> Details(int id)
        {
            var menuItemFromDb = await _db.MenuItem
                .Include(m => m.Category)
                .Include(m => m.SubCategory)
                .Where(m => m.Id == id).FirstOrDefaultAsync();

            ShoppingCart shoppingCart = new ShoppingCart()
            {
                MenuItem = menuItemFromDb,
                MenuItemId = menuItemFromDb.Id
            };

            return View(shoppingCart);
        }

        //POST Shopping Card
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Details(ShoppingCart objCart)
        {
            objCart.Id = 0;

            if (ModelState.IsValid)
            {
                var claimsIdentity = (ClaimsIdentity)this.User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                objCart.ApplicationUserId = claim.Value;

                ShoppingCart cartFromDb = await _db.ShoppingCart
                    .Where(c => c.ApplicationUserId == objCart.ApplicationUserId && c.MenuItemId == objCart.MenuItemId).FirstOrDefaultAsync();

                if (cartFromDb == null)
                {
                    await _db.ShoppingCart.AddAsync(objCart);
                }
                else
                {
                    cartFromDb.Count = cartFromDb.Count + objCart.Count;
                }
                await _db.SaveChangesAsync();

                var count = _db.ShoppingCart.Where(c => c.ApplicationUserId == objCart.ApplicationUserId).ToList().Count();
                HttpContext.Session.SetInt32(StaticDetail.ssShoppingCartCount, count);

                return RedirectToAction(nameof(Index));
            }
            else
            {
                var menuItemFromDb = await _db.MenuItem
                    .Include(m => m.Category)
                    .Include(m => m.SubCategory)
                    .Where(m => m.Id == objCart.MenuItemId).FirstOrDefaultAsync();

                ShoppingCart shoppingCart = new ShoppingCart()
                {
                    MenuItem = menuItemFromDb,
                    MenuItemId = menuItemFromDb.Id
                };

                return View(shoppingCart);
            }
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
