using FoodDelivery.Data;
using FoodDelivery.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FoodDelivery.Services.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IEmailSender _emailSender;

        public ICategoryServices Category { get; private set; }
        public ISubCategoryServices SubCategory { get; private set; }
        public ICouponServices Coupon { get; private set; }
        public IUserServices User { get; private set; }
        public IMenuItemServices MenuItem { get; private set; }
        public IShoppingCartServices ShoppingCart { get; private set; }
        public IOrderServices OrderServices { get; private set; }
        public IOrderDetailServices OrderDetailServices { get; private set; }

        public UnitOfWork(ApplicationDbContext db, IHttpContextAccessor httpContextAccessor, IEmailSender emailSender)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
            _emailSender = emailSender;

            Category = new CategoryServices(db);
            SubCategory = new SubCategoryServices(db);
            Coupon = new CouponServices(db);
            User = new UserServices(db, httpContextAccessor);
            MenuItem = new MenuItemServices(db);
            ShoppingCart = new ShoppingCartServices(db);
            OrderServices = new OrderServices(db, emailSender);
            OrderDetailServices = new OrderDetailServices(db);
        }
    }
}
