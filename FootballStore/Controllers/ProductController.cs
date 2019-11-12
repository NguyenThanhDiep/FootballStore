using FootballStore.DAL;
using FootballStore.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace FootballStore.Controllers
{
    public class ProductController : Controller
    {
        private StoreDbContext _db = new StoreDbContext();
        public ActionResult Product(string name, int min = Int32.MinValue, int max = Int32.MaxValue)
        {
            var products = from p in _db.Products select p;
            if (!String.IsNullOrEmpty(name)) products = products.Where(p => p.Name.Contains(name));
            products = products.Where(p => p.Price >= min && p.Price <= max);

            //Message add product to shooping card
            if (TempData["Message"] != null)
            {
                ViewBag.Message = TempData["Message"];
            }
            return View(products);
        }

        public ActionResult Detail(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = _db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = _db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Price,UrlImage")] Product product)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(product).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Product");
            }
            return View(product);
        }

        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(Product product)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(product).State = EntityState.Added;
                _db.SaveChanges();
                return RedirectToAction("Product");
            }
            return View(product);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Product product = _db.Products.Find(id);
            if (product == null) return HttpNotFound();
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            Product product = _db.Products.Find(id);
            _db.Products.Remove(product);
            _db.SaveChanges();
            return RedirectToAction("Product");
        }

        public ActionResult AddToShoppingCard(int id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                TempData["Message"] = Localization.Product.message_Login;
            }
            else
            {
                try
                {
                    var userId = User.Identity.GetUserId();
                    var product = _db.Products.Find(id);

                    var order = new Order()
                    {
                        ProductId = id,
                        UserId = userId
                    };
                    _db.Entry(order).State = EntityState.Added;
                    _db.SaveChanges();
                    TempData["Message"] = string.Format(Localization.Product.message_Add, product.Name);
                }
                catch (DataException ex)
                {
                    throw new Exception(ex.ToString());
                }
            }

            return RedirectToAction("Product");
        }
    }
}