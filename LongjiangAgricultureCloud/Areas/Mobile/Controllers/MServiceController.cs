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
        /// <summary>
        /// 农机服务首页
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 分类列表
        /// </summary>
        /// <param name="Machine"></param>
        /// <returns></returns>
        public ActionResult Service(bool Machine)
        {
            if (Machine)
                return RedirectToAction("Catalog", "Mobile", new { type = CatalogType.农机服务分类, Service = InformationType.农机找活 });
            else
                return RedirectToAction("Catalog", "Mobile", new { type = CatalogType.农机服务分类, Service = InformationType.土地找机手 });
        }
        
        /// <summary>
        /// 农机服务信息列表
        /// </summary>
        /// <param name="id"></param>
        /// <param name="Service"></param>
        /// <returns></returns>
        public ActionResult List(int id, InformationType Service)
        {
            var time = DateTime.Now;
            time = time.AddDays(-7);
            var informations = (from i in DB.Informations
                                where i.CatalogID == id
                                && i.Type == Service
                                && i.Time >= time
                                && i.Verify == true
                                orderby i.Time descending
                                select i).ToList();
            ViewBag.Title = Service.ToString();
            return View(informations);
        }

        /// <summary>
        /// 农机服务信息展示
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Show(int id)
        {
            var information = DB.Informations.Find(id);
            ViewBag.Title = information.Type.ToString();
            return View(information);
        }

        /// <summary>
        /// 维修站
        /// </summary>
        /// <returns></returns>
        public ActionResult Station()
        {
            return View();
        }

        /// <summary>
        /// 获取维修站
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetStations()
        {
            var informations = (from i in DB.Informations
                                where i.Type == InformationType.维修站
                                && i.Verify == true
                                select new
                                {
                                    ID = i.ID,
                                    Title = i.Title,
                                    Lon = i.Lon,
                                    Lat = i.Lat
                                }).ToList();
            return Json(informations, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 展示维修站
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult ShowStation(int id)
        {
            var station = DB.Informations.Find(id);
            return View(station);
        }

        /// <summary>
        /// 展示农手
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult ShowFammer(int id)
        {
            var fammer = DB.Informations.Find(id);
            return View(fammer);
        }

        /// <summary>
        /// 二手农机列表
        /// </summary>
        /// <returns></returns>
        public ActionResult Machine()
        {
            return View();
        }

        /// <summary>
        /// 附近农手列表
        /// </summary>
        /// <returns></returns>
        public ActionResult Fammer()
        {
            return View();
        }

        /// <summary>
        /// 附近农手
        /// </summary>
        /// <returns></returns>
        public ActionResult FammerRaw()
        {
            var fammers = (from i in DB.Informations
                           where i.Type == InformationType.附近农手
                           orderby i.Time descending
                           select new
                           {
                               ID = i.ID,
                               Title = i.Title,
                               Lon = i.Lon,
                               Lat = i.Lat
                           }).Take(100).ToList();
            return Json(fammers, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 二手农机
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public ActionResult MachineRaw(int p = 0)
        {
            var informations = (from i in DB.Informations
                                where i.Type == InformationType.二手农机
                                && i.Verify == true
                                orderby i.Time descending
                                select i).Skip(20 * p).Take(20).ToList();
            return View(informations);
        }

        /// <summary>
        /// 展示二手农机
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult ShowMachine(int id)
        {
            var information = DB.Informations.Find(id);
            return View(information);
        }
    }
}