using FoodDelivery.Data;
using FoodDelivery.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FoodDelivery.Services
{
    public class UserServices : IUserServices
    {
        private readonly ApplicationDbContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserServices(ApplicationDbContext db, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<ApplicationUser>> GetAll()
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            return await _db.ApplicationUser.Where(u => u.Id != userId).ToListAsync();
        }

        public async Task<ApplicationUser> GetCurrentUser()
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            return await _db.ApplicationUser.Where(u => u.Id == userId).FirstOrDefaultAsync();
        }

        public async Task<ApplicationUser> GetId(string id)
        {
            return await _db.ApplicationUser.Where(u => u.Id == id).FirstOrDefaultAsync();
        }

        public async Task<ApplicationUser> GetUserByEmail(string userEmail)
        {
            return await _db.ApplicationUser
                            .Where(u => u.Email.ToLower().Contains(userEmail.ToLower()))
                            .FirstOrDefaultAsync();
        }
        
        public async Task<ApplicationUser> LockUser(string id)
        {
            if (id == null)
            {
                return null;
            }

            var getUser = await GetId(id);

            if (getUser == null)
            {
                return null;
            }

            getUser.LockoutEnd = DateTime.Now.AddYears(1000);
            await _db.SaveChangesAsync();

            return getUser;
        }

        public async Task<ApplicationUser> UnLockUser(string id)
        {
            if (id == null)
            {
                return null;
            }

            var getUser = await GetId(id);

            if (getUser == null)
            {
                return null;
            }

            getUser.LockoutEnd = DateTime.Now;
            await _db.SaveChangesAsync();

            return getUser;
        }
    }
}
