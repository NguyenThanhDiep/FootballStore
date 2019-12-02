using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FootballStore.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Home()
        {
            ViewBag.Title = "Home";
            return View();
        }
        public ActionResult ChangeLanguage(string lang, string returnUrl)
        {
            Session["Culture"] = new CultureInfo(lang);
            string[] url = returnUrl.Split('/');
            switch (url.Length)
            {
                case 3:
                    string actionName = url[2].Split('?')[0];
                    return RedirectToAction(actionName, url[1]);
                default:
                    return RedirectToAction("Home");
            }
        }
        public ActionResult Branch()
        {
            return View();
        }
    }
}