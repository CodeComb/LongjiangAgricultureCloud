using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LongjiangAgricultureCloud.Models;

namespace LongjiangAgricultureCloud.Schema
{
    public class BaseAttribute : AuthorizeAttribute
    {
        public User CurrentUser;
        public readonly LongjiangAgricultureCloudContext DB = new LongjiangAgricultureCloudContext();

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (!httpContext.User.Identity.IsAuthenticated) return false;
            CurrentUser = (from u in DB.Users
                           where u.Username == httpContext.User.Identity.Name
                           select u).Single();
            return base.AuthorizeCore(httpContext);
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.User.Identity.IsAuthenticated)
                filterContext.HttpContext.Response.Redirect("/Shared/NoAccess");
            else
                base.HandleUnauthorizedRequest(filterContext);
        }
    }
}