using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LongjiangAgricultureCloud.Models;

namespace LongjiangAgricultureCloud.Schema
{
    public class CheckRoleAttribute : BaseAttribute
    {
        public UserRole _Role;

        public CheckRoleAttribute(UserRole Role)
        {
            _Role = Role;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var tmp = base.AuthorizeCore(httpContext);
            if (!tmp) return false;
            if (CurrentUser.Role >= _Role) return true;
            else return false;
        }
    }
}