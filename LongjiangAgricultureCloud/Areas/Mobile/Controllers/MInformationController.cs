using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using LongjiangAgricultureCloud.Models;
using LongjiangAgricultureCloud.Schema;

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
        public ActionResult ListRaw(int id, string Title, int p = 0)
        {
            var informations = (from i in DB.Informations
                                where i.CatalogID == id
                                && (i.Title.Contains(Title) || Title.Contains(i.Title))
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
            ViewBag.Comments = (from c in DB.Comments
                                where c.TargetID == id
                                && c.Type == CommentType.农业信息评论
                                orderby c.Time ascending
                                select c).ToList();
            return View(information);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [MobileAuthorize]
        public ActionResult Comment(int id, string Content)
        {
            var comment = new Comment
            {
                ID = Guid.NewGuid(),
                Type = CommentType.农业信息评论,
                TargetID = id,
                Time = DateTime.Now,
                Content = Content,
                Verify = ViewBag.VerifyInformationComment ? false : true,
                UserID = CurrentUser.ID
            };
            var Video = Request.Files["Video"];
            if (Video.ContentLength > 0)
            {
                 var fname = Guid.NewGuid().ToString().Replace("-", "") + ((string.IsNullOrEmpty(Path.GetExtension(Video.FileName)) && Video.ContentType.IndexOf("image") < 0) ? ".3gp" : Path.GetExtension(Video.FileName));
                 Video.SaveAs(Server.MapPath("~/Files/Video/" + fname));
                if (Path.GetExtension(Video.FileName) == ".3gp")
                {
                    var destname = Guid.NewGuid().ToString().Replace("-", "") + ".mp4";
                    Helpers.Video.ChangeFilePhy(Server.MapPath("~/Files/Video/" + fname), Server.MapPath("~/Files/Video/" + destname), "480", "320");
                    comment.VideoURL = destname;
                }
                else
                {
                    comment.VideoURL = fname;
                }
            }
            DB.Comments.Add(comment);
            DB.SaveChanges();
            return Msg("评论发表成功");
        }
    }
}