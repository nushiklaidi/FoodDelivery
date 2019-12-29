using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FoodDelivery.Data;
using FoodDelivery.Models;
using FoodDelivery.Models.ViewModels;
using FoodDelivery.Services;
using FoodDelivery.Services.UnitOfWork;
using FoodDelivery.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FoodDelivery.Controllers.Admin
{
    [Authorize(Roles = StaticDetail.ManagerUser)]
    public class SubCategoryController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IUnitOfWork _unitOfWork;

        public SubCategoryController(ApplicationDbContext db, IUnitOfWork unitOfWork)
        {
            _db = db;
            _unitOfWork = unitOfWork;
        }

        [TempData]
        public string StatusMessage { get; set; }
          
        public async Task<IActionResult> Index()
        {
            return View(await _unitOfWork.SubCategory.GetAll());
        }

        //GET - CREATE
        public async Task<IActionResult> Create()
        {
            SubCategoryAndCategoryViewModel model = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await _unitOfWork.Category.GetAll(),
                SubCategory = new SubCategory(),
                SubCategoryList = await _unitOfWork.SubCategory.GetAll()
            };

            return View(model);
        }

        //POST - CREATE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SubCategoryAndCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var subCategoryExist = _unitOfWork.SubCategory.SubCategoryExist(model);

                if (subCategoryExist.Count() > 0)
                {
                    //ERROR
                    StatusMessage = "Error: Sub Category exists under " + subCategoryExist.First().Category.Name + " category. Please use another name.";
                }
                else
                {
                    await _unitOfWork.SubCategory.Create(model);

                    return RedirectToAction(nameof(Index));
                }
                                
            }

            SubCategoryAndCategoryViewModel modelVM = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await _unitOfWork.Category.GetAll(),
                SubCategory = new SubCategory(),
                SubCategoryList = await _unitOfWork.SubCategory.GetAll(),
                StatusMessage = StatusMessage
            };

            return View(modelVM);

        }

        //GET - EDIT
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //var subCategory = await _db.SubCategory.SingleOrDefaultAsync(s => s.Id == id);
            var subCategory = await _unitOfWork.SubCategory.GetId(id);

            if (subCategory == null)
            {
                return NotFound();
            }

            SubCategoryAndCategoryViewModel model = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await _unitOfWork.Category.GetAll(),
                SubCategory = subCategory,
                SubCategoryList = await _unitOfWork.SubCategory.GetAll()
            };

            return View(model);
        }

        //POST - EDIT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SubCategoryAndCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var subCategoryExist = _unitOfWork.SubCategory.SubCategoryExist(model);

                if (subCategoryExist.Count() > 0)
                {
                    //ERROR
                    StatusMessage = "Error: Sub Category exists under " + subCategoryExist.First().Category.Name + " category. Please use another name.";
                }
                else
                {
                    await _unitOfWork.SubCategory.Update(model.SubCategory);

                    return RedirectToAction(nameof(Index));
                }

            }

            SubCategoryAndCategoryViewModel modelVM = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await _db.Category.ToListAsync(),
                SubCategory = new Models.SubCategory(),
                //SubCategoryList = await _db.SubCategory.OrderBy(p => p.Name).Select(p => p.Name).Distinct().ToListAsync(),
                SubCategoryList = await _unitOfWork.SubCategory.GetAll(),
                StatusMessage = StatusMessage
            };

            return View(modelVM);

        }

        //GET - DETAILS
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
                        
            var subCategory = await _unitOfWork.SubCategory.GetId(id);

            if (subCategory == null)
            {
                return NotFound();
            }

            return View(subCategory);                 
        }

        //GET - DELETE
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subCategory = await _unitOfWork.SubCategory.GetId(id);

            if (subCategory == null)
            {
                return NotFound();
            }

            return View(subCategory);
        }

        //POST - DELETE
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subCategory = await _db.SubCategory.SingleOrDefaultAsync(s => s.Id == id);

            if (subCategory == null)
            {
                return NotFound();
            }

            await _unitOfWork.SubCategory.Delete(id);

            return RedirectToAction(nameof(Index));
        }

        //GET - SUBCATEGORY
        public async Task<IActionResult> GetSubCategory(int id)
        {
            var subCategories = await _unitOfWork.SubCategory.GetSubCategories(id);

            return Json(new SelectList(subCategories, "Id", "Name"));
        }
        
    }
}