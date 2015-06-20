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
                Count = Count
            };
            DB.OrderDetails.Add(OrderDetail);
            DB.SaveChanges();
            return Msg("该商品已经成功加入到购物车！");
        }
    }
}