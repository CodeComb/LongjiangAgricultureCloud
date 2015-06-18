using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LongjiangAgricultureCloud.Areas.Mobile.Controllers
{
    public class MallController : Controller
    {
        // GET: Mobile/Mall
        public ActionResult Index()
        {
            return RedirectToAction("Catalog", "Mobile", new { type = "商品分类" });
        }
    }
}