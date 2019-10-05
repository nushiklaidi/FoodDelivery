using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FoodDelivery.Models.ViewModels
{
    public class OrderDetailsCartViewModel
    {
        public List<ShoppingCart> ListCart { get; set; }
        public Order Order { get; set; }
    }
}
