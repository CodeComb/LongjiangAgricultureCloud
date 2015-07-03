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
        /// <summary>
        /// 基本信息设置
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var config = new vConfig
            {
                InformationComment = ViewBag.InformationComment,
                ServiceTel = ViewBag.ServiceTel,
                VerifyLocalTong = ViewBag.VerifyLocalTong,
                VerifyLocalTongComment = ViewBag.VerifyLocalTongComment,
                VerifyProductComment = ViewBag.VerifyProductComment,
                VerifyService = ViewBag.VerifyService,
                VerifyInformationComment = ViewBag.VerifyInformationComment
            };
            return View(config);
        }

        /// <summary>
        /// 基本信息设置
        /// </summary>
        /// <param name="Config"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Base(vConfig Config)
        {
            ConfigurationManager.AppSettings["VerifyProductComment"] = Config.VerifyProductComment ? "true" : "false";
            ConfigurationManager.AppSettings["VerifyService"] = Config.VerifyService ? "true" : "false";
            ConfigurationManager.AppSettings["VerifyLocalTong"] = Config.VerifyLocalTong ? "true" : "false";
            ConfigurationManager.AppSettings["VerifyLocalTongComment"] = Config.VerifyLocalTongComment ? "true" : "false";
            ConfigurationManager.AppSettings["InformationComment"] = Config.InformationComment ? "true" : "false";
            ConfigurationManager.AppSettings["VerifyInformationComment"] = Config.VerifyInformationComment ? "true" : "false";
            ConfigurationManager.AppSettings["ServiceTel"] = Config.ServiceTel;
            return RedirectToAction("Success", "Shared", null);
        }

        /// <summary>
        /// 支付信息设置
        /// </summary>
        /// <param name="AlipayAppKey"></param>
        /// <param name="WeixinPayAppKey"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Pay(string AlipayAppKey, string WeixinPayAppKey)
        {
            ConfigurationManager.AppSettings["AlipayAppKey"] = AlipayAppKey;
            ConfigurationManager.AppSettings["WeixinPayAppKey"] = WeixinPayAppKey;
            return RedirectToAction("Success", "Shared", null);
        }

        /// <summary>
        /// 分类设置
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Catalog(int? id)
        {
            ViewBag.Info = false;
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
                ViewBag.Info = catalog.Type == CatalogType.农业信息分类;
                return View(DB.Catalogs.Where(x => x.FatherID == id.Value && !x.Delete).OrderBy(x => x.Type).ToList());
            }
        }

        /// <summary>
        /// 创建分类
        /// </summary>
        /// <param name="FatherID"></param>
        /// <param name="Title"></param>
        /// <param name="Type"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateCatalog(int? FatherID, string Title, CatalogType? Type, bool Commentable = true)
        {
            var catalog = new Catalog();
            Catalog father = null;
            if (FatherID.HasValue)
            {
                father = DB.Catalogs.Find(FatherID.Value);
                catalog.FatherID = FatherID.Value;
                catalog.Level = father.Level + 1;
                catalog.Type = father.Type;
                catalog.Commentable = Commentable;
            }
            else
            {
                catalog.Level = 0;
                catalog.Type = Type.Value;
                catalog.FatherID = null;
            }
            catalog.Title = Title;
            if (DB.Catalogs.Any(x => x.Type == catalog.Type && catalog.FatherID == x.FatherID && catalog.Title == x.Title && !x.Delete))
            {
                return Msg("请勿创建名称相同的分类！");
            }
            DB.Catalogs.Add(catalog);
            DB.SaveChanges();
            return RedirectToAction("Success", "Shared");
        }

        /// <summary>
        /// 删除分类
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteCatalog(int id)
        {
            var catalog = DB.Catalogs.Find(id);
            catalog.Delete = true;
            DB.SaveChanges();
            return Content("ok");
        }

        /// <summary>
        /// 评论管理
        /// </summary>
        /// <param name="Type"></param>
        /// <param name="Begin"></param>
        /// <param name="End"></param>
        /// <param name="Verify"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public ActionResult Comment(CommentType? Type, DateTime? Begin, DateTime? End, int? Verify, int p = 1)
        {
            IEnumerable<Comment> query = DB.Comments;
            if (Begin.HasValue)
                query = query.Where(x => x.Time >= Begin.Value);
            if (End.HasValue)
                query = query.Where(x => x.Time <= End.Value);
            if (Type.HasValue)
                query = query.Where(x => x.Type == Type);
            if (Verify.HasValue)
                query = query.Where(x => x.Verify == (Verify.Value == 1 ? true : false));
            ViewBag.PageInfo = PagerHelper.Do(ref query, 50, p);
            return View(query);
        }

        /// <summary>
        /// 删除评论
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteComment(Guid id)
        {
            var comment = DB.Comments.Find(id);
            DB.Comments.Remove(comment);
            DB.SaveChanges();
            return Content("ok");
        }

        /// <summary>
        /// 服务条款管理
        /// </summary>
        /// <returns></returns>
        public ActionResult License()
        {
            ViewBag.Content = System.IO.File.ReadAllText(Server.MapPath("~/Files/License.html"));
            return View();
        }

        /// <summary>
        /// 服务条款管理
        /// </summary>
        /// <param name="Content"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult License(string Content)
        {
            System.IO.File.WriteAllText(Server.MapPath("~/Files/License.html"), Content);
            return RedirectToAction("Success", "Shared");
        }

        /// <summary>
        /// 审核评论
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult VerifyComment(Guid id)
        {
            var comment = DB.Comments.Find(id);
            comment.Verify = true;
            DB.SaveChanges();
            return Content("ok");
        }

        /// <summary>
        /// 编辑分类
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult RenameCatalog(int id)
        {
            var catalog = DB.Catalogs.Find(id);
            return View(catalog);
        }

        /// <summary>
        /// 编辑分类
        /// </summary>
        /// <param name="id"></param>
        /// <param name="Title"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RenameCatalog(int id, string Title, bool Commentable = true)
        {
            var catalog = DB.Catalogs.Find(id);
            catalog.Title = Title;
            catalog.Commentable = Commentable;
            DB.SaveChanges();
            return RedirectToAction("Success", "Shared");
        }
    }
}