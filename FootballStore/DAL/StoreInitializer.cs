using FootballStore.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace FootballStore.DAL
{
    public class StoreInitializer : DropCreateDatabaseIfModelChanges<StoreDbContext>
    {
        protected override void Seed(StoreDbContext context)
        {
            var products = new List<Product>
            {
                new Product
                {
                    Id = 1,
                    Name = "Backpack Classic",
                    Price = 500000,
                    UrlImage = "/Content/images/Backpack_Classic.jpg",
                },
                new Product
                {
                    Id = 2,
                    Name = "Black Tshirt",
                    Price = 800000,
                    UrlImage = "/Content/images/Black_Tshirt.jpg",
                },
                new Product
                {
                    Id = 3,
                    Name = "Football Boot",
                    Price = 200000,
                    UrlImage = "/Content/images/Football_Boot.jpg",
                },
                new Product
                {
                    Id = 4,
                    Name = "Rs1300_Shoes",
                    Price = 400000,
                    UrlImage = "/Content/images/Rs1300_Shoes.jpg",
                },
                new Product
                {
                    Id = 5,
                    Name = "SlipOn Shoes",
                    Price = 900000,
                    UrlImage = "/Content/images/SlipOn_Shoes.jpg",
                },
                new Product
                {
                    Id = 6,
                    Name = "White Tshirt",
                    Price = 700000,
                    UrlImage = "/Content/images/White_Tshirt.jpg",
                }
            };
            products.ForEach(p => context.Products.Add(p));

            //Add default Role: Admin
            if (!context.Roles.Any(r => r.Name == "Admin"))
            {
                var roleStore = new RoleStore<IdentityRole>(context);
                var roleManager = new ApplicationRoleManager(roleStore);
                var role = new IdentityRole { Name = "Admin" };
                var resultRole = roleManager.Create(role);
            }

            //Add default User: ntd,123
            if (!context.Users.Any(u => u.UserName == "ntd"))
            {
                var userStore = new UserStore<User>(context);
                var userManager = new ApplicationUserManager(userStore);
                userManager.UserValidator = new UserValidator<User>(userManager)
                {
                    AllowOnlyAlphanumericUserNames = false,
                    RequireUniqueEmail = false
                };
                userManager.PasswordValidator = new PasswordValidator
                {
                    RequiredLength = 3,
                    RequireNonLetterOrDigit = false,
                    RequireDigit = false,
                    RequireLowercase = false,
                    RequireUppercase = false
                };
                userManager.UserLockoutEnabledByDefault = true;
                userManager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
                userManager.MaxFailedAccessAttemptsBeforeLockout = 5;
                var user = new User { UserName = "ntd", DateOfBirth = new DateTime(1995, 3, 30) };
                var resultUser = userManager.Create(user, "123");
                if (resultUser.Succeeded) userManager.AddToRole(user.Id, "Admin");
            }
            context.SaveChanges();
        }
    }
}