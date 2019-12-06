using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using FootballStore.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;

namespace FootballStore.Controllers
{
    public class RoleController : Controller
    {
        private ApplicationRoleManager _roleManager;
        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }
        public RoleController()
        {
        }
        public RoleController(ApplicationRoleManager roleManager)
        {
            RoleManager = roleManager;
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Role()
        {
            var roles = RoleManager.Roles.ToList();
            return View(roles);
        }

        public ActionResult CreateRole()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateRole(CreateRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var roleFind = await RoleManager.FindByNameAsync(model.Name);
                if (roleFind != null)
                {
                    ModelState.AddModelError("", "Role name has already exist. Please choose another role name.");
                    return View(model);
                }
                var role = new IdentityRole { Name = model.Name };
                var result = await RoleManager.CreateAsync(role);
                if (result.Succeeded)
                {
                    return RedirectToAction("Role");
                }
                AddErrors(result);
            }
            return View(model);
        }

        #region Helpers
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }
        #endregion
    }
}