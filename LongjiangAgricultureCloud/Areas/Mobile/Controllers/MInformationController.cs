using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LongjiangAgricultureCloud.Models;

namespace LongjiangAgricultureCloud.Areas.Mobile.Controllers
{
    public class MInformationController : BaseController
    {
        // GET: Mobile/MInformation
        public ActionResult Index()
        {
            return RedirectToAction("Catalog", "Mobile", new { type = CatalogType.农业信息分类 });
        }

        public ActionResult List(int id)
        {
            var catalog = DB.Catalogs.Find(id);
            ViewBag.Title = catalog.Title;
            return View();
        }

        public ActionResult ListRaw(int id, int p = 0)
        {
            var informations = (from i in DB.Informations
                                where i.CatalogID == id
                                orderby i.Time descending
                                select i).Skip(p * 20).Take(20).ToList();
            return View(informations);
        }

        public ActionResult Show(int id)
        {
            var information = DB.Informations.Find(id);
            return View(information);
        }
    }
}