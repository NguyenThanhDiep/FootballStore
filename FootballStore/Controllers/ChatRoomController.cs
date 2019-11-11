using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FootballStore.Controllers
{
    public class ChatRoomController : Controller
    {
        // GET: ChatRoom
        public ActionResult ChatRoom()
        {
            ViewBag.userName = string.IsNullOrEmpty(User.Identity.Name) ? "anonymous" : User.Identity.Name;
            return View();
        }
    }
}