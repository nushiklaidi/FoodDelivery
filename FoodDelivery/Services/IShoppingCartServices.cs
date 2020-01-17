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
        Task<IEnumerable<ShoppingCart>> GetShoppingCartListByUserId(string userId);
        Task<ShoppingCart> GetShoppingCartById(int id);
        Task<ShoppingCart> OrderItemPlus(int cartId);
        Task<ShoppingCart> OrderItemMinus(int cartId);
        Task<ShoppingCart> OrderItemRemove(int cartId);
    }
}
