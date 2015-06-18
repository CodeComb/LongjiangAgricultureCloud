using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LongjiangAgricultureCloud.Areas.Mobile.Controllers
{
    public class MobileController : BaseController
    {
        // GET: Mobile/Mobile
        public ActionResult Index()
        {
            ViewBag.Informations = (from i in DB.Informations
                                    where i.Type == LongjiangAgricultureCloud.Models.InformationType.农业信息
                                    && i.Top == true
                                    orderby i.Time descending
                                    select i).ToList();
            return View();
        }
    }
}