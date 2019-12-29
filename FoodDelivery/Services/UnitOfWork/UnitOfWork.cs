using FoodDelivery.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FoodDelivery.Services.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;

        public ICategoryServices Category { get; private set; }
        public ISubCategoryServices SubCategory { get; private set; }

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;

            Category = new CategoryServices(db);
            SubCategory = new SubCategoryServices(db);
        }
    }
}
