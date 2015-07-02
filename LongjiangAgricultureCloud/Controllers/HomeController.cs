using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LongjiangAgricultureCloud.Schema;

namespace LongjiangAgricultureCloud.Controllers
{
    [CheckRole(Models.UserRole.大区经理)]
    public class HomeController : BaseController
    {
        /// <summary>
        /// 后台首页
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }
    }
}