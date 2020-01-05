using FoodDelivery.Data;
using FoodDelivery.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FoodDelivery.Services
{
    public class OrderDetailServices : IOrderDetailServices
    {
        public readonly ApplicationDbContext _db;

        public OrderDetailServices(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<List<OrderDetails>> GetDetailByOrderId(int orderId)
        {
            return await _db.OrderDetails
                .Where(o => o.OrderId == orderId)
                .ToListAsync();
        }
    }
}
