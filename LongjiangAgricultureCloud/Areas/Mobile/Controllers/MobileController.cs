using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using LongjiangAgricultureCloud.Models;

namespace LongjiangAgricultureCloud.Areas.Mobile.Controllers
{
    public class MobileController : BaseController
    {
        // GET: Mobile/Mobile
        public ActionResult Message(string sid, string msg)
        {
            if (Session["sid"].ToString() != sid)
                msg = "您无权执行本操作！";
            ViewBag.Msg = msg;
            return View();
        }

        public ActionResult Index()
        {
            ViewBag.Informations = (from i in DB.Informations
                                    where i.Type == LongjiangAgricultureCloud.Models.InformationType.农业信息
                                    && i.Top == true
                                    orderby i.Time descending
                                    select i).ToList();
            return View();
        }

        public ActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "User");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string Username, string Password, string returnUrl)
        {
            var pwd = Helpers.Security.SHA1(Password);
            System.Diagnostics.Debug.WriteLine(pwd);
            var user = (from u in DB.Users
                        where u.Username == Username
                        && u.Password == pwd
                        select u).SingleOrDefault();
            if (user == null)
            {
                return Msg("用户名或密码错误！");
            }
            else
            {
                FormsAuthentication.SetAuthCookie(Username, false);
                return Redirect(returnUrl ?? "/");
            }
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(User User, string Confirm)
        {
            User.Role = UserRole.普通用户;
            if (User.Password != Confirm)
                return Msg("两次密码输入不一致！");
        }
    }
}