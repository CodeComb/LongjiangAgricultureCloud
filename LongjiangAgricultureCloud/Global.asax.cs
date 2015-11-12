using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Timers;

namespace LongjiangAgricultureCloud
{
    public class AppHandleErrorAttribute : HandleErrorAttribute
    {
        /// <summary>
        /// 异常
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnException(ExceptionContext filterContext)
        {
            //使用log4net或其他记录错误消息
            if (filterContext.HttpContext.Request.UrlReferrer.ToString().IndexOf("/Mobile") >= 0)
            {
                filterContext.ExceptionHandled = true;
                filterContext.Result = new RedirectResult("/Mobile/Mobile/Message?msg=非法请求！请返回重试！&sid="+ filterContext.HttpContext.Session["sid"].ToString());//跳转至错误提示页面
            }
            else
            {
                filterContext.ExceptionHandled = true;
                filterContext.Result = new RedirectResult("/Message?msg=非法请求！请返回重试！&sid=" + filterContext.HttpContext.Session["sid"].ToString());//跳转至错误提示页面
            }
        }
    }

    public class MvcApplication : System.Web.HttpApplication
    {
        public static string Path;
        protected void Application_Start()
        {
            GlobalFilters.Filters.Add(new AppHandleErrorAttribute());

            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            var timer = new Timer();
            timer.Interval = 1000 * 60 * 60 * 12;
            timer.Elapsed += Timer_Elapsed;
            timer.Enabled = true;
            ClearCart();
            timer.Start();
            Path = Server.MapPath("~/");
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            ClearCart();
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            Session["sid"] = Guid.NewGuid().ToString();
        }

        private void ClearCart()
        {
            using (var DB = new Models.LongjiangAgricultureCloudContext())
            {
                var orders = (from o in DB.Orders
                              where o.Status == Models.OrderStatus.待付款
                              select o).ToList();
                foreach (var o in orders)
                {
                    if (o.Time.AddDays(1) <= DateTime.Now)
                    {
                        o.Status = Models.OrderStatus.已取消;
                        foreach (var od in o.OrderDetails)
                        {
                            od.Product.StoreCount += od.Count;
                        }
                        DB.SaveChanges();
                    }
                }
            }
        }
    }
}
