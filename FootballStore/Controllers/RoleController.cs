using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using FootballStore.DAL;
using FootballStore.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;

namespace FootballStore.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RoleController : Controller
    {
        private ApplicationRoleManager _roleManager;
        private ApplicationUserManager _userManager;
        private readonly StoreDbContext _db = new StoreDbContext();

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
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public RoleController()
        {
        }
        public RoleController(ApplicationRoleManager roleManager, ApplicationUserManager userManager)
        {
            RoleManager = roleManager;
            UserManager = userManager;
        }

        public ActionResult Role()
        {
            var listRole = new List<Role>();
            var allRoles = RoleManager.Roles.ToList();
            foreach (var roleDB in allRoles)
            {
                var role = new Role
                {
                    Id = roleDB.Id,
                    Name = roleDB.Name
                };
                foreach (var userDB in roleDB.Users)
                {
                    var user = UserManager.FindById(userDB.UserId);
                    role.Users.Add(user);
                    role.UsersName.Add(user.UserName);
                }
                listRole.Add(role);
            }
            return View(listRole);
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
        public async Task<ActionResult> DeleteRole(string id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var role = await RoleManager.FindByIdAsync(id);
            if (role == null) return HttpNotFound();
            ViewBag.Purpose = "DeleteRole";
            return PartialView("_RoleDialog", role);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteRolePost(string id)
        {
            var role = await RoleManager.FindByIdAsync(id);
            var result = await RoleManager.DeleteAsync(role);
            if (!result.Succeeded)
            {
                AddErrors(result);
                return View("Role");
            }
            return RedirectToAction("Role");
        }

        public async Task<ActionResult> ChangeNameRole(string id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var role = await RoleManager.FindByIdAsync(id);
            if (role == null) return HttpNotFound();
            ViewBag.Purpose = "ChangeNameRole";
            return PartialView("_RoleDialog", role);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangeNameRole(string id, string newName)
        {
            var role = await RoleManager.FindByIdAsync(id);
            role.Name = newName;
            var result = await RoleManager.UpdateAsync(role);
            if (!result.Succeeded) AddErrors(result);
            return RedirectToAction("Role");
        }
        public async Task<ActionResult> EditRole(string id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var roleDB = await RoleManager.FindByIdAsync(id);
            if (roleDB == null) return HttpNotFound();
            var role = new Role { Name = roleDB.Name, Id = roleDB.Id };
            foreach (var userDB in roleDB.Users)
            {
                var user = await UserManager.FindByIdAsync(userDB.UserId);
                role.Users.Add(user);
                role.UsersName.Add(user.UserName);
            }
            return View(role);
        }

        public async Task<ActionResult> RemoveUserFromRole(string userId, string roleName)
        {
            if (userId == null || roleName == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var user = await UserManager.FindByIdAsync(userId);
            if (user == null) return HttpNotFound();
            ViewBag.RoleName = roleName;
            return PartialView("_UserDialog", user);
        }
        [HttpPost, ActionName("RemoveUserFromRole")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemoveUserFromRolePost(string userId, string roleName)
        {
            var result = await UserManager.RemoveFromRoleAsync(userId, roleName);
            if (!result.Succeeded)
            {
                AddErrors(result);
                return View("EditRole");
            }
            var roleDB = await RoleManager.FindByNameAsync(roleName);
            return RedirectToAction("EditRole", new { id = roleDB.Id });
        }
        public JsonResult GetHelplistUserName(string userName, string roleName)
        {
            var result = new JsonResult() { JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            var query = $"SELECT * FROM dbo.AspNetUsers WHERE UserName like '{userName}%'";
            var userList = _db.Database.SqlQuery<User>(query).ToList();
            result.Data = userList.Where(u => !UserManager.IsInRole(u.Id, roleName)).Select(u => new { id = u.Id, text = u.UserName });
            return result;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddUserToRole(string roleName, string[] userIds)
        {
            if (userIds == null || userIds.Length == 0) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var role = await RoleManager.FindByNameAsync(roleName);
            foreach (var userId in userIds)
            {
                var result = await UserManager.AddToRoleAsync(userId, roleName);
                if (!result.Succeeded) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return RedirectToAction("EditRole", new { id = role.Id });
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