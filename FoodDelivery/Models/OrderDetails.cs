using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FoodDelivery.Models
{
    public class OrderDetails
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int OrderId { get; set; }

        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; }

        [Required]
        public int MenuItemId { get; set; }

        [ForeignKey("MenuItemId")]
        public virtual MenuItems MenuItems { get; set; }

        public int Count { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        public double Price { get; set; }
    }
}
