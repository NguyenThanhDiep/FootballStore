using System;
using System.Collections.Generic;
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
    }
}