using FoodDelivery.Data;
using FoodDelivery.Models;
using FoodDelivery.Utility;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FoodDelivery.Services
{
    public class OrderServices : IOrderServices
    {
        private readonly ApplicationDbContext _db;
        private readonly IEmailSender _emailSender;

        public OrderServices(ApplicationDbContext db, IEmailSender emailSender)
        {
            _db = db;
            _emailSender = emailSender;
        }

        public async Task<Order> ChangeOrderStatusInCancel(int orderId)
        {
            Order order = await GetOrderById(orderId);
            order.Status = StaticDetail.StatusCancelled;
            await _db.SaveChangesAsync();
            await _emailSender.SendEmailAsync(_db.Users.Where(u => u.Id == order.UserId).FirstOrDefault().Email, "Food - Order Canceled " + order.Id.ToString(), "Order has been canceled successfully");

            return order;
        }

        public async Task<Order> ChangeOrderStatusInComplete(int orderId)
        {
            Order order = await GetOrderById(orderId);
            order.Status = StaticDetail.StatusCompleted;
            await _db.SaveChangesAsync();
            await _emailSender.SendEmailAsync(_db.Users.Where(u => u.Id == order.UserId).FirstOrDefault().Email, "Food - Order Delivered " + order.Id.ToString(), "Order has been Delivered successfully");

            return order;
        }

        public async Task<Order> ChangeOrderStatusInDelivery(int orderId)
        {
            Order order = await GetOrderById(orderId);
            order.Status = StaticDetail.StatusForDelivery;
            await _db.SaveChangesAsync();
            await _emailSender.SendEmailAsync(_db.Users.Where(u => u.Id == order.UserId).FirstOrDefault().Email, "Food - Order Completed " + order.Id.ToString(), "Order has been completed successfully");

            return order;
        }

        public async Task<Order> ChangeOrderStatusInProcess(int orderId)
        {
            Order order = await GetOrderById(orderId);
            order.Status = StaticDetail.StatusInProcess;
            await _db.SaveChangesAsync();

            return order;
        }

        public async Task<Order> ChangeOrderStatusInReady(int orderId)
        {
            Order order = await GetOrderById(orderId);
            order.Status = StaticDetail.StatusReady;
            await _db.SaveChangesAsync();
            await _emailSender.SendEmailAsync(_db.Users.Where(u => u.Id == order.UserId).FirstOrDefault().Email, "Food - Order ready for pickup " + order.Id.ToString(), "Order is ready for pickup");

            return order;
        }

        public async Task<Order> CreateOrder(Order order)
        {
            _db.Order.Add(order);
            await _db.SaveChangesAsync();

            return order;
        }

        public async Task<Order> GetOrderById(int orderId)
        {
            return await _db.Order
                .Where(o => o.Id == orderId)
                .FirstOrDefaultAsync();
        }

        public async Task<Order> GetOrderByUserId(int orderId, string userId)
        {
            return await _db.Order
                .Include(u => u.ApplicationUser)
                .Where(u => u.Id == orderId && u.UserId == userId)
                .FirstOrDefaultAsync();
        }

        public async Task<List<Order>> GetOrderListByUserId(string userId)
        {
            return await _db.Order
                    .Include(o => o.ApplicationUser)
                    .Where(u => u.UserId == userId)
                    .ToListAsync();
        }

        public async Task<List<Order>> GetOrderListByUserName(string userName)
        {
            return await _db.Order.Include(o => o.ApplicationUser)
                        .Where(u => u.PickupName.ToLower().Contains(userName.ToLower()))
                        .ToListAsync();
        }

        public async Task<List<Order>> GetOrderListByUserPhone(string userPhone)
        {
            return await _db.Order.Include(o => o.ApplicationUser)
                                .Where(u => u.PhoneNumber.Contains(userPhone))
                                .ToListAsync();
        }

        public async Task<List<Order>> GetOrderListWithStatusForDelivery()
        {
            return await _db.Order.Include(o => o.ApplicationUser)
                    .Where(u => u.Status == StaticDetail.StatusForDelivery)
                    .ToListAsync();
        }

        public async Task<List<Order>> GetOrderListWithStatusReady()
        {
            return await _db.Order.Include(o => o.ApplicationUser)
                    .Where(u => u.Status == StaticDetail.StatusReady)
                    .ToListAsync();
        }

        public async Task<List<Order>> GetSubmitedAndProcessOrder()
        {
            return await _db.Order
                .Where(o => o.Status == StaticDetail.StatusSubmitted || o.Status == StaticDetail.StatusInProcess)
                .ToListAsync();
        }
    }
}
