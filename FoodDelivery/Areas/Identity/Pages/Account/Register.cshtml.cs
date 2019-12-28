using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using FoodDelivery.Data;
using FoodDelivery.Models;
using FoodDelivery.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace FoodDelivery.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly RoleManager<IdentityRole> _roleManager;
        private ApplicationDbContext _db;

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext db)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _roleManager = roleManager;
            _db = db;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Required]
            public string Name { get; set; }

            [Display(Name = "Street Adress")]
            public string StreetAdress { get; set; }

            [Display(Name = "Phone Number")]
            public string PhoneNumber { get; set; }

            public string City { get; set; }

            [Display(Name = "Postal Code")]
            public string PostalCode { get; set; }  
        }

        public void OnGet(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            string role = Request.Form["rdUserRole"].ToString();

            returnUrl = returnUrl ?? Url.Content("~/");
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser {
                    UserName = Input.Email,
                    Email = Input.Email,
                    Name = Input.Name,
                    City = Input.City,
                    StreetAdress = Input.StreetAdress,
                    PostalCode = Input.PostalCode,
                    PhoneNumber = Input.PhoneNumber
                };
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    if (!await _roleManager.RoleExistsAsync(StaticDetail.ManagerUser))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(StaticDetail.ManagerUser));
                    }
                    if (!await _roleManager.RoleExistsAsync(StaticDetail.KitchenUser))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(StaticDetail.KitchenUser));
                    }
                    if (!await _roleManager.RoleExistsAsync(StaticDetail.FrontDeskUser))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(StaticDetail.FrontDeskUser));
                    }
                    if (!await _roleManager.RoleExistsAsync(StaticDetail.CustomerEndUser))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(StaticDetail.CustomerEndUser));
                    }
                    if (!await _roleManager.RoleExistsAsync(StaticDetail.DeliveryUser))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(StaticDetail.DeliveryUser));
                    }

                    if (role == StaticDetail.KitchenUser)
                    {
                        await _userManager.AddToRoleAsync(user, StaticDetail.KitchenUser);
                    }
                    else
                    {
                        if (role == StaticDetail.FrontDeskUser)
                        {
                            await _userManager.AddToRoleAsync(user, StaticDetail.FrontDeskUser);
                        }
                        else
                        {
                            if (role == StaticDetail.ManagerUser)
                            {
                                await _userManager.AddToRoleAsync(user, StaticDetail.ManagerUser);
                            }
                            else
                            {
                                if (role == StaticDetail.DeliveryUser)
                                {
                                    await _userManager.AddToRoleAsync(user, StaticDetail.DeliveryUser);
                                }
                                else
                                {
                                    await _userManager.AddToRoleAsync(user, StaticDetail.CustomerEndUser);
                                    await _signInManager.SignInAsync(user, isPersistent: false);
                                    return LocalRedirect(returnUrl);
                                }
                            }
                        }
                    }
                    await _emailSender.SendEmailAsync(_db.Users.Where(u => u.Email == user.Email).FirstOrDefault().Email, "Food - New User Registration", "New user has been registered successfully");

                    return RedirectToAction("Index", "User");
                    //await _userManager.AddToRoleAsync(user, StaticDetail.ManagerUser);

                    //_logger.LogInformation("User created a new account with password.");

                    //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    //var callbackUrl = Url.Page(
                    //    "/Account/ConfirmEmail",
                    //    pageHandler: null,
                    //    values: new { userId = user.Id, code = code },
                    //    protocol: Request.Scheme);
                    
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
