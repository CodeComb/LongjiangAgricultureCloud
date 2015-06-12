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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            var user = DB.Users.Find(id);
            DB.Users.Remove(user);
            DB.SaveChanges();
            return Content("ok");
        }

        public ActionResult Edit(int id)
        {
            var user = DB.Users.Find(id);
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, User User)
        {
            var user = DB.Users.Find(id);
            if (user.Username != User.Username && DB.Users.Any(x => x.Username == User.Username))
                return Msg("已经存在用户名(手机号)为\"" + User.Username + "\"的用户，请修改后重试！");
            user.Name = User.Name;
            user.AreaID = User.AreaID; //TODO: 选择地区
            user.Question = User.Question;
            user.Answer = User.Answer;
            user.Password = Security.SHA1(User.Password);
            DB.SaveChanges();
            return RedirectToAction("Success", "Shared");
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(User User, string Confirm)
        {
            if (User.Password != Confirm)
                return Msg("两次密码输入不一致，请返回重新尝试！");
            User.Password = Security.SHA1(Confirm);
            DB.Users.Add(User);
            DB.SaveChanges();
            return RedirectToAction("Success", "Shared");
        }

        public ActionResult Area(int? id)
        {
            IEnumerable<Area> query = DB.Areas;
            if (id.HasValue)
                query = query.Where(x => x.FatherID == id.Value);
            else
                query = query.Where(x => x.FatherID == null);
            Area a = null;
            if (id.HasValue)
                a = DB.Areas.Find(id.Value);
            ViewBag.Area = a;
            return View(query.ToList());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateArea(string Title, int? FatherID)
        {
            var area = new Area();
            if (FatherID.HasValue)
            {
                var father = DB.Areas.Find(FatherID.Value);
                area.FatherID = FatherID.Value;
                area.Level = (AreaLevel)(Convert.ToInt32(father.Level) + 1);
            }
            else
            {
                area.Level = AreaLevel.省;
            }
            area.Title = Title;
            DB.Areas.Add(area);
            DB.SaveChanges();
            return RedirectToAction("Success", "Shared");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteArea(int id)
        {
            var area = DB.Areas.Find(id);
            DB.Areas.Remove(area);
            DB.SaveChanges();
            return Content("ok");
        }
    }
}