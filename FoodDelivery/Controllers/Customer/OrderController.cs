using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using FoodDelivery.Data;
using FoodDelivery.Models;
using FoodDelivery.Models.ViewModels;
using FoodDelivery.Utility;
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

        [Authorize(Roles = StaticDetail.KitchenUser + "," + StaticDetail.ManagerUser)]
        public async Task<IActionResult> ManageOrder()
        {
            List<OrderDetailsViewModel> orderDetailsVM = new List<OrderDetailsViewModel>();
            
            List<Order> orderHeader = await _db.Order
                .Where(o => o.Status == StaticDetail.StatusSubmitted || o.Status == StaticDetail.StatusInProcess)                
                .ToListAsync();

            foreach (Order item in orderHeader)
            {
                OrderDetailsViewModel individual = new OrderDetailsViewModel
                {
                    Order = item,
                    OrderDetails = await _db.OrderDetails.Where(o => o.OrderId == item.Id).ToListAsync()
                };
                orderDetailsVM.Add(individual);
            }

            return View(orderDetailsVM);
        }

        [Authorize(Roles = StaticDetail.KitchenUser + "," + StaticDetail.ManagerUser)]
        public async Task<IActionResult> OrderPrepare(int OrderId)
        {
            Order order = await _db.Order.FindAsync(OrderId);
            order.Status = StaticDetail.StatusInProcess;
            await _db.SaveChangesAsync();
            return RedirectToAction("ManageOrder", "Order");
        }

        [Authorize(Roles = StaticDetail.KitchenUser + "," + StaticDetail.ManagerUser)]
        public async Task<IActionResult> OrderReady(int OrderId)
        {
            Order order = await _db.Order.FindAsync(OrderId);
            order.Status = StaticDetail.StatusReady;
            await _db.SaveChangesAsync();

            //Email login to notify user that order is ready fot pickup

            return RedirectToAction("ManageOrder", "Order");
        }

        [Authorize(Roles = StaticDetail.KitchenUser + "," + StaticDetail.ManagerUser)]
        public async Task<IActionResult> OrderCancel(int OrderId)
        {
            Order order = await _db.Order.FindAsync(OrderId);
            order.Status = StaticDetail.StatusCancelled;
            await _db.SaveChangesAsync();
            return RedirectToAction("ManageOrder", "Order");
        }

        public async Task<IActionResult> OrderPickup(int productPage = 1, string searchEmail = null, string searchPhone = null, string searchName = null)
        {
            OrderListViewModel orderListVM = new OrderListViewModel()
            {
                Orders = new List<OrderDetailsViewModel>()
            };

            List<Order> OrderList = new List<Order>();

            StringBuilder param = new StringBuilder();
            param.Append("/Order/OrderPickup?productPage=:");

            param.Append("&searchName=");
            if (searchName != null)
            {
                param.Append(searchName);
            }

            param.Append("&searchEmail=");
            if (searchEmail != null)
            {
                param.Append(searchEmail);
            }

            param.Append("&searchPhone=");
            if (searchPhone != null)
            {
                param.Append(searchPhone);
            }

            if (searchEmail != null || searchName != null || searchPhone != null)
            {
                var user = new ApplicationUser();

                if (searchName != null)
                {
                    OrderList = await _db.Order.Include(o => o.ApplicationUser)
                        .Where(u => u.PickupName.ToLower().Contains(searchName.ToLower()))
                        .ToListAsync();
                }
                else
                {
                    if (searchEmail != null)
                    {
                        user = await _db.ApplicationUser
                            .Where(u => u.Email.ToLower().Contains(searchEmail.ToLower()))
                            .FirstOrDefaultAsync();

                        OrderList = await _db.Order.Include(o => o.ApplicationUser)
                            .Where(o => o.UserId == user.Id)
                            .ToListAsync();
                    }
                    else
                    {
                        if (searchPhone != null)
                        {
                            OrderList = await _db.Order.Include(o => o.ApplicationUser)
                                .Where(u => u.PhoneNumber.Contains(searchPhone))
                                .ToListAsync();
                        }
                    }
                }

            }
            else
            {
                OrderList = await _db.Order.Include(o => o.ApplicationUser)
                    .Where(u => u.Status == StaticDetail.StatusReady)
                    .ToListAsync();
            }

            foreach (Order item in OrderList)
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
                UrlParam = param.ToString()
            };

            return View(orderListVM);
        }

        [Authorize(Roles = StaticDetail.FrontDeskUser + "," + StaticDetail.ManagerUser)]
        [HttpPost]
        [ActionName("OrderPickup")]
        public async Task<IActionResult> OrderPickupPost(int orderId)
        {
            Order order = await _db.Order.FindAsync(orderId);
            order.Status = StaticDetail.StatusCompleted;
            await _db.SaveChangesAsync();

            return RedirectToAction("OrderPickup", "Order");
        }
    }
}