using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LongjiangAgricultureCloud.Models;

namespace LongjiangAgricultureCloud.Areas.Mobile.Controllers
{
    public class MLocalController : BaseController
    {
        // GET: Mobile/MLocal
        public ActionResult Index()
        {
            return RedirectToAction("Catalog", "Mobile", new { type = CatalogType.本地通分类 });
        }

        public ActionResult List()
        {
            return View();
        }

        public ActionResult ListRaw(int p = 0)
        {
            var informations = (from i in DB.Informations
                                where i.Type == InformationType.本地通信息
                                && i.Verify == true
                                orderby i.Time descending
                                select i).ToList();
            return View(informations);
        }

        public ActionResult Show(int id)
        {
            var information = DB.Informations.Find(id);
            return View(information);
        }
    }
}