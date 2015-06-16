using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.Mvc;

namespace LongjiangAgricultureCloud.Controllers
{
    public class SharedController : BaseController
    {
        // GET: Shared
        [Route("Login")]
        public ActionResult Login()
        {
            return View();
        }

        [Route("Message")]
        public ActionResult Message(string msg, string sid)
        {
            if (Session["sid"].ToString() != sid)
                return RedirectToAction("NoAccess", "Shared", null);
            ViewBag.Msg = msg;
            return View();
        }

        [Route("Login")]
        [ValidateAntiForgeryToken]
        [HttpPost]
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
                if (user.Role < Models.UserRole.大区经理)
                {
                    return RedirectToAction("NoAccess", "Shared");
                }
                FormsAuthentication.SetAuthCookie(Username, false);
                return Redirect(returnUrl ?? "/");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Shared");
        }

        public ActionResult Success()
        {
            return View();
        }

        public ActionResult NoAccess()
        {
            return View();
        }

        public ActionResult ProductImg(int id, int index)
        {
            var product = DB.Products.Find(id);
            switch (index)
            {
                case 1: return File(product.Picture1, "image/jpeg");
                case 2: return File(product.Picture2, "image/jpeg");
                case 3: return File(product.Picture3, "image/jpeg");
                case 4: return File(product.Picture4, "image/jpeg");
                case 5: return File(product.Picture5, "image/jpeg");
                default: return File(new byte[0], "image/jpeg");
            }
        }
    }
}