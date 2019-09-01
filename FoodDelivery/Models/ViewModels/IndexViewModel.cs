using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FoodDelivery.Models.ViewModels
{
    public class IndexViewModel
    {
        public IEnumerable<MenuItems> MenuItems { get; set; }
        public IEnumerable<Category> Category { get; set; }
        public IEnumerable<Coupon> Coupon { get; set; }
    }
}
