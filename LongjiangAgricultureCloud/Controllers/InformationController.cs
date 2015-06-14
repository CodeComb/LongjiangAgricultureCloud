using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using LongjiangAgricultureCloud.Models;
using LongjiangAgricultureCloud.Schema;
using LongjiangAgricultureCloud.Helpers;

namespace LongjiangAgricultureCloud.Controllers
{
    public class InformationController : BaseController
    {
        // GET: Information
        public ActionResult Index(int? CatalogID, DateTime? Begin, DateTime? End, int p = 0)
        {
            ViewBag.Level1 = (from c in DB.Catalogs
                              where c.Level == 0
                              && c.Type == CatalogType.农业信息分类
                              select c).ToList();
            ViewBag.Level2 = (from c in DB.Catalogs
                              where c.Level == 1
                              && c.Type == CatalogType.农业信息分类
                              select c).ToList();
            ViewBag.Level3 = (from c in DB.Catalogs
                              where c.Level == 2
                              && c.Type == CatalogType.农业信息分类
                              select c).ToList();
            IEnumerable<Information> query = DB.Informations.Where(x => x.Type == InformationType.农业信息);
            if (CatalogID.HasValue)
                query = query.Where(x => x.CatalogID == CatalogID.Value);
            if (Begin.HasValue)
                query = query.Where(x => x.Time >= Begin.Value);
            if (End.HasValue)
                query = query.Where(x => x.Time <= End.Value);
            query = query.OrderByDescending(x => x.Time);
            ViewBag.PageInfo = PagerHelper.Do(ref query, 50, p);
            return View(query);
        }

        public ActionResult Delete(int id)
        {
            var information = DB.Informations.Find(id);
            DB.Informations.Remove(information);
            DB.SaveChanges();
            return Content("ok");
        }

        public ActionResult Edit(int id)
        {
            var information = DB.Informations.Find(id);
            return View(information);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(string Title, string Description, int? CatalogID)
        {
            var Information = new Information
            {
                Title = Title,
                Description = Description,
                CatalogID = CatalogID,
                UserID = CurrentUser.ID,
                Time = DateTime.Now
            };

            DB.Informations.Add(Information);
            DB.SaveChanges();
            return RedirectToAction("Success", "Shared");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, string Title, string Description, int? CatalogID)
        {
            var information = DB.Informations.Find(id);
            information.Title = Title;
            information.Description = Description;
            information.CatalogID = CatalogID;
            DB.SaveChanges();
            return RedirectToAction("Success", "Shared");
        }
    }
}