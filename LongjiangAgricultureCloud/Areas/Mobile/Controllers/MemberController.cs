using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LongjiangAgricultureCloud.Schema;
using LongjiangAgricultureCloud.Models;
using LongjiangAgricultureCloud.Helpers;

namespace LongjiangAgricultureCloud.Areas.Mobile.Controllers
{
    [MobileAuthorize]
    public class MemberController : BaseController
    {
        // GET: Mobile/Member
        public ActionResult Index()
        {
            return View(CurrentUser);
        }

        public ActionResult Config()
        {
            return View(CurrentUser);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Config(User User, string Confirm)
        {
            CurrentUser.AreaID = User.AreaID;
            CurrentUser.Address = User.Address;
            CurrentUser.Name = User.Name;
            if (!string.IsNullOrEmpty(User.Password))
            {
                if (Confirm != User.Password)
                    return Msg("两次密码输入不一致，请返回重试！");
                CurrentUser.Password = Security.SHA1(Confirm);
            }
            DB.SaveChanges();
            return Msg("个人资料保存成功！");
        }
    }
}