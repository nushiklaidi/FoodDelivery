using FoodDelivery.Data;
using FoodDelivery.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FoodDelivery.Services
{
    public class ShoppingCartServices : IShoppingCartServices
    {
        private readonly ApplicationDbContext _db;

        public ShoppingCartServices(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<ShoppingCart> AddToShoppingCart(ShoppingCart shoppingCart)
        {
            ShoppingCart cartFromDb = await _db.ShoppingCart
                    .Where(c => c.ApplicationUserId == shoppingCart.ApplicationUserId && c.MenuItemId == shoppingCart.MenuItemId).FirstOrDefaultAsync();

            if (cartFromDb == null)
            {
                await _db.ShoppingCart.AddAsync(shoppingCart);
            }
            else
            {
                cartFromDb.Count = cartFromDb.Count + shoppingCart.Count;
            }
            await _db.SaveChangesAsync();

            return shoppingCart;
        }

        public int GetShoppingCartsCount(string id)
        {
            return _db.ShoppingCart.Where(u => u.ApplicationUserId == id).ToList().Count;
        }
    }
}
