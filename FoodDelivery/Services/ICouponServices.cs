using FoodDelivery.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FoodDelivery.Services
{
    public interface ICouponServices
    {
        Task<IEnumerable<Coupon>> GetAll();
        Task<Coupon> Create(Coupon coupon);
        Task<Coupon> GetId(int? id);
        Task<Coupon> Update(Coupon coupon);
        Task<Coupon> Delete(int? id);
        Task<IEnumerable<Coupon>> GetActiveCoupon();
        Task<Coupon> GetCouponCode(string couponName);
    }
}
