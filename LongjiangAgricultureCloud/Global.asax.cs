using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Timers;

namespace LongjiangAgricultureCloud
{
    public class MvcApplication : System.Web.HttpApplication
    {
        public static string Path;
        protected void Application_Start()
        {
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
