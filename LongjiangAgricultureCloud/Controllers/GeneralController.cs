using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using System.Web;
using System.Web.Mvc;
using System.IO;
using LongjiangAgricultureCloud.Models;
using LongjiangAgricultureCloud.Schema;
using LongjiangAgricultureCloud.Helpers;

namespace LongjiangAgricultureCloud.Controllers
{
    [CheckRole(Models.UserRole.系统管理员)]
    public class GeneralController : BaseController
    {
        // GET: General
        public ActionResult Index()
        {
            var config = new vConfig
            {
                InformationComment = ViewBag.InformationComment,
                ServiceTel = ViewBag.ServiceTel,
                VerifyLocalTong = ViewBag.VerifyLocalTong,
                VerifyLocalTongComment = ViewBag.VerifyLocalTongComment,
                VerifyProductComment = ViewBag.VerifyProductComment,
                VerifyService = ViewBag.VerifyService
            };
            return View(config);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Base(vConfig Config)
        {
            ConfigurationManager.AppSettings["VerifyProductComment"] = Config.VerifyProductComment ? "true" : "false";
            ConfigurationManager.AppSettings["VerifyService"] = Config.VerifyService ? "true" : "false";
            ConfigurationManager.AppSettings["VerifyLocalTong"] = Config.VerifyLocalTong ? "true" : "false";
            ConfigurationManager.AppSettings["VerifyLocalTongComment"] = Config.VerifyLocalTongComment ? "true" : "false";
            ConfigurationManager.AppSettings["InformationComment"] = Config.InformationComment ? "true" : "false";
            ConfigurationManager.AppSettings["ServiceTel"] = Config.ServiceTel;
            return RedirectToAction("Success", "Shared", null);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Pay(string AlipayAppKey, string WeixinPayAppKey)
        {
            ConfigurationManager.AppSettings["AlipayAppKey"] = AlipayAppKey;
            ConfigurationManager.AppSettings["WeixinPayAppKey"] = WeixinPayAppKey;
            return RedirectToAction("Success", "Shared", null);
        }
        
        public ActionResult Catalog(int? id)
        {
            if (id == null)
            {
                ViewBag.Level = 0;
                return View(DB.Catalogs.Where(x => x.Level == 0 && !x.Delete).OrderBy(x => x.Type).ToList());
            }
            else
            {
                var catalog = DB.Catalogs.Find(id.Value);
                ViewBag.Level = catalog.Level + 1;
                ViewBag.FatherID = catalog.ID;
                ViewBag.FatherTitle = catalog.Title;
                ViewBag.Type = catalog.Type;
                return View(DB.Catalogs.Where(x => x.FatherID == id.Value && !x.Delete).OrderBy(x => x.Type).ToList());
            }
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateCatalog(int? FatherID, string Title, CatalogType? Type)
        {
            var catalog = new Catalog();
            Catalog father = null;
            if (FatherID.HasValue)
            {
                father = DB.Catalogs.Find(FatherID.Value);
                catalog.FatherID = FatherID.Value;
                catalog.Level = father.Level + 1;
                catalog.Type = father.Type;
            }
            else
            {
                catalog.Level = 0;
                catalog.Type = Type.Value;
                catalog.FatherID = null;
            }
            catalog.Title = Title;
            DB.Catalogs.Add(catalog);
            DB.SaveChanges();
            return RedirectToAction("Success", "Shared");
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteCatalog(int id)
        {
            var catalog = DB.Catalogs.Find(id);
            catalog.Delete = true;
            DB.SaveChanges();
            return Content("ok");
        }
        
        public ActionResult Comment(CommentType? Type, DateTime? Begin, DateTime? End, int p = 1)
        {
            IEnumerable<Comment> query = DB.Comments;
            if (Begin.HasValue)
                query = query.Where(x => x.Time >= Begin.Value);
            if (End.HasValue)
                query = query.Where(x => x.Time <= End.Value);
            if (Type.HasValue)
                query = query.Where(x => x.Type == Type);
            ViewBag.PageInfo = PagerHelper.Do(ref query, 50, p);
            return View(query);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteComment(int id)
        {
            var comment = DB.Comments.Find(id);
            DB.Comments.Remove(comment);
            DB.SaveChanges();
            return Content("ok");
        }

        public ActionResult License()
        {
            ViewBag.Content = System.IO.File.ReadAllText(Server.MapPath("~/Files/License.html"));
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult License(string Content)
        {
            System.IO.File.WriteAllText(Server.MapPath("~/Files/License.html"), Content);
            return RedirectToAction("Success", "Shared");
        }
    }
}