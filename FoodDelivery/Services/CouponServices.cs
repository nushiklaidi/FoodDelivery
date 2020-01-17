using FoodDelivery.Data;
using FoodDelivery.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FoodDelivery.Services
{
    public class CouponServices : ICouponServices
    {
        private readonly ApplicationDbContext _db;

        public CouponServices(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<Coupon> Create(Coupon coupon)
        {
            _db.Coupon.Add(coupon);
            await _db.SaveChangesAsync();

            return coupon;
        }

        public async Task<Coupon> Delete(int? id)
        {
            var findId = await GetId(id);

            _db.Coupon.Remove(findId);
            await _db.SaveChangesAsync();

            return findId;
        }

        public async Task<IEnumerable<Coupon>> GetActiveCoupon()
        {
            return await _db.Coupon.Where(c => c.IsActive == true).ToListAsync();
        }

        public async Task<IEnumerable<Coupon>> GetAll()
        {
            return await _db.Coupon.ToListAsync();
        }

        public async Task<Coupon> GetCouponCode(string couponName)
        {
            return await _db.Coupon.Where(c => c.Name.ToLower() == couponName.ToLower()).FirstOrDefaultAsync();
        }

        public async Task<Coupon> GetId(int? id)
        {
            return await _db.Coupon.SingleOrDefaultAsync(c => c.Id == id);
        }

        public Task<Coupon> Update(Coupon coupon)
        {
            throw new NotImplementedException();
        }
    }
}
