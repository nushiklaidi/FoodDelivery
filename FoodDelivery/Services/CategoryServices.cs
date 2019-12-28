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

        public async Task<IEnumerable<Category>> GetAll()
        {
            var Categories = await _db.Category.ToListAsync();

            return Categories;
        }        
    }
}
