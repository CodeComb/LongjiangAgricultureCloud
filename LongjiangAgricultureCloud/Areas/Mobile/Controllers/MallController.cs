using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LongjiangAgricultureCloud.Models;
using LongjiangAgricultureCloud.Schema;
using Com.Alipay;
using System.Collections.Specialized;
using WxPayAPI;
using System.Text;

namespace LongjiangAgricultureCloud.Areas.Mobile.Controllers
{
    public class MallController : BaseController
    {
        /// <summary>
        /// 商品分类
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return RedirectToAction("Catalog", "Mobile", new { type = CatalogType.商品分类 });
        }

        /// <summary>
        /// 商品列表
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
        /// 商品列表
        /// </summary>
        /// <param name="id"></param>
        /// <param name="Title"></param>
        /// <param name="Key"></param>
        /// <param name="Desc"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public ActionResult ListRaw(int id, string Title, string Key, bool Desc = false, int p = 0)
        {
            IEnumerable<Product> products = DB.Products.Where(x => x.CatalogID == id && x.StoreCount > 0 && !x.Delete);
            if (!string.IsNullOrEmpty(Title))
                products = products.Where(x => x.Title.Contains(Title) || Title.Contains(x.Title));
            products = products.OrderByDescending(x => x.Top).ThenByDescending(x => x.ID);
            if (Key == "Price")
            {
                if (Desc)
                    products = products.OrderByDescending(x => x.Price);
                else
                    products = products.OrderBy(x => x.Price);
            }
            else if (Key == "Sale")
            {
                if (Desc)
                    products = products.OrderByDescending(x => x.OrderDetails.Sum(y => y.Count));
                else
                    products = products.OrderBy(x => x.OrderDetails.Sum(y => y.Count));
            }
            products = products.Skip(p * 20).Take(20).ToList();
            return View(products);
        }

        /// <summary>
        /// 商品展示
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Show(int id)
        {
            var product = DB.Products.Find(id);
            return View(product);
        }

        /// <summary>
        /// 购物车
        /// </summary>
        /// <param name="id"></param>
        /// <param name="Count"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MobileAuthorize]
        public ActionResult Cart(int id, int Count)
        {
            var product = DB.Products.Find(id);
            if (Count > product.StoreCount)
                return Msg("库存不足，无法加入购物车！");
            var OrderDetail = new OrderDetail
            {
                ID = Guid.NewGuid(),
                OrderID = null,
                ProductID = id,
                Price = product.ID,
                Count = Count,
                UserID = CurrentUser.ID
            };
            DB.OrderDetails.Add(OrderDetail);
            DB.SaveChanges();
            return Msg("该商品已经成功加入到购物车！");
        }

        /// <summary>
        /// 购物车
        /// </summary>
        /// <returns></returns>
        [MobileAuthorize]
        public ActionResult Cart()
        {
            var orders = (from od in DB.OrderDetails
                          where od.UserID == CurrentUser.ID
                          && od.OrderID == null
                          orderby od.ID descending
                          select od).ToList();
            foreach (var od in orders)
            {
                od.Price = od.Product.Price;
                if (od.Count > od.Product.StoreCount)
                    od.Count = od.Product.StoreCount;
            }
            DB.SaveChanges();
            return View(orders);
        }

        /// <summary>
        /// 移除购物车
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MobileAuthorize]
        public ActionResult RemoveCart(Guid id)
        {
            var od = DB.OrderDetails.Find(id);
            if (od.UserID != CurrentUser.ID)
                return Msg("非法操作！");
            DB.OrderDetails.Remove(od);
            DB.SaveChanges();
            return RedirectToAction("Cart", "Mall");
        }

        /// <summary>
        /// 支付
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [MobileAuthorize]
        public ActionResult Pay(Guid id)
        {
            var orders = (from od in DB.OrderDetails
                          where od.UserID == CurrentUser.ID
                          && od.OrderID == id
                          orderby od.ID descending
                          select od).ToList();
            foreach (var od in orders)
            {
                if (od.Count > od.Product.StoreCount)
                    od.Count = od.Product.StoreCount;
                od.Price = od.Product.Price * od.Count;
            }
            ViewBag.Price = orders.Sum(x => x.Price).ToString("0.00");
            ViewBag.OrderId = id;
            return View();
        }

        /// <summary>
        /// 结算
        /// </summary>
        /// <param name="Order"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MobileAuthorize]
        public ActionResult Buy(Order Order)
        {
            Order.ID = Guid.NewGuid();
            Order.UserID = CurrentUser.ID;
            Order.Time = DateTime.Now;
            Order.Status = OrderStatus.待付款;
            Order.PayMethod = PayMethod.支付宝;
            Order.PayCode = "";
            DB.Orders.Add(Order);
            DB.SaveChanges();
            var orders = (from od in DB.OrderDetails
                          where od.UserID == CurrentUser.ID
                          && od.OrderID == null
                          orderby od.ID descending
                          select od).ToList();
            foreach (var od in orders)
            {
                if (od.Count > od.Product.StoreCount)
                    od.Count = od.Product.StoreCount;
                od.Price = od.Product.Price * od.Count;
                od.Product.StoreCount -= od.Count;
                od.OrderID = Order.ID;
            }
            DB.SaveChanges();
            return RedirectToAction("Pay", "Mall", new { id = Order.ID });
        }

        /// <summary>
        /// 商品评论
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Comment(int id)
        {
            var product = DB.Products.Find(id);
            return View(product);
        }

        /// <summary>
        /// 商品评论
        /// </summary>
        /// <param name="id"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public ActionResult CommentRaw(int id, int p = 0)
        {
            var comments = (from c in DB.Comments
                            where c.Type == CommentType.商品评论
                            && c.TargetID == id
                            && c.Verify
                            orderby c.Time descending
                            select c).Skip(p * 20).Take(20).ToList();
            return View(comments);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public void Alipay(string aliOrderId, string aliPrice)
        {
            ////////////////////////////////////////////请求参数////////////////////////////////////////////

            //支付类型
            string payment_type = "1";
            //必填，不能修改
            //服务器异步通知页面路径
            string notify_url = "http://221.208.208.32:7532/Mobile/Mall/alipaynotify";
            //需http://格式的完整路径，不能加?id=123这类自定义参数

            //页面跳转同步通知页面路径
            string return_url = "http://221.208.208.32:7532/Mobile/Mall/alipayreturn";
            //需http://格式的完整路径，不能加?id=123这类自定义参数，不能写成http://localhost/

            //卖家支付宝帐户
            string seller_email = "qieaijiaren@163.com";
            //必填

            //商户订单号
            string out_trade_no = aliOrderId;
            //商户网站订单系统中唯一订单号，必填

            //订单名称
            string subject = "商品支付";
            //必填

            //付款金额
            string total_fee = aliPrice;
            //必填

            //订单描述

            string body = "商品支付";
            //商品展示地址
            string show_url = "http://221.208.208.32:7532/Mobile/";
            //需以http://开头的完整路径，例如：http://121.42.136.4:9003/Mobile

            //防钓鱼时间戳
            string anti_phishing_key = Submit.Query_timestamp();
            //若要使用请调用类文件submit中的query_timestamp函数

            //客户端的IP地址
            string exter_invoke_ip = "";
            //非局域网的外网IP地址，如：221.0.0.1

            ////////////////////////////////////////////////////////////////////////////////////////////////

            //把请求参数打包成数组
            SortedDictionary<string, string> sParaTemp = new SortedDictionary<string, string>();
            sParaTemp.Add("partner", Config.Partner);
            sParaTemp.Add("seller_id", Config.Seller_id);
            sParaTemp.Add("_input_charset", Config.Input_charset.ToLower());
            sParaTemp.Add("service", "alipay.wap.create.direct.pay.by.user");
            sParaTemp.Add("payment_type", payment_type);
            sParaTemp.Add("notify_url", notify_url);
            sParaTemp.Add("return_url", return_url);
            sParaTemp.Add("out_trade_no", out_trade_no);
            sParaTemp.Add("subject", subject);
            sParaTemp.Add("total_fee", total_fee);
            sParaTemp.Add("show_url", show_url);
            sParaTemp.Add("body", body);


            //建立请求
            string sHtmlText = Submit.BuildRequest(sParaTemp, "get", "确认");
            Response.Write(sHtmlText);
        }


        // 同步调用，只发生一次
        public ActionResult AlipayReturn()
        {
            var model = new AlipayReturnViewModel();
            SortedDictionary<string, string> sPara = GetRequestGet();

            if (sPara.Count > 0)//判断是否有带返回参数
            {

                if (Request.QueryString["trade_status"] == "TRADE_FINISHED" || Request.QueryString["trade_status"] == "TRADE_SUCCESS")//验证成功
                {
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    //请在这里加上商户的业务逻辑程序代码


                    //——请根据您的业务逻辑来编写程序（以下代码仅作参考）——
                    //获取支付宝的通知返回参数，可参考技术文档中页面跳转同步通知参数列表

                    //商户订单号
                    model.out_trade_no = Request.QueryString["out_trade_no"];

                    //支付宝交易号
                    model.trade_no = Request.QueryString["trade_no"];

                    //交易状态
                    model.trade_status = Request.QueryString["trade_status"];


                    model.message = ("支付成功");

                    Guid oid = new Guid(model.out_trade_no);
                    var orders = (from od in DB.OrderDetails
                                  where od.OrderID == oid
                                  orderby od.ID descending
                                  select od).ToList();

                    foreach (var od in orders)
                    {
                        od.Order.Status = OrderStatus.待发货;
                        od.Order.PayMethod = PayMethod.支付宝;
                        od.Order.PayTime = DateTime.Now;
                    }
                    DB.SaveChanges();
                }
                else//验证失败
                {
                    model.message = ("支付失败");
                }
            }
            else
            {
                model.message = ("无返回参数");
            }
            return View(model);
        }

        /// <summary>
        /// 获取支付宝GET过来通知消息，并以“参数名=参数值”的形式组成数组
        /// </summary>
        /// <returns>request回来的信息组成的数组</returns>
        public SortedDictionary<string, string> GetRequestGet()
        {
            int i = 0;
            SortedDictionary<string, string> sArray = new SortedDictionary<string, string>();
            NameValueCollection coll;
            //Load Form variables into NameValueCollection variable.
            coll = Request.QueryString;

            // Get names of all forms into a string array.
            String[] requestItem = coll.AllKeys;

            for (i = 0; i < requestItem.Length; i++)
            {
                sArray.Add(requestItem[i], Request.QueryString[requestItem[i]]);
            }

            return sArray;
        }
        
        public ActionResult Wxpay(string wxOrderId, double  wxPrice)
        {
            NativePay nativePay = new NativePay();

            string url = nativePay.GetPayUrl(wxOrderId, "购买商品", (wxPrice*100).ToString());
            ViewBag.Code = QRCode.Create(url);
            return View();
        }


        public void WxResult()
        {
            WxPayData notifyData = GetNotifyData();
            string _oid = notifyData.GetValue("attach").ToString();
            Guid oid = new Guid(_oid);
            Log.Info(this.GetType().ToString(), _oid);
            var orders = (from od in DB.OrderDetails
                          where od.OrderID == oid
                          orderby od.ID descending
                          select od).ToList();

            foreach (var od in orders)
            {
                od.Order.Status = OrderStatus.待发货;
                od.Order.PayMethod = PayMethod.微信支付;
                od.Order.PayTime = DateTime.Now;
            }
            DB.SaveChanges();
        }

        /// <summary>
        /// 接收从微信支付后台发送过来的数据并验证签名
        /// </summary>
        /// <returns>微信支付后台返回的数据</returns>
        public WxPayData GetNotifyData()
        {
            //接收从微信后台POST过来的数据
            System.IO.Stream s = Request.InputStream;
            int count = 0;
            byte[] buffer = new byte[1024];
            StringBuilder builder = new StringBuilder();
            while ((count = s.Read(buffer, 0, 1024)) > 0)
            {
                builder.Append(Encoding.UTF8.GetString(buffer, 0, count));
            }
            s.Flush();
            s.Close();
            s.Dispose();

            Log.Info(this.GetType().ToString(), "Receive data from WeChat : " + builder.ToString());

            //转换数据格式并验证签名
            WxPayData data = new WxPayData();
            try
            {
                data.FromXml(builder.ToString());
            }
            catch (WxPayException ex)
            {
                //若签名错误，则立即返回结果给微信支付后台
                WxPayData res = new WxPayData();
                res.SetValue("return_code", "FAIL");
                res.SetValue("return_msg", ex.Message);
                Log.Error(this.GetType().ToString(), "Sign check error : " + res.ToXml());
            }
            Log.Info(this.GetType().ToString(), "Check sign success");
            return data;
        }
    }
}