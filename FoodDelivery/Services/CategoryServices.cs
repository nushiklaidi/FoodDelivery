using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FoodDelivery.Data;
using FoodDelivery.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodDelivery.Services
{
    public class CategoryServices : ICategoryServices
    {
        private readonly ApplicationDbContext _db;
        

        public CategoryServices(ApplicationDbContext db)
        {
            _db = db;
        }

        public List<Category> GetAll()
        {
            var Categories = _db.Category.ToList();

            return Categories;
        }
    }
}
