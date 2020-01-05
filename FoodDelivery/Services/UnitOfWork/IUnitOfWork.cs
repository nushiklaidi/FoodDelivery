using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FoodDelivery.Services.UnitOfWork
{
    public interface IUnitOfWork
    {
        ICategoryServices Category { get; }
        ISubCategoryServices SubCategory { get; }
        ICouponServices Coupon { get; }
        IUserServices User { get; }
        IMenuItemServices MenuItem { get; }
        IShoppingCartServices ShoppingCart { get; }
        IOrderServices OrderServices { get; }
        IOrderDetailServices OrderDetailServices { get; }
    }
}
