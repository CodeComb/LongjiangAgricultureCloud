using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LongjiangAgricultureCloud.Schema;

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
    }
}