using FootballStore.Models;
using System.Collections.Generic;
using System.Data.Entity;

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
            context.SaveChanges();
        }
    }
}