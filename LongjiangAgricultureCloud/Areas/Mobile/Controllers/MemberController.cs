using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LongjiangAgricultureCloud.Schema;
using LongjiangAgricultureCloud.Models;

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
        public ActionResult Config(User User)
        {
            CurrentUser.AreaID = User.AreaID;
            CurrentUser.Address = User.Address;
            CurrentUser.Name = User.Name;
        }
    }
}