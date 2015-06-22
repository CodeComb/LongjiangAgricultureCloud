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
        /// <summary>
        /// 农业信息分类
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return RedirectToAction("Catalog", "Mobile", new { type = CatalogType.农业信息分类 });
        }

        /// <summary>
        /// 农业信息列表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult List(int id)
        {
            var catalog = DB.Catalogs.Find(id);
            ViewBag.Title = catalog.Title;
            return View();
        }

        /// <summary>
        /// 农业信息列表
        /// </summary>
        /// <param name="id"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public ActionResult ListRaw(int id, int p = 0)
        {
            var informations = (from i in DB.Informations
                                where i.CatalogID == id
                                orderby i.Time descending
                                select i).Skip(p * 20).Take(20).ToList();
            return View(informations);
        }

        /// <summary>
        /// 农业信息详情展示
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Show(int id)
        {
            var information = DB.Informations.Find(id);
            return View(information);
        }
    }
}