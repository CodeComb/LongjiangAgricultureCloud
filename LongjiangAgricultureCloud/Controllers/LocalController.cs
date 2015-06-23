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
    public class LocalController : BaseController
    {
        /// <summary>
        /// 本地通信息列表
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
                              && c.Type == CatalogType.本地通分类
                              select c).ToList();
            ViewBag.Level2 = (from c in DB.Catalogs
                              where c.Level == 1
                              && !c.Delete
                              && c.Type == CatalogType.本地通分类
                              select c).ToList();
            ViewBag.Level3 = (from c in DB.Catalogs
                              where c.Level == 2
                              && !c.Delete
                              && c.Type == CatalogType.本地通分类
                              select c).ToList();
            IEnumerable<Information> query = DB.Informations.Where(x => x.Type == InformationType.本地通信息);
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

        /// <summary>
        /// 删除本地通信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Delete(int id)
        {
            var information = DB.Informations.Find(id);
            DB.Informations.Remove(information);
            DB.SaveChanges();
            return Content("ok");
        }

        /// <summary>
        /// 编辑本地通信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(int id)
        {
            ViewBag.Level1 = (from c in DB.Catalogs
                              where c.Level == 0
                              && !c.Delete
                              && c.Type == CatalogType.本地通分类
                              select c).ToList();
            ViewBag.Level2 = (from c in DB.Catalogs
                              where c.Level == 1
                              && !c.Delete
                              && c.Type == CatalogType.本地通分类
                              select c).ToList();
            ViewBag.Level3 = (from c in DB.Catalogs
                              where c.Level == 2
                              && !c.Delete
                              && c.Type == CatalogType.本地通分类
                              select c).ToList();
            var information = DB.Informations.Find(id);
            return View(information);
        }

        /// <summary>
        /// 创建本地通信息
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            ViewBag.Level1 = (from c in DB.Catalogs
                              where c.Level == 0
                              && !c.Delete
                              && c.Type == CatalogType.本地通分类
                              select c).ToList();
            ViewBag.Level2 = (from c in DB.Catalogs
                              where c.Level == 1
                              && !c.Delete
                              && c.Type == CatalogType.本地通分类
                              select c).ToList();
            ViewBag.Level3 = (from c in DB.Catalogs
                              where c.Level == 2
                              && !c.Delete
                              && c.Type == CatalogType.本地通分类
                              select c).ToList();
            return View();
        }

        /// <summary>
        /// 创建本地通信息
        /// </summary>
        /// <param name="Title"></param>
        /// <param name="Description"></param>
        /// <param name="CatalogID"></param>
        /// <param name="Name"></param>
        /// <param name="Phone"></param>
        /// <param name="Address"></param>
        /// <param name="Price"></param>
        /// <param name="SupplyDemand"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create(string Title, string Description, int? CatalogID, string Name, string Phone, string Address, string  Price, SupplyDemand SupplyDemand)
        {
            var Information = new Information
            {
                Title = Title,
                Description = Description,
                Name = Name,
                Phone = Phone,
                Address = Address,
                CatalogID = CatalogID,
                UserID = CurrentUser.ID,
                Time = DateTime.Now,
                SupplyDemand = SupplyDemand,
                Type = InformationType.本地通信息,
                Verify = true,
                Price = Price
            };
            
            DB.Informations.Add(Information);
            DB.SaveChanges();
            return RedirectToAction("Success", "Shared");
        }

        /// <summary>
        /// 编辑本地通信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="Title"></param>
        /// <param name="Description"></param>
        /// <param name="CatalogID"></param>
        /// <param name="Name"></param>
        /// <param name="Phone"></param>
        /// <param name="Address"></param>
        /// <param name="Price"></param>
        /// <param name="SupplyDemand"></param>
        /// <param name="Top"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(int id, string Title, string Description, int? CatalogID, string Name, string Phone, string Address, string Price, SupplyDemand SupplyDemand, bool Top)
        {
            var information = DB.Informations.Find(id);
            information.Title = Title;
            information.Description = Description;
            information.CatalogID = CatalogID;
            information.Name = Name;
            information.Phone = Phone;
            information.Top = Top;
            information.Address = Address;
            information.SupplyDemand = SupplyDemand;
            information.Price = Price;
            DB.SaveChanges();
            return RedirectToAction("Success", "Shared");
        }

        /// <summary>
        /// 审核本地通信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Verify(int id)
        {
            var information = DB.Informations.Find(id);
            return View(information);
        }

        /// <summary>
        /// 审核本地通信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="Verify"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Verify(int id, bool Verify = true)
        {
            var information = DB.Informations.Find(id);
            information.Verify = true;
            DB.SaveChanges();
            return RedirectToAction("Success", "Shared");
        }
    }
}