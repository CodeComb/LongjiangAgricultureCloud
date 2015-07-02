using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LongjiangAgricultureCloud.Models;

namespace LongjiangAgricultureCloud.Schema
{
    public class CheckRoleEqual : BaseAttribute
    {
        public UserRole _Role;

        public CheckRoleEqual(UserRole Role)
        {
            _Role = Role;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var tmp = base.AuthorizeCore(httpContext);
            if (!tmp) return false;
            if (CurrentUser.Role == _Role || CurrentUser.Role == UserRole.系统管理员) return true;
            else return false;
        }
    }
}