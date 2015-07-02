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
    [CheckRoleEqual(UserRole.信息审核员)]
    public class InformationController : BaseController
    {
        /// <summary>
        /// 农业信息列表
        /// </summary>
        /// <param name="CatalogID"></param>
        /// <param name="Begin"></param>
        /// <param name="End"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public ActionResult Index(int? CatalogID, DateTime? Begin, DateTime? End, int p = 0)
        {
            ViewBag.Level1 = (from c in DB.Catalogs
                              where c.Level == 0
                              && !c.Delete
                              && c.Type == CatalogType.农业信息分类
                              select c).ToList();
            ViewBag.Level2 = (from c in DB.Catalogs
                              where c.Level == 1
                              && !c.Delete
                              && c.Type == CatalogType.农业信息分类
                              select c).ToList();
            ViewBag.Level3 = (from c in DB.Catalogs
                              where c.Level == 2
                              && !c.Delete
                              && c.Type == CatalogType.农业信息分类
                              select c).ToList();
            ViewBag.Level4 = (from c in DB.Catalogs
                              where c.Level == 3
                              && !c.Delete
                              && c.Type == CatalogType.农业信息分类
                              select c).ToList();
            IEnumerable<Information> query = DB.Informations.Where(x => x.Type == InformationType.农业信息);
            if (CatalogID.HasValue)
                query = query.Where(x => x.CatalogID == CatalogID.Value);
            if (Begin.HasValue)
                query = query.Where(x => x.Time >= Begin.Value);
            if (End.HasValue)
                query = query.Where(x => x.Time <= End.Value);
            query = query.OrderByDescending(x => x.Top).ThenByDescending(x => x.Time);
            ViewBag.PageInfo = PagerHelper.Do(ref query, 50, p);
            return View(query);
        }

        /// <summary>
        /// 删除农业信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            var information = DB.Informations.Find(id);
            DB.Informations.Remove(information);
            DB.SaveChanges();
            return Content("ok");
        }

        /// <summary>
        /// 编辑农业信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(int id)
        {
            ViewBag.Level1 = (from c in DB.Catalogs
                              where c.Level == 0
                              && !c.Delete
                              && c.Type == CatalogType.农业信息分类
                              select c).ToList();
            ViewBag.Level2 = (from c in DB.Catalogs
                              where c.Level == 1
                              && !c.Delete
                              && c.Type == CatalogType.农业信息分类
                              select c).ToList();
            ViewBag.Level3 = (from c in DB.Catalogs
                              where c.Level == 2
                              && !c.Delete
                              && c.Type == CatalogType.农业信息分类
                              select c).ToList();
            ViewBag.Level4 = (from c in DB.Catalogs
                              where c.Level == 3
                              && !c.Delete
                              && c.Type == CatalogType.农业信息分类
                              select c).ToList();
            var information = DB.Informations.Find(id);
            return View(information);
        }

        /// <summary>
        /// 创建农业信息
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            ViewBag.Level1 = (from c in DB.Catalogs
                              where c.Level == 0
                              && !c.Delete
                              && c.Type == CatalogType.农业信息分类
                              select c).ToList();
            ViewBag.Level2 = (from c in DB.Catalogs
                              where c.Level == 1
                              && !c.Delete
                              && c.Type == CatalogType.农业信息分类
                              select c).ToList();
            ViewBag.Level3 = (from c in DB.Catalogs
                              where c.Level == 2
                              && !c.Delete
                              && c.Type == CatalogType.农业信息分类
                              select c).ToList();
            ViewBag.Level4 = (from c in DB.Catalogs
                              where c.Level == 3
                              && !c.Delete
                              && c.Type == CatalogType.农业信息分类
                              select c).ToList();
            return View();
        }

        /// <summary>
        /// 创建农业信息
        /// </summary>
        /// <param name="Title"></param>
        /// <param name="Description"></param>
        /// <param name="CatalogID"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create(string Title, string Description, int? CatalogID)
        {
           
            var Information = new Information
            {
                Title = Title,
                Description = Description,
                CatalogID = CatalogID,
                UserID = CurrentUser.ID,
                Time = DateTime.Now,
                Verify = true,
                Type = InformationType.农业信息
            };

            var Video = Request.Files["Video"];
            if (Video != null && Video.ContentLength > 0)
            {
                var fname = Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(Video.FileName);
                Video.SaveAs(Server.MapPath("~/Files/Video/" + fname));
                Information.VideoURL = fname;
            }

            DB.Informations.Add(Information);
            DB.SaveChanges();
            return RedirectToAction("Success", "Shared");
        }

        /// <summary>
        /// 编辑农业信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="Title"></param>
        /// <param name="Description"></param>
        /// <param name="CatalogID"></param>
        /// <param name="Top"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(int id, string Title, string Description, int? CatalogID, int Top, bool Recommend)
        {
            var information = DB.Informations.Find(id);
            information.Title = Title;
            information.Description = Description;
            information.CatalogID = CatalogID;
            information.Top = Top;
            information.Recommend = Recommend;

            var Video = Request.Files["Video"];
            if (Video != null && Video.ContentLength > 0)
            {
                var fname = Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(Video.FileName);
                Video.SaveAs(Server.MapPath("~/Files/Video/" + fname));
                information.VideoURL = fname;
            }
            DB.SaveChanges();
            return RedirectToAction("Success", "Shared");
        }
    }
}