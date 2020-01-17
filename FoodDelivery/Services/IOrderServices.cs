using FoodDelivery.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FoodDelivery.Services
{
    public interface IOrderServices
    {
        Task<Order> GetOrderByUserId(int orderId, string id);
        Task<Order> GetOrderById(int orderId);
        Task<List<Order>> GetOrderListByUserId(string userId);
        Task<List<Order>> GetOrderListByUserName(string userName);
        Task<List<Order>> GetOrderListByUserPhone(string userPhone);
        Task<List<Order>> GetOrderListWithStatusReady();
        Task<List<Order>> GetSubmitedAndProcessOrder();
        Task<List<Order>> GetOrderListWithStatusForDelivery();
        Task<Order> ChangeOrderStatusInProcess(int orderId);
        Task<Order> ChangeOrderStatusInReady(int orderId);
        Task<Order> ChangeOrderStatusInCancel(int orderId);
        Task<Order> ChangeOrderStatusInDelivery(int orderId);
        Task<Order> ChangeOrderStatusInComplete(int orderId);
        Task<Order> CreateOrder(Order order);
    }
}
