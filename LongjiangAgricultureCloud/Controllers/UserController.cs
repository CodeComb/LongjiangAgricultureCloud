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
    [CheckRoleEqual(UserRole.大区经理)]
    public class UserController : BaseController
    {
        /// <summary>
        /// 用户列表
        /// </summary>
        /// <param name="Username"></param>
        /// <param name="Role"></param>
        /// <param name="Name"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public ActionResult Index(string Username, UserRole? Role, string Name, int p = 0)
        {
            IEnumerable<User> query = DB.Users;
            if (!string.IsNullOrEmpty(Username))
                query = query.Where(x => x.Username == Username);
            if (!string.IsNullOrEmpty(Name))
                query = query.Where(x => x.Name.Contains(Name));
            if (Role.HasValue)
                query = query.Where(x => x.Role == Role.Value);
            if (CurrentUser.Role == UserRole.大区经理)
                query = query.Where(x => x.ManagerID == CurrentUser.ID);
            ViewBag.PageInfo = PagerHelper.Do(ref query, 50, p);
            return View(query);
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            var user = DB.Users.Find(id);
            if (user.ManagerID != CurrentUser.ID && CurrentUser.Role != UserRole.系统管理员)
                return RedirectToAction("NoAccess", "Shared");
            DB.Users.Remove(user);
            DB.SaveChanges();
            return Content("ok");
        }

        /// <summary>
        /// 编辑用户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(int id)
        {
            var user = DB.Users.Find(id);
            ViewBag.Managers = (from u in DB.Users
                                where u.Role == UserRole.大区经理
                                select u).ToList();
            if (user.ManagerID != CurrentUser.ID && CurrentUser.Role != UserRole.系统管理员)
                return RedirectToAction("NoAccess", "Shared");
            return View(user);
        }

        /// <summary>
        /// 编辑用户
        /// </summary>
        /// <param name="id"></param>
        /// <param name="User"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, User User)
        {
            var user = DB.Users.Find(id);
            if (user.Username != User.Username && DB.Users.Any(x => x.Username == User.Username))
                return Msg("已经存在用户名(手机号)为\"" + User.Username + "\"的用户，请修改后重试！");
            if (user.ManagerID != CurrentUser.ID && CurrentUser.Role != UserRole.系统管理员)
                return RedirectToAction("NoAccess", "Shared");
            user.Name = User.Name;
            user.AreaID = User.AreaID; //TODO: 选择地区
            user.Question = User.Question;
            user.Answer = User.Answer;
            user.PostCode = User.PostCode;
            if (CurrentUser.Role == UserRole.系统管理员)
                user.Role = User.Role;
            user.ManagerID = User.ManagerID;
            if (!string.IsNullOrEmpty(User.Password))
                user.Password = Security.SHA1(User.Password);
            user.AreaID = User.AreaID;
            user.Address = User.Address;
            DB.SaveChanges();
            return RedirectToAction("Success", "Shared");
        }

        /// <summary>
        /// 创建用户
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            ViewBag.Managers = (from u in DB.Users
                                where u.Role == UserRole.大区经理
                                select u).ToList();
            return View();
        }

        /// <summary>
        /// 创建用户
        /// </summary>
        /// <param name="User"></param>
        /// <param name="Confirm"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(User User, string Confirm)
        {
            if (DB.Users.Any(x => x.Username == User.Username))
                return Msg("已经存在用户名(手机号)为\"" + User.Username + "\"的用户，请修改后重试！");
            if (User.Password != Confirm)
                return Msg("两次密码输入不一致，请返回重新尝试！");
            if (CurrentUser.Role == UserRole.大区经理)
            {
                User.Role = UserRole.服务站;
                User.ManagerID = CurrentUser.ID;
            }
       
            User.Password = Security.SHA1(Confirm);
            DB.Users.Add(User);
            DB.SaveChanges();
            return RedirectToAction("Success", "Shared");
        }

        /// <summary>
        /// 地区管理
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [CheckRole(UserRole.系统管理员)]
        public ActionResult Area(int? id)
        {
            IEnumerable<Area> query = DB.Areas;
            if (id.HasValue)
                query = query.Where(x => x.FatherID == id.Value);
            else
                query = query.Where(x => x.FatherID == null && x.Level == AreaLevel.省);
            Area a = null;
            if (id.HasValue)
                a = DB.Areas.Find(id.Value);
            ViewBag.Area = a;
            return View(query.ToList());
        }

        /// <summary>
        /// 创建地区
        /// </summary>
        /// <param name="Title"></param>
        /// <param name="FatherID"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckRole(UserRole.系统管理员)]
        public ActionResult CreateArea(string Title, int? FatherID)
        {
            var area = new Area();
            area.Title = Title;
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
            if (DB.Areas.Any(x => x.Title == Title && x.Level == area.Level))
            {
                return Msg("请勿重复创建地区");
            }
            DB.Areas.Add(area);
            DB.SaveChanges();
            var lvl = Convert.ToInt32(area.Level) + 1;
            var id = area.ID;
            while (lvl != 6)
            {
                var a = new Area { Title = "-", FatherID = id, Level = (AreaLevel)lvl };
                DB.Areas.Add(a);
                DB.SaveChanges();
                lvl++;
                id = a.ID;
            }
            return RedirectToAction("Success", "Shared");
        }

        /// <summary>
        /// 删除地区
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckRole(UserRole.系统管理员)]
        public ActionResult DeleteArea(int id)
        {
            var area = DB.Areas.Find(id);
            DB.Areas.Remove(area);
            DB.SaveChanges();
            return Content("ok");
        }
    }
}