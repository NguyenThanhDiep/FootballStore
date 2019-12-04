using FootballStore.DAL;
using FootballStore.Models;
using Microsoft.AspNet.Identity;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
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
            var groupProducts = GetProducts();
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

        public ActionResult ChangeAmountProduct(int? id, bool isIncrease)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Order order = _db.Orders.Find(id);
            if (order == null) return HttpNotFound();
            if (isIncrease) ViewBag.isIncrease = true;
            else ViewBag.isIncrease = false;
            return PartialView("_ChangeAmountProduct", order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeAmountProduct(int id, bool isIncrease)
        {
            Order order = _db.Orders.Find(id);
            if (isIncrease)
            {
                var userId = User.Identity.GetUserId();
                _db.Entry(new Order()
                {
                    ProductId = order.ProductId,
                    UserId = userId
                }).State = EntityState.Added;
            }
            else _db.Orders.Remove(order);
            _db.SaveChanges();
            return RedirectToAction("Shopping");
        }

        public ActionResult ExportToExcel(string currentUrl)
        {
            var products = GetProducts();
            #region Order by products
            var queryString = currentUrl.Split(new char[] { '?', '=' });
            if (queryString.Length == 1) products = products.OrderBy(p => p.Name);
            else
            {
                switch (queryString[1])
                {
                    case "PriceAscending":
                        products = queryString[2] == "True" ? products.OrderBy(p => p.Price) : products.OrderByDescending(p => p.Price);
                        break;
                    case "AmountAscending":
                        products = queryString[2] == "True" ? products.OrderBy(p => p.Amount) : products.OrderByDescending(p => p.Amount);
                        break;
                    default:
                        products = queryString[2] == "True" ? products.OrderBy(p => p.Name) : products.OrderByDescending(p => p.Name);
                        break;
                }
            }
            #endregion
            #region Generate Excel File
            ExcelPackage excel = new ExcelPackage();
            var workSheet = excel.Workbook.Worksheets.Add("Sheet1");
            workSheet.TabColor = System.Drawing.Color.Black;
            workSheet.DefaultRowHeight = 12;
            //Header of table  
            workSheet.Row(1).Height = 20;
            workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Row(1).Style.Font.Bold = true;
            workSheet.Cells[1, 1].Value = "Name";
            workSheet.Cells[1, 2].Value = "Price";
            workSheet.Cells[1, 3].Value = "Amount";
            //Body of table  
            int recordIndex = 2;
            int totalPrice = 0;
            foreach (var product in products)
            {
                workSheet.Cells[recordIndex, 1].Value = product.Name;
                workSheet.Cells[recordIndex, 2].Value = product.Price;
                workSheet.Cells[recordIndex, 3].Value = product.Amount;
                totalPrice += product.Price * product.Amount;
                recordIndex++;
            }
            workSheet.Column(1).AutoFit();
            workSheet.Column(2).AutoFit();
            workSheet.Column(3).AutoFit();
            //Row total Price
            workSheet.Cells[recordIndex, 1, recordIndex, 2].Merge = true;
            workSheet.Row(recordIndex).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Row(recordIndex).Style.Font.Bold = true;
            workSheet.Cells[recordIndex, 1].Value = "Total Price";
            workSheet.Cells[recordIndex, 3].Value = totalPrice;

            string excelName = "ShoppingCard" + DateTime.Now.ToString("ddMMyyyyHHmmss");
            using (var memoryStream = new MemoryStream())
            {
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment; filename=" + excelName + ".xlsx");
                excel.SaveAs(memoryStream);
                memoryStream.WriteTo(Response.OutputStream);
                Response.Flush();
                Response.End();
            }
            #endregion

            if (string.IsNullOrEmpty(currentUrl)) RedirectToAction("Shopping");
            return Redirect(currentUrl);
        }

        #region Helper
        public IEnumerable<ShoppingCard> GetProducts()
        {
            var userId = User.Identity.GetUserId();
            var user = _db.Users.Find(userId);
            var result = from order in user.Orders
                         group order by order.ProductId into grOrder
                         select new ShoppingCard
                         {
                             Id = grOrder.FirstOrDefault().Id,
                             Name = grOrder.FirstOrDefault().Product.Name,
                             Price = grOrder.FirstOrDefault().Product.Price,
                             Amount = grOrder.Count()
                         };
            return result;
        }
        #endregion
    }
}