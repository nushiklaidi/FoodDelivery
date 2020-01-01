using FoodDelivery.Data;
using FoodDelivery.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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

        public ICategoryServices Category { get; private set; }
        public ISubCategoryServices SubCategory { get; private set; }
        public ICouponServices Coupon { get; private set; }
        public IUserServices User { get; private set; }
        public IMenuItemServices MenuItem { get; private set; }

        public UnitOfWork(ApplicationDbContext db, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;

            Category = new CategoryServices(db);
            SubCategory = new SubCategoryServices(db);
            Coupon = new CouponServices(db);
            User = new UserServices(db, httpContextAccessor);
            MenuItem = new MenuItemServices(db);
        }
    }
}
