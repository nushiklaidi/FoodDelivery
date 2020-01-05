using FoodDelivery.Data;
using FoodDelivery.Models;
using FoodDelivery.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FoodDelivery.Services
{
    public class MenuItemServices : IMenuItemServices
    {
        private readonly ApplicationDbContext _db;

        public MenuItemServices(ApplicationDbContext db)
        {
            _db = db;
        }
        
        public async Task<MenuItems> Create(MenuItemViewModel menuItemVM)
        {
            _db.MenuItem.Add(menuItemVM.MenuItems);
            await _db.SaveChangesAsync();

            return menuItemVM.MenuItems;
        }

        public async Task<MenuItems> Delete(int? id)
        {
            MenuItems menuItem = await GetId(id);

            _db.MenuItem.Remove(menuItem);
            await _db.SaveChangesAsync();

            return menuItem;
        }

        public async Task<IEnumerable<MenuItems>> GetAll()
        {
            return await _db.MenuItem.Include(c => c.Category).Include(s => s.SubCategory).ToListAsync();
        }

        public async Task<MenuItems> GetId(int? id)
        {
            return await _db.MenuItem.Include(c => c.Category).Include(s => s.SubCategory).Where(m => m.Id == id).FirstOrDefaultAsync();
        }        
    }
}
