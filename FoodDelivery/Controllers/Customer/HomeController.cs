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
using FoodDelivery.Services.UnitOfWork;

namespace FoodDelivery.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(IUnitOfWork untOfWork)
        {
            _unitOfWork = untOfWork;
        }

        //GET - MenuItem
        public async Task<IActionResult> Index()
        {
            IndexViewModel IndexVM = new IndexViewModel()
            {
                MenuItems = await _unitOfWork.MenuItem.GetAll(),
                Category = _unitOfWork.Category.GetAllList(),
                Coupon = await _unitOfWork.Coupon.GetActiveCoupon()
            };
            
            var getUserId = await _unitOfWork.User.GetCurrentUser();

            if (getUserId != null)
            {                
                int count = _unitOfWork.ShoppingCart.GetShoppingCartsCount(getUserId.Id);
                HttpContext.Session.SetInt32(StaticDetail.ssShoppingCartCount, count);
            }

            return View(IndexVM);
        }
        
        //GET - Detail
        [Authorize]
        public async Task<IActionResult> Details(int id)
        {
            var menuItemFromDb = await _unitOfWork.MenuItem.GetId(id);

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
                var getUser = await _unitOfWork.User.GetCurrentUser();
                objCart.ApplicationUserId = getUser.Id;

                await _unitOfWork.ShoppingCart.AddToShoppingCart(objCart);

                return RedirectToAction(nameof(Index));
            }
            else
            {
                var menuItemFromDb = await _unitOfWork.MenuItem.GetId(objCart.MenuItem.Id);

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
