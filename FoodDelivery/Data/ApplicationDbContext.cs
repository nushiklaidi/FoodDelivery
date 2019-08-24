using System;
using System.Collections.Generic;
using System.Text;
using FoodDelivery.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FoodDelivery.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Category> Category { get; set; }
        public DbSet<SubCategory> SubCategory { get; set; }
        public DbSet<MenuItems> MenuItem { get; set; }
        public DbSet<Coupon> Coupon { get; set; }
    }
}
