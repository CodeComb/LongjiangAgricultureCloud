using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LongjiangAgricultureCloud.Models;
using LongjiangAgricultureCloud.Schema;

namespace LongjiangAgricultureCloud.Areas.Mobile.Controllers
{
    public class MallController : BaseController
    {
        // GET: Mobile/Mall
        public ActionResult Index()
        {
            return RedirectToAction("Catalog", "Mobile", new { type = CatalogType.商品分类 });
        }

        public ActionResult List(int id)
        {
            var catalog = DB.Catalogs.Find(id);
            ViewBag.Title = catalog.Title;
            return View();
        }

        public ActionResult ListRaw(int id, string Title, string Key, bool Desc = false, int p = 0)
        {
            IEnumerable<Product> products = DB.Products.Where(x => x.CatalogID == id && x.StoreCount > 0);
            if (!string.IsNullOrEmpty(Title))
                products = products.Where(x => x.Title.Contains(Key) || Key.Contains(x.Title));
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

        public ActionResult Show(int id)
        {
            var product = DB.Products.Find(id);
            return View(product);
        }

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RemoveCart(Guid id)
        {
            var od = DB.OrderDetails.Find(id);
            if (od.UserID != CurrentUser.ID)
                return Msg("非法操作！");
            DB.OrderDetails.Remove(od);
            DB.SaveChanges();
            return RedirectToAction("Cart", "Mall");
        }

        public ActionResult Buy()
        {
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
            }
            ViewBag.Price = orders.Sum(x => x.Price);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Buy(Order Order)
        {
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
            }
            DB.SaveChanges();
            Order.UserID = CurrentUser.ID;
            Order.Time = DateTime.Now;
            Order.Status = OrderStatus.待付款;
            Order.PayMethod = PayMethod.支付宝;
            Order.PayCode = "";
            DB.Orders.Add(Order);
            DB.SaveChanges();
            return RedirectToAction("Pay", "Mall", new { id = Order.ID });
        }
    }
}