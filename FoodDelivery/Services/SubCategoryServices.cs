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
    public class SubCategoryServices : ISubCategoryServices
    {
        private readonly ApplicationDbContext _db;

        public SubCategoryServices(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<SubCategory> Create(SubCategoryAndCategoryViewModel model)
        {
            _db.SubCategory.Add(model.SubCategory);
            await _db.SaveChangesAsync();

            return model.SubCategory;
        }

        public async Task<SubCategory> Delete(int? id)
        {
            var findId = await GetId(id);

            _db.SubCategory.Remove(findId);
            await _db.SaveChangesAsync();

            return findId;
        }

        public async Task<IEnumerable<SubCategory>> GetAll()
        {
            return await _db.SubCategory.Include(s => s.Category).ToListAsync();
        }

        public async Task<SubCategory> GetId(int? id)
        {
            return await _db.SubCategory.Include(s => s.Category).SingleOrDefaultAsync(c => c.Id == id);
        }

        public IEnumerable<SubCategory> SubCategoryExist(SubCategoryAndCategoryViewModel model)
        {
            var subCategoryExist = _db.SubCategory.Include(s => s.Category)
                                    .Where(s => s.Name == model.SubCategory.Name && s.Category.Id == model.SubCategory.CategoryId);

            return subCategoryExist;
        }

        public async Task<SubCategory> Update(SubCategory subCategory)
        {
            var subCategoryFromDb = await GetId(subCategory.Id);

            subCategoryFromDb.Name = subCategory.Name;
            subCategoryFromDb.Description = subCategory.Description;

            await _db.SaveChangesAsync();

            return subCategory;
        }

        public async Task<IEnumerable<SubCategory>> GetSubCategories(int id)
        {
            return await(from s in _db.SubCategory
                                  where s.CategoryId == id
                                  select s).ToListAsync();
        }

        public async Task<IEnumerable<SubCategory>> GetListById(int? id)
        {
            return await _db.SubCategory.Where(s => s.CategoryId == id).ToListAsync();
        }
    }
}
