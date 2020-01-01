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
            return await _db.Category.ToListAsync();            
        }

        public async Task<Category> Create(Category category)
        {
            _db.Category.Add(category);
            await _db.SaveChangesAsync();

            return category;
        }

        public async Task<Category> GetId(int? id)
        {
            return await _db.Category.FindAsync(id);            
        }

        public async Task<Category> Update(Category category)
        {
            _db.Category.Update(category);
            await _db.SaveChangesAsync();

            return category;
        }

        public async Task<Category> Delete(int? id)
        {
            var findId = await GetId(id);

            _db.Category.Remove(findId);
            await _db.SaveChangesAsync();

            return findId;
        }

        public IEnumerable<Category> GetAllList()
        {
            return _db.Category.ToList();
        }
    }
}
