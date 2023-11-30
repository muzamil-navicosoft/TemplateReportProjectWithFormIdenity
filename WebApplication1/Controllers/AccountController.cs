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
            //List<Role> roles = new List<Role>();
            //roles = context.Roles.ToList();
            List<UserRolesMapping> userRolesMappings = new List<UserRolesMapping>();
            userRolesMappings = context.UserRolesMappings.ToList();

            List<UserModelWithRoleView> userModelWithRoles = new List<UserModelWithRoleView>();
            foreach (var item in userRolesMappings)
            {
                UserModelWithRoleView obj = new UserModelWithRoleView
                {
                    ID= item.ID,
                    UserName = item.User.UserName,
                    role = item.Role.RollName
                };
                userModelWithRoles.Add(obj);
            }
            foreach (var item in users)
            {
                foreach (var itemrole in userModelWithRoles)
                {
                    if (item.UserName == itemrole.UserName)
                        itemrole.UserPassword = item.UserPassword;
                }
            }
            
            return View(userModelWithRoles);
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
            
            return RedirectToAction("list");
        }

        public ActionResult Edit(int id)
        {
            var result = context.UserRolesMappings.Find(id);
            var userRole = context.Roles.Find(result.RoleID);
            var user = context.Users.Find(result.UserID);

            var list = context.Roles.ToList();

            UserModelWithRole obj = new UserModelWithRole
            {
                UserName = user.UserName,
                UserPassword = user.UserPassword,
                role = userRole.ID
            };

            ViewBag.roles = new SelectList(list, "ID", "RollName",obj.role);
            return View(obj);
        }
        [HttpPost]
        public ActionResult Edit(UserModelWithRole model)
        {

            
            var user = context.Users.Where(x => x.UserName.ToLower() == model.UserName.ToLower()).FirstOrDefault();
            //User use = new User()
            //{
            //    UserName = model.UserName,
            //    UserPassword = model.UserPassword
            //};
            user.UserName = model.UserName;
            user.UserPassword = model.UserPassword;
            context.SaveChanges();

            var rolesMapping = context.UserRolesMappings.Find(model.ID);

            rolesMapping.UserID = user.ID;
            rolesMapping.RoleID = model.role;
            
            //.UserRolesMappings.update(rolemap);
            context.SaveChanges();

            return RedirectToAction("list");
        }
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }
    }
}