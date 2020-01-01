using FoodDelivery.Models;
using FoodDelivery.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FoodDelivery.Services
{
    public interface ISubCategoryServices
    {
        Task<IEnumerable<SubCategory>> GetAll();
        Task<SubCategory> Create(SubCategoryAndCategoryViewModel model);
        Task<SubCategory> GetId(int? id);
        Task<SubCategory> Update(SubCategory subCategory);
        Task<SubCategory> Delete(int? id);
        IEnumerable<SubCategory> SubCategoryExist(SubCategoryAndCategoryViewModel model);
        Task<IEnumerable<SubCategory>> GetSubCategories(int id);
        Task<IEnumerable<SubCategory>> GetListById(int? id);
    }
}
