using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LongjiangAgricultureCloud.Models;

namespace LongjiangAgricultureCloud.Areas.Mobile.Controllers
{
    public class MServiceController : BaseController
    {
        // GET: Mobile/MService
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Service(bool Machine)
        {
            if (Machine)
                return RedirectToAction("Catalog", "Mobile", new { type = CatalogType.农机服务分类, Service = InformationType.农机找活 });
            else
                return RedirectToAction("Catalog", "Mobile", new { type = CatalogType.农机服务分类, Service = InformationType.土地找机手 });
        }

        public ActionResult List(int id, InformationType Service)
        {
            var time = DateTime.Now;
            time = time.AddDays(-7);
            var informations = (from i in DB.Informations
                                where i.CatalogID == id
                                && i.Type == Service
                                && i.Time >= time
                                orderby i.Time descending
                                select i).ToList();
            ViewBag.Title = Service.ToString();
            return View(informations);
        }

        public ActionResult Show(int id)
        {
            var information = DB.Informations.Find(id);
            ViewBag.Title = information.Type.ToString();
            return View(information);
        }
    }
}