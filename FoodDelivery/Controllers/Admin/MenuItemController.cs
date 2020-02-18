using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FoodDelivery.Data;
using FoodDelivery.Models;
using FoodDelivery.Models.ViewModels;
using FoodDelivery.Services.UnitOfWork;
using FoodDelivery.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodDelivery.Controllers.Admin
{
    [Authorize(Roles = StaticDetail.ManagerUser)]
    public class MenuItemController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _hostingEnvironment;

        private readonly IUnitOfWork _unitOfWork;

        [BindProperty]
        public MenuItemViewModel MenuItemVM { get; set; }

        public MenuItemController(ApplicationDbContext db, IWebHostEnvironment hostingEnvironment, IUnitOfWork unitOfWork)
        {
            _db = db;
            _hostingEnvironment = hostingEnvironment;
            _unitOfWork = unitOfWork;

            MenuItemVM = new MenuItemViewModel()
            {
                Category = _unitOfWork.Category.GetAllList(),
                MenuItems = new MenuItems()
            };
        }

        //GET
        public async Task<IActionResult> Index()
        {
            return View(await _unitOfWork.MenuItem.GetAll());
        }

        //GET - CREATE
        public IActionResult Create()
        {
            return View(MenuItemVM);
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePost()
        {
            MenuItemVM.MenuItems.SubCategoryId = Convert.ToInt32(Request.Form["SubCategoryId"].ToString());

            if (!ModelState.IsValid)
            {
                return View(MenuItemVM);
            }

            await _unitOfWork.MenuItem.Create(MenuItemVM);

            //Image saving section
            string webRootPath = _hostingEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;

            var menuItemFromDb = await _unitOfWork.MenuItem.GetId(MenuItemVM.MenuItems.Id);

            if (files.Count > 0)
            {
                //files has been uploaded
                var uploads = Path.Combine(webRootPath, "img");
                var extension = Path.GetExtension(files[0].FileName);
                var fileName = Path.GetFileName(files[0].FileName);

                using (var filesStream = new FileStream(Path.Combine(uploads, MenuItemVM.MenuItems.Id + "_" + fileName), FileMode.Create))
                {
                    files[0].CopyTo(filesStream);
                }
                menuItemFromDb.Image = @"\img\" + MenuItemVM.MenuItems.Id + "_" + fileName;
            }
            else
            {
                //no file was uploaded, so use default file
                var uploads = Path.Combine(webRootPath, @"img\" + StaticDetail.DefaultImg);
                System.IO.File.Copy(uploads, webRootPath + @"\img\" + MenuItemVM.MenuItems.Id + ".png");
                menuItemFromDb.Image = @"\img\" + MenuItemVM.MenuItems.Id + ".png";
            }

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        //GET - EDIT
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            MenuItemVM.MenuItems = await _unitOfWork.MenuItem.GetId(id);
            MenuItemVM.SubCategory = await _unitOfWork.SubCategory.GetListById(MenuItemVM.MenuItems.CategoryId);

            if (MenuItemVM.MenuItems == null)
            {
                return NotFound();
            }

            return View(MenuItemVM);
        }

        //POST
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            MenuItemVM.MenuItems.SubCategoryId = Convert.ToInt32(Request.Form["SubCategoryId"].ToString());

            if (!ModelState.IsValid)
            {
                MenuItemVM.SubCategory = await _unitOfWork.SubCategory.GetListById(MenuItemVM.MenuItems.CategoryId);
                return View(MenuItemVM);
            }
            
            //Image saving section
            string webRootPath = _hostingEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;

            var menuItemFromDb = await _db.MenuItem.FindAsync(MenuItemVM.MenuItems.Id);

            if (files.Count > 0)
            {
                //new files has been uploaded
                var uploads = Path.Combine(webRootPath, "img");
                var extension_new = Path.GetExtension(files[0].FileName);
                var fileName = Path.GetFileName(files[0].FileName);

                //delete the original file
                //var imagePath = Path.Combine(webRootPath, menuItemFromDb.Image.TrimStart('\\'));

                //if (System.IO.File.Exists(imagePath))
                //{
                //    System.IO.File.Delete(imagePath);
                //}

                //we will upload new file
                using (var filesStream = new FileStream(Path.Combine(uploads, MenuItemVM.MenuItems.Id + "_" + fileName), FileMode.Create))
                {
                    files[0].CopyTo(filesStream);
                }
                menuItemFromDb.Image = @"\img\" + MenuItemVM.MenuItems.Id + "_" + fileName;
            }

            menuItemFromDb.Name = MenuItemVM.MenuItems.Name;
            menuItemFromDb.Price = MenuItemVM.MenuItems.Price;
            menuItemFromDb.Spicy = MenuItemVM.MenuItems.Spicy;
            menuItemFromDb.CategoryId = MenuItemVM.MenuItems.CategoryId;
            menuItemFromDb.SubCategoryId = MenuItemVM.MenuItems.SubCategoryId;

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        //GET : Details MenuItem
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            MenuItemVM.MenuItems = await _unitOfWork.MenuItem.GetId(id);

            if (MenuItemVM.MenuItems == null)
            {
                return NotFound();
            }

            return View(MenuItemVM);
        }

        //GET : Delete MenuItem
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            MenuItemVM.MenuItems = await _unitOfWork.MenuItem.GetId(id);

            if (MenuItemVM.MenuItems == null)
            {
                return NotFound();
            }

            return View(MenuItemVM);
        }

        //POST Delete MenuItem
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            string webRootPath = _hostingEnvironment.WebRootPath;
            MenuItems menuItem = await _unitOfWork.MenuItem.GetId(id);

            if (menuItem != null)
            {
                var imagePath = Path.Combine(webRootPath, menuItem.Image.TrimStart('\\'));

                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }

                await _unitOfWork.MenuItem.Delete(id);

            }

            return RedirectToAction(nameof(Index));
        }
    }
}