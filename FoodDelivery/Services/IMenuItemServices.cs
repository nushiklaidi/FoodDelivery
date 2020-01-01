using FoodDelivery.Models;
using FoodDelivery.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FoodDelivery.Services
{
    public interface IMenuItemServices
    {
        Task<IEnumerable<MenuItems>> GetAll();
        Task<MenuItems> Create(MenuItemViewModel menuItemVM);
        Task<MenuItems> GetId(int? id);
        Task<MenuItems> Delete(int? id);
    }
}
