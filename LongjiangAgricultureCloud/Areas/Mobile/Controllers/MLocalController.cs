using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LongjiangAgricultureCloud.Schema;
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

        public ActionResult ListRaw(int id, int p = 0)
        {
            var informations = (from i in DB.Informations
                                where i.Type == InformationType.本地通信息
                                && i.Verify == true
                                && i.CatalogID == id
                                orderby i.Time descending
                                select i).ToList();
            return View(informations);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [MobileAuthorize]
        public ActionResult Comment(int id, string Content)
        {
            var comment = new Comment
            {
                Type = CommentType.本地通评论,
                TargetID = id,
                Time = DateTime.Now,
                Content = Content,
                Verify = ViewBag.VerifyLocalTongComment ? false : true
            };
            DB.Comments.Add(comment);
            DB.SaveChanges();
            return Msg("评论发表成功");
        }

        public ActionResult Show(int id)
        {
            var information = DB.Informations.Find(id);
            var comments = (from c in DB.Comments
                            where c.Type == CommentType.本地通评论
                            && c.TargetID == id
                            && c.Verify
                            orderby c.Time descending
                            select c).Take(10).ToList();
            ViewBag.Comments = comments;
            return View(information);
        }
    }
}