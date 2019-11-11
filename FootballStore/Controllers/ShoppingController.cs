using FootballStore.DAL;
using FootballStore.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace FootballStore.Controllers
{
    [Authorize]
    public class ShoppingController : Controller
    {
        private StoreDbContext _db = new StoreDbContext();
        // GET: Shopping
        public ActionResult Shopping(bool? NameAscending = null, bool? PriceAscending = null, bool? AmountAscending = null)
        {
            var userId = User.Identity.GetUserId();
            var user = _db.Users.Find(userId);
            var groupProducts = from order in user.Orders
                                group order by order.ProductId into grOrder
                                select new ShoppingCard
                                {
                                    Id = grOrder.FirstOrDefault().Id,
                                    Name = grOrder.FirstOrDefault().Product.Name,
                                    Price = grOrder.FirstOrDefault().Product.Price,
                                    Amount = grOrder.Count()
                                };

            //Caculate totalPrice
            var totalPrice = 0;
            foreach (var group in groupProducts)
            {
                totalPrice += group.Price * group.Amount;
            }
            ViewBag.TotalPrice = totalPrice;

            //Arrange products
            if (PriceAscending != null)
            {
                groupProducts = PriceAscending == true ? groupProducts.OrderBy(p => p.Price) : groupProducts.OrderByDescending(p => p.Price);
                PriceAscending = !PriceAscending;
            }
            else if (AmountAscending != null)
            {
                groupProducts = AmountAscending == true ? groupProducts.OrderBy(p => p.Amount) : groupProducts.OrderByDescending(p => p.Amount);
                AmountAscending = !AmountAscending;
            }
            else
            {
                groupProducts = (NameAscending ?? true) == false ? groupProducts.OrderByDescending(p => p.Name) : groupProducts.OrderBy(p => p.Name);
                NameAscending = !(NameAscending ?? true);
            }
            ViewData["NameAscending"] = NameAscending;
            ViewData["PriceAscending"] = PriceAscending;
            ViewData["AmountAscending"] = AmountAscending;

            return View(groupProducts.ToList());
        }

        public ActionResult Delete(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Order order = _db.Orders.Find(id);
            if (order == null) return HttpNotFound();
            return PartialView("_Delete", order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            Order order = _db.Orders.Find(id);
            _db.Orders.Remove(order);
            _db.SaveChanges();
            return RedirectToAction("Shopping");
        }
    }
}