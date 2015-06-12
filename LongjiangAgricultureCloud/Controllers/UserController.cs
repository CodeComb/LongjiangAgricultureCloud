using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LongjiangAgricultureCloud.Schema;
using LongjiangAgricultureCloud.Models;
using LongjiangAgricultureCloud.Helpers;

namespace LongjiangAgricultureCloud.Controllers
{
    [CheckRole(UserRole.系统管理员)]
    public class UserController : BaseController
    {
        // GET: User
        public ActionResult Index(string Username, UserRole? Role, string Name, int p = 0)
        {
            IEnumerable<User> query = DB.Users;
            if (!string.IsNullOrEmpty(Username))
                query = query.Where(x => x.Username == Username);
            if (!string.IsNullOrEmpty(Name))
                query = query.Where(x => x.Name.Contains(Name));
            if (Role.HasValue)
                query = query.Where(x => x.Role == Role.Value);
            ViewBag.PageInfo = PagerHelper.Do(ref query, 50, p);
            return View(query);
        }

        public ActionResult Delete(int id)
        {
            var user = DB.Users.Find(id);
            DB.Users.Remove(user);
            DB.SaveChanges();
            return Content("ok");
        }
    }
}