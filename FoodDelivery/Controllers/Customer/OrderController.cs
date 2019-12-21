using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FoodDelivery.Data;
using FoodDelivery.Models;
using FoodDelivery.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodDelivery.Controllers.Customer
{
    public class OrderController : Controller
    {
        private ApplicationDbContext _db;

        private int PageSize = 1;

        public OrderController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Confirm(int id)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            OrderDetailsViewModel orderDetailsVM = new OrderDetailsViewModel()
            {
                Order = await _db.Order.Include(u => u.ApplicationUser).Where(u => u.Id == id && u.UserId == claim.Value).FirstOrDefaultAsync(),
                OrderDetails = await _db.OrderDetails.Where(o => o.OrderId == id).ToListAsync()
            };

            return View(orderDetailsVM);
        }

        public async Task<IActionResult> OrderHistory(int productPage = 1)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            OrderListViewModel orderListVM = new OrderListViewModel()
            {
                Orders = new List<OrderDetailsViewModel>()
            };
            
            List<Order> orderHeader = await _db.Order.Include(o => o.ApplicationUser).Where(u => u.UserId == claim.Value).ToListAsync();

            foreach (Order item in orderHeader)
            {
                OrderDetailsViewModel individual = new OrderDetailsViewModel
                {
                    Order = item,
                    OrderDetails = await _db.OrderDetails.Where(o => o.OrderId == item.Id).ToListAsync()
                };
                orderListVM.Orders.Add(individual);
            }

            var count = orderListVM.Orders.Count;
            orderListVM.Orders = orderListVM.Orders.OrderByDescending(p => p.Order.Id).Skip((productPage - 1) * PageSize).Take(PageSize).ToList();

            orderListVM.PagingInfo = new PagingInfo
            {
                CurrentPage = productPage,
                ItemsPerPage = PageSize,
                TotalItem = count,
                UrlParam = "/Order/OrderHistory?productPage=:"
            };

            return View(orderListVM);
        }

        public async Task<IActionResult> GetOrderDetails(int Id)
        {
            OrderDetailsViewModel orderDetailsVM = new OrderDetailsViewModel()
            {
                Order = await _db.Order.Where(o => o.Id == Id).FirstOrDefaultAsync(),
                OrderDetails = await _db.OrderDetails.Where(m => m.OrderId == Id).ToListAsync()                
            };

            orderDetailsVM.Order.ApplicationUser = await _db.ApplicationUser.Where(u => u.Id == orderDetailsVM.Order.UserId).FirstOrDefaultAsync();

            return PartialView("_IndividualOrderDetails", orderDetailsVM);
        }
    }
}