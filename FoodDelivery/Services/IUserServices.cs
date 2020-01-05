using FoodDelivery.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FoodDelivery.Services
{
    public interface IUserServices
    {
        Task<IEnumerable<ApplicationUser>> GetAll();
        Task<ApplicationUser> GetCurrentUser();
        Task<ApplicationUser> GetId(string id);
        Task<ApplicationUser> LockUser(string id);
        Task<ApplicationUser> UnLockUser(string id);
        Task<ApplicationUser> GetUserByEmail(string userEmail);
    }
}
