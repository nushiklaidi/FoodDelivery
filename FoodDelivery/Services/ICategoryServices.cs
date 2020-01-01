using FoodDelivery.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FoodDelivery.Services
{
    public interface ICategoryServices
    {
        Task<IEnumerable<Category>> GetAll();
        Task<Category> Create(Category category);
        Task<Category> GetId(int? id);
        Task<Category> Update(Category category);
        Task<Category> Delete(int? id);
        IEnumerable<Category> GetAllList();
    }
}
