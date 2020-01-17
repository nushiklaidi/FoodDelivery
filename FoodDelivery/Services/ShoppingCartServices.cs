using FoodDelivery.Data;
using FoodDelivery.Models;
using FoodDelivery.Utility;
using Microsoft.AspNetCore.Http;
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

        public async Task<ShoppingCart> GetShoppingCartById(int id)
        {
            return await _db.ShoppingCart.Where(c => c.Id == id).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<ShoppingCart>> GetShoppingCartListByUserId(string userId)
        {
            return await _db.ShoppingCart.Where(s => s.ApplicationUserId == userId).ToListAsync();
        }

        public int GetShoppingCartsCount(string id)
        {
            return _db.ShoppingCart.Where(u => u.ApplicationUserId == id).ToList().Count;
        }

        public async Task<ShoppingCart> OrderItemMinus(int cartId)
        {
            var cart = await GetShoppingCartById(cartId);

            if (cart.Count == 1)
            {
                _db.ShoppingCart.Remove(cart);
                await _db.SaveChangesAsync();
            }
            else
            {
                cart.Count -= 1;
                await _db.SaveChangesAsync();
            }

            return cart;
        }

        public async Task<ShoppingCart> OrderItemPlus(int cartId)
        {
            var cart = await GetShoppingCartById(cartId);
            cart.Count += 1;

            await _db.SaveChangesAsync();

            return cart;
        }

        public async Task<ShoppingCart> OrderItemRemove(int cartId)
        {
            var cart = await GetShoppingCartById(cartId);

            _db.ShoppingCart.Remove(cart);
            await _db.SaveChangesAsync();

            return cart;
        }
    }
}
