using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FoodDelivery.Data;
using FoodDelivery.Models;
using FoodDelivery.Models.ViewModels;
using FoodDelivery.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodDelivery.Controllers.Customer
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IEmailSender _emailSender;

        [BindProperty]
        public OrderDetailsCartViewModel orderDetailsVM { get; set; }

        public CartController(ApplicationDbContext db, IEmailSender emailSender)
        {
            _db = db;
            _emailSender = emailSender;
        }
        
        public async Task<IActionResult> Index()
        {
            orderDetailsVM = new OrderDetailsCartViewModel()
            {
                Order = new Order()
            };
            orderDetailsVM.Order.OrderTotal = 0;

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var cart = _db.ShoppingCart.Where(c => c.ApplicationUserId == claim.Value);

            if (cart != null)
            {
                orderDetailsVM.ListCart = cart.ToList();
            }

            foreach(var list in orderDetailsVM.ListCart)
            {
                list.MenuItem = await _db.MenuItem.Where(m => m.Id == list.MenuItemId).FirstOrDefaultAsync();
                orderDetailsVM.Order.OrderTotal = orderDetailsVM.Order.OrderTotal + (list.MenuItem.Price * list.Count);
            }
            orderDetailsVM.Order.OrderTotalOriginal = orderDetailsVM.Order.OrderTotal;

            if (HttpContext.Session.GetString(StaticDetail.ssCouponCode) != null)
            {
                orderDetailsVM.Order.CouponCode = HttpContext.Session.GetString(StaticDetail.ssCouponCode);

                var couponFromDb = await _db.Coupon.Where(c => c.Name.ToLower() == orderDetailsVM.Order.CouponCode.ToLower()).FirstOrDefaultAsync();                
                orderDetailsVM.Order.OrderTotal = StaticDetail.DiscountedPrice(couponFromDb, orderDetailsVM.Order.OrderTotalOriginal);
            }

            return View(orderDetailsVM);
        }

        //GET
        public async Task<IActionResult> Summary()
        {
            orderDetailsVM = new OrderDetailsCartViewModel()
            {
                Order = new Order()
            };
            orderDetailsVM.Order.OrderTotal = 0;

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            ApplicationUser applicationUser = await _db.ApplicationUser.Where(c => c.Id == claim.Value).FirstOrDefaultAsync();

            var cart = _db.ShoppingCart.Where(c => c.ApplicationUserId == claim.Value);

            if (cart != null)
            {
                orderDetailsVM.ListCart = cart.ToList();
            }

            foreach (var list in orderDetailsVM.ListCart)
            {
                list.MenuItem = await _db.MenuItem.Where(m => m.Id == list.MenuItemId).FirstOrDefaultAsync();
                orderDetailsVM.Order.OrderTotal = orderDetailsVM.Order.OrderTotal + (list.MenuItem.Price * list.Count);
            }
            orderDetailsVM.Order.OrderTotalOriginal = orderDetailsVM.Order.OrderTotal;
            orderDetailsVM.Order.PickupName = applicationUser.Name;
            orderDetailsVM.Order.PhoneNumber = applicationUser.PhoneNumber;
            orderDetailsVM.Order.PickUpTime = DateTime.Now;

            if (HttpContext.Session.GetString(StaticDetail.ssCouponCode) != null)
            {
                orderDetailsVM.Order.CouponCode = HttpContext.Session.GetString(StaticDetail.ssCouponCode);

                var couponFromDb = await _db.Coupon.Where(c => c.Name.ToLower() == orderDetailsVM.Order.CouponCode.ToLower()).FirstOrDefaultAsync();
                orderDetailsVM.Order.OrderTotal = StaticDetail.DiscountedPrice(couponFromDb, orderDetailsVM.Order.OrderTotalOriginal);
            }

            return View(orderDetailsVM);
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Summary")]
        public async Task<IActionResult> SummaryPost()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            
            orderDetailsVM.ListCart = await _db.ShoppingCart.Where(c => c.ApplicationUserId == claim.Value).ToListAsync();

            orderDetailsVM.Order.PaymentStatus = StaticDetail.PaymentStatusApproved;
            orderDetailsVM.Order.OrderDate = DateTime.Now;
            orderDetailsVM.Order.UserId = claim.Value;
            orderDetailsVM.Order.Status = StaticDetail.StatusSubmitted;
            orderDetailsVM.Order.PickUpTime = Convert.ToDateTime(orderDetailsVM.Order.PickUpDate.ToShortDateString() + " " + orderDetailsVM.Order.PickUpTime.ToShortTimeString());

            List<OrderDetails> orderDetailsList = new List<OrderDetails>();

            _db.Order.Add(orderDetailsVM.Order);
            await _db.SaveChangesAsync();

            orderDetailsVM.Order.OrderTotalOriginal = 0;
            
            foreach (var item in orderDetailsVM.ListCart)
            {
                item.MenuItem = await _db.MenuItem.Where(m => m.Id == item.MenuItemId).FirstOrDefaultAsync();
                OrderDetails orderDetails = new OrderDetails
                {
                    MenuItemId = item.MenuItemId,
                    OrderId = orderDetailsVM.Order.Id,
                    Name = item.MenuItem.Name,
                    Price = item.MenuItem.Price,
                    Count = item.Count
                };
                orderDetailsVM.Order.OrderTotalOriginal += orderDetails.Count * orderDetails.Price;
                _db.OrderDetails.Add(orderDetails);
            }

            if (HttpContext.Session.GetString(StaticDetail.ssCouponCode) != null)
            {
                orderDetailsVM.Order.CouponCode = HttpContext.Session.GetString(StaticDetail.ssCouponCode);

                var couponFromDb = await _db.Coupon.Where(c => c.Name.ToLower() == orderDetailsVM.Order.CouponCode.ToLower()).FirstOrDefaultAsync();
                orderDetailsVM.Order.OrderTotal = StaticDetail.DiscountedPrice(couponFromDb, orderDetailsVM.Order.OrderTotalOriginal);
            }
            else
            {
                orderDetailsVM.Order.OrderTotal = orderDetailsVM.Order.OrderTotalOriginal;
            }

            orderDetailsVM.Order.CouponCodeDiscount = orderDetailsVM.Order.OrderTotalOriginal - orderDetailsVM.Order.OrderTotal;
            
            _db.ShoppingCart.RemoveRange(orderDetailsVM.ListCart);

            HttpContext.Session.SetInt32(StaticDetail.ssShoppingCartCount, 0);

            await _emailSender.SendEmailAsync(_db.Users.Where(u => u.Id == claim.Value).FirstOrDefault().Email, "Food - Order Created " + orderDetailsVM.Order.Id.ToString(), "Order has been submitted successfully");

            await _db.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }

        public IActionResult AddCoupon()
        {
            if (orderDetailsVM.Order.CouponCode == null)
            {
                orderDetailsVM.Order.CouponCode = "";
            }

            HttpContext.Session.SetString(StaticDetail.ssCouponCode, orderDetailsVM.Order.CouponCode);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult RemoveCoupon()
        {
            HttpContext.Session.SetString(StaticDetail.ssCouponCode, string.Empty);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Plus(int cartId)
        {
            var cart = await _db.ShoppingCart.Where(c => c.Id == cartId).FirstOrDefaultAsync();
            cart.Count += 1;

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Minus(int cartId)
        {
            var cart = await _db.ShoppingCart.Where(c => c.Id == cartId).FirstOrDefaultAsync();

            if (cart.Count == 1)
            {
                _db.ShoppingCart.Remove(cart);
                await _db.SaveChangesAsync();

                var cnt = _db.ShoppingCart.Where(c => c.ApplicationUserId == cart.ApplicationUserId).ToList().Count;
                HttpContext.Session.SetInt32(StaticDetail.ssShoppingCartCount, cnt);
            }
            else
            {
                cart.Count -= 1;

                await _db.SaveChangesAsync();                
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Remove(int cartId)
        {
            var cart = await _db.ShoppingCart.Where(c => c.Id == cartId).FirstOrDefaultAsync();

            _db.ShoppingCart.Remove(cart);
            await _db.SaveChangesAsync();

            var cnt = _db.ShoppingCart.Where(c => c.ApplicationUserId == cart.ApplicationUserId).ToList().Count;
            HttpContext.Session.SetInt32(StaticDetail.ssShoppingCartCount, cnt);
            
            return RedirectToAction(nameof(Index));
        }
    }
}