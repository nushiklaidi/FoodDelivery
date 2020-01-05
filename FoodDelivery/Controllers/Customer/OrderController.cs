using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using FoodDelivery.Data;
using FoodDelivery.Models;
using FoodDelivery.Models.ViewModels;
using FoodDelivery.Services;
using FoodDelivery.Services.UnitOfWork;
using FoodDelivery.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodDelivery.Controllers.Customer
{
    public class OrderController : Controller
    {
        private readonly IEmailSender _emailSender;
        private readonly IUnitOfWork _unitOfWork;

        private int PageSize = 1;

        public OrderController(IEmailSender emailSender, IUnitOfWork unitOfWork)
        {
            _emailSender = emailSender;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Confirm(int id)
        {
            var getUserId = await _unitOfWork.User.GetCurrentUser();

            OrderDetailsViewModel orderDetailsVM = new OrderDetailsViewModel()
            {
                Order = await _unitOfWork.OrderServices.GetOrderByUserId(id, getUserId.Id),
                OrderDetails = await _unitOfWork.OrderDetailServices.GetDetailByOrderId(id)
            };

            return View(orderDetailsVM);
        }

        public async Task<IActionResult> OrderHistory(int productPage = 1)
        {
            var getUser = await _unitOfWork.User.GetCurrentUser();

            OrderListViewModel orderListVM = new OrderListViewModel()
            {
                Orders = new List<OrderDetailsViewModel>()
            };

            List<Order> orderHeader = await _unitOfWork.OrderServices.GetOrderListByUserId(getUser.Id);

            foreach (Order item in orderHeader)
            {
                OrderDetailsViewModel individual = new OrderDetailsViewModel
                {
                    Order = item,
                    OrderDetails = await _unitOfWork.OrderDetailServices.GetDetailByOrderId(item.Id)
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
                Order = await _unitOfWork.OrderServices.GetOrderById(Id),
                OrderDetails = await _unitOfWork.OrderDetailServices.GetDetailByOrderId(Id)                
            };

            orderDetailsVM.Order.ApplicationUser = await _unitOfWork.User.GetId(orderDetailsVM.Order.UserId);

            return PartialView("_IndividualOrderDetails", orderDetailsVM);
        }

        [Authorize(Roles = StaticDetail.KitchenUser + "," + StaticDetail.ManagerUser)]
        public async Task<IActionResult> ManageOrder()
        {
            List<OrderDetailsViewModel> orderDetailsVM = new List<OrderDetailsViewModel>();

            List<Order> orderHeader = await _unitOfWork.OrderServices.GetSubmitedAndProcessOrder();

            foreach (Order item in orderHeader)
            {
                OrderDetailsViewModel individual = new OrderDetailsViewModel
                {
                    Order = item,
                    OrderDetails = await _unitOfWork.OrderDetailServices.GetDetailByOrderId(item.Id)
                };
                orderDetailsVM.Add(individual);
            }

            return View(orderDetailsVM);
        }

        [Authorize(Roles = StaticDetail.KitchenUser + "," + StaticDetail.ManagerUser)]
        public async Task<IActionResult> OrderPrepare(int OrderId)
        {
            await _unitOfWork.OrderServices.ChangeOrderStatusInProcess(OrderId);

            return RedirectToAction("ManageOrder", "Order");
        }

        [Authorize(Roles = StaticDetail.KitchenUser + "," + StaticDetail.ManagerUser)]
        public async Task<IActionResult> OrderReady(int OrderId)
        {
            await _unitOfWork.OrderServices.ChangeOrderStatusInReady(OrderId);
            
            return RedirectToAction("ManageOrder", "Order");
        }

        [Authorize(Roles = StaticDetail.KitchenUser + "," + StaticDetail.ManagerUser)]
        public async Task<IActionResult> OrderCancel(int OrderId)
        {
            await _unitOfWork.OrderServices.ChangeOrderStatusInCancel(OrderId);

            return RedirectToAction("ManageOrder", "Order");
        }

        [Authorize(Roles = StaticDetail.FrontDeskUser + "," + StaticDetail.ManagerUser)]
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
                    OrderList = await _unitOfWork.OrderServices.GetOrderListByUserName(searchName);
                }
                else
                {
                    if (searchEmail != null)
                    {
                        user = await _unitOfWork.User.GetUserByEmail(searchEmail);

                        OrderList = await _unitOfWork.OrderServices.GetOrderListByUserId(user.Id);
                    }
                    else
                    {
                        if (searchPhone != null)
                        {
                            OrderList = await _unitOfWork.OrderServices.GetOrderListByUserPhone(searchPhone);
                        }
                    }
                }

            }
            else
            {
                OrderList = await _unitOfWork.OrderServices.GetOrderListWithStatusReady();
            }

            foreach (Order item in OrderList)
            {
                OrderDetailsViewModel individual = new OrderDetailsViewModel
                {
                    Order = item,
                    OrderDetails = await _unitOfWork.OrderDetailServices.GetDetailByOrderId(item.Id)
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
            await _unitOfWork.OrderServices.ChangeOrderStatusInDelivery(orderId);

            return RedirectToAction("OrderPickup", "Order");
        }

        [Authorize(Roles = StaticDetail.DeliveryUser + "," + StaticDetail.ManagerUser)]
        public async Task<IActionResult> OrderDelivery(int productPage = 1, string searchEmail = null, string searchPhone = null, string searchName = null)
        {
            OrderListViewModel orderListVM = new OrderListViewModel()
            {
                Orders = new List<OrderDetailsViewModel>()
            };

            List<Order> OrderList = new List<Order>();

            StringBuilder param = new StringBuilder();
            param.Append("/Order/OrderDelivery?productPage=:");

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
                    OrderList = await _unitOfWork.OrderServices.GetOrderListByUserName(searchName);
                }
                else
                {
                    if (searchEmail != null)
                    {
                        user = await _unitOfWork.User.GetUserByEmail(searchEmail);

                        OrderList = await _unitOfWork.OrderServices.GetOrderListByUserId(user.Id);
                    }
                    else
                    {
                        if (searchPhone != null)
                        {
                            OrderList = await _unitOfWork.OrderServices.GetOrderListByUserPhone(searchPhone);
                        }
                    }
                }

            }
            else
            {
                OrderList = await _unitOfWork.OrderServices.GetOrderListWithStatusForDelivery();
            }

            foreach (Order item in OrderList)
            {
                OrderDetailsViewModel individual = new OrderDetailsViewModel
                {
                    Order = item,
                    OrderDetails = await _unitOfWork.OrderDetailServices.GetDetailByOrderId(item.Id)
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

        [Authorize(Roles = StaticDetail.DeliveryUser + "," + StaticDetail.ManagerUser)]
        [HttpPost]
        [ActionName("OrderDelivery")]
        public async Task<IActionResult> OrderDeliveryPost(int orderId)
        {
            await _unitOfWork.OrderServices.ChangeOrderStatusInComplete(orderId);

            return RedirectToAction("OrderDelivery", "Order");
        }
    }
}