using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FoodDelivery.Data;
using FoodDelivery.Services.UnitOfWork;
using FoodDelivery.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodDelivery.Controllers.Admin
{
    [Authorize(Roles = StaticDetail.ManagerUser)]
    public class UserController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {        
            return View(await _unitOfWork.User.GetAll());
        }

        public async Task<IActionResult> Lock(string id)
        {
            await _unitOfWork.User.LockUser(id);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> UnLock(string id)
        {
            await _unitOfWork.User.UnLockUser(id);

            return RedirectToAction(nameof(Index));
        }
    }
}