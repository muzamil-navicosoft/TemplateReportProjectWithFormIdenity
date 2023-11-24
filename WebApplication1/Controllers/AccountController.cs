using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        ProjectContext context = new ProjectContext();
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(UserModel model)
        {
            using (ProjectContext context = new ProjectContext())
            {
                bool IsValidUser = context.Users.Any(user => user.UserName.ToLower() ==
                     model.UserName.ToLower() && user.UserPassword == model.UserPassword);
                if (IsValidUser)
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, false);
                    return RedirectToAction("Index", "Report");
                }
                ModelState.AddModelError("", "invalid Username or Password");
                return View();
            }
        }
        public ActionResult list()
        {
            List<User> users = new List<User>();
            users = context.Users.ToList();
            
            return View(users);
        }
        public ActionResult Signup()
        {
            var list = context.Roles.ToList();
            ViewBag.roles = new SelectList(list, "ID", "RollName");
            return View();
        }
        [HttpPost]
        public ActionResult Signup(UserModelWithRole model)
        {
            User use = new User()
            {
                UserName = model.UserName,
                UserPassword = model.UserPassword
            };
            var result =  context.Users.Add(use);
            context.SaveChanges();
            UserRolesMapping rolemap = new UserRolesMapping
            {
                UserID = result.ID,
                RoleID = model.role
            };
            context.UserRolesMappings.Add(rolemap);
            context.SaveChanges();
            
            return RedirectToAction("Login");
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }
    }
}