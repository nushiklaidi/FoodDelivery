using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FoodDelivery.Models
{
    public class MenuItems
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Spicy { get; set; }

        public enum EnumSpicy {
            NA = 0,
            NotSpicy = 1,
            Spicy = 0,
            VerySpicy = 3
        }

        //public byte[] Image { get; set; }
        public string Image { get; set; }

        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        [Display(Name = "SubCategory")]
        public int SubCategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }

        [ForeignKey("SubCategoryId")]
        public virtual SubCategory SubCategory { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Price should be greater then ${1}")]
        public double Price { get; set; }
    }
}
