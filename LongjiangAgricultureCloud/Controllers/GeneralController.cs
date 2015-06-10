﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using System.Web;
using System.Web.Mvc;
using LongjiangAgricultureCloud.Schema;

namespace LongjiangAgricultureCloud.Controllers
{
    [CheckRole(Models.UserRole.大区经理)]
    public class GeneralController : BaseController
    {
        // GET: General
        public ActionResult Index()
        {
            ViewBag.VerifyProductComment = Convert.ToBoolean(ConfigurationManager.AppSettings["VerifyProductComment"]);
            ViewBag.VerifyService = Convert.ToBoolean(ConfigurationManager.AppSettings["VerifyService"]);
            ViewBag.VerifyLocalTong = Convert.ToBoolean(ConfigurationManager.AppSettings["VerifyLocalTong"]);
            ViewBag.AlipayAppKey = ConfigurationManager.AppSettings["AlipayAppKey"];
            ViewBag.WeixinPayAppKey = ConfigurationManager.AppSettings["WeixinPayAppKey"];
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Base(bool VerifyProductComment, bool VerifyService, bool VerifyLocalTong)
        {
            ConfigurationManager.AppSettings["VerifyProductComment"] = VerifyProductComment.ToString();
            ConfigurationManager.AppSettings["VerifyService"] = VerifyService.ToString();
            ConfigurationManager.AppSettings["VerifyLocalTong"] = VerifyLocalTong.ToString();
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
        
        public ActionResult Catalog()
        {
            return View();
        }

        public ActionResult Comment()
        {
            return View();
        }
    }
}