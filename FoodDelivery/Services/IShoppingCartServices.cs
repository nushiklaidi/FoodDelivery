using FoodDelivery.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FoodDelivery.Services
{
    public interface IShoppingCartServices
    {
        int GetShoppingCartsCount(string id);
        Task<ShoppingCart> AddToShoppingCart(ShoppingCart shoppingCart);
    }
}
