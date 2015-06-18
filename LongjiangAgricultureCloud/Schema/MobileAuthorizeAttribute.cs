using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LongjiangAgricultureCloud.Schema
{
    public class MobileAuthorizeAttribute : AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Request.UrlReferrer.ToString() == "")
                filterContext.HttpContext.Response.Redirect("/Mobile/Mobile/Login");
            else
                filterContext.HttpContext.Response.Redirect("/Mobile/Mobile/Login?returnUrl=" + HttpUtility.UrlEncode(filterContext.HttpContext.Request.UrlReferrer.ToString()));
        }
    }
}