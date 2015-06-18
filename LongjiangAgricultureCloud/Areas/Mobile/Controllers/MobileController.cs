using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using LongjiangAgricultureCloud.Models;
using LongjiangAgricultureCloud.Helpers;

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
                if (string.IsNullOrEmpty(returnUrl))
                    return Redirect(Request.UrlReferrer == null ? "/Mobile/Member" : Request.UrlReferrer.ToString());
                else
                    return Redirect(returnUrl);
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
            if (DB.Users.Any(x => x.Username == User.Username))
                return Msg("该手机号码已存在，请修改后重新尝试注册！");
            User.Password = Security.SHA1(Confirm);
            DB.Users.Add(User);
            DB.SaveChanges();
            return Msg("注册成功！您可以使用该账号进行登录了！");
        }

        public ActionResult Catalog(string type, int? id)
        {
            CatalogType Type;
            Enum.TryParse(type, out Type);

            if (id.HasValue)
            {
                var catalog = DB.Catalogs.Find(id);
                if (catalog.Type == CatalogType.商品分类 && catalog.Level == 2)
                    return RedirectToAction("Index", "Mall", new { cid = catalog.ID });
                else if (catalog.Type == CatalogType.农业信息分类 && catalog.Level == 2)
                    return RedirectToAction("Index", "MInformation", new { cid = catalog.ID });
                else if (catalog.Type == CatalogType.农机服务分类 && catalog.Level == 3)
                    return RedirectToAction("Index", "MService", new { cid = catalog.ID });
                else if (catalog.Type == CatalogType.本地通分类 && catalog.Level == 2)
                    return RedirectToAction("Index", "MLocal", new { cid = catalog.ID });
                else
                {
                    if (catalog.Type == CatalogType.商品分类)
                        ViewBag.ShopNav = true;
                    ViewBag.Title = catalog.Title;
                    return View(catalog.Catalogs.ToList());
                }
            }
            else
            {
                if (Type == CatalogType.商品分类)
                    ViewBag.ShopNav = true;
                ViewBag.Title = Type.ToString();
                return View(DB.Catalogs.Where(x => x.Type == Type).ToList());
            }
        }
    }
}