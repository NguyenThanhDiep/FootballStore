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
            ViewBag.Language = lang;
            return View("Home");
        }
    }
}