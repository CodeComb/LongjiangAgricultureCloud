using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using LongjiangAgricultureCloud.Models;
using LongjiangAgricultureCloud.Schema;
using LongjiangAgricultureCloud.Helpers;

namespace LongjiangAgricultureCloud.Controllers
{
    [CheckRole(UserRole.库存管理员)]
    public class ShopController : BaseController
    {
        // GET: Shop

        /// <summary>
        /// 商品列表
        /// </summary>
        /// <param name="Title"></param>
        /// <param name="ProductCode"></param>
        /// <param name="Provider"></param>
        /// <param name="Store"></param>
        /// <param name="StoreGte"></param>
        /// <param name="StoreLte"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public ActionResult Index(string Title, string ProductCode, string Provider, int? Store, int? StoreGte, int? StoreLte, int p = 0)
        {
            ViewBag.Stores = DB.Stores.ToList();
            IEnumerable<Product> query = DB.Products.Where(x => !x.Delete).ToList();
            if (!string.IsNullOrEmpty(Title))
                query = query.Where(x => x.Title.Contains(Title) || Title.Contains(x.Title)).ToList();
            if (!string.IsNullOrEmpty(ProductCode))
                query = query.Where(x => x.ProductCode == ProductCode).ToList();
            if (!string.IsNullOrEmpty(Provider))
                query = query.Where(x => x.Provider.Title.Contains(Provider) || Provider.Contains(x.Provider.Title)).ToList();
            if (StoreGte.HasValue)
                query = query.Where(x => x.StoreCount >= StoreGte.Value).ToList();
            if (StoreLte.HasValue)
                query = query.Where(x => x.StoreCount <= StoreLte.Value).ToList();
            if (Store.HasValue)
                query = query.Where(x => x.StoreID == Store.Value).ToList();
            if (CurrentUser.Role == UserRole.库存管理员)
                query = query.Where(x => x.Store.UserID != null && x.Store.UserID == CurrentUser.ID).ToList();
            query = query.OrderByDescending(x => x.Top).ToList();
            ViewBag.PageInfo = PagerHelper.Do(ref query, 50, p);
            return View(query);
        }

        /// <summary>
        /// 创建商品
        /// </summary>
        /// <returns></returns>
        public ActionResult CreateProduct()
        {
            ViewBag.Stores = DB.Stores.Where(x => !x.Delete).ToList();
            if (ViewBag.Stores.Count == 0)
                return Msg("请先添加仓库！");
            ViewBag.Providers = DB.Providers.Where(x => x.Status == ProviderStatus.审核通过 && !x.Delete).ToList();
            ViewBag.Level1 = (from c in DB.Catalogs
                              where c.Level == 0
                              && !c.Delete
                              && c.Type == CatalogType.商品分类
                              select c).ToList();
            ViewBag.Level2 = (from c in DB.Catalogs
                              where c.Level == 1
                              && !c.Delete
                              && c.Type == CatalogType.商品分类
                              select c).ToList();
            ViewBag.Level3 = (from c in DB.Catalogs
                              where c.Level == 2
                              && !c.Delete
                              && c.Type == CatalogType.商品分类
                              select c).ToList();
            return View();
        }

        /// <summary>
        /// 创建商品
        /// </summary>
        /// <param name="Title"></param>
        /// <param name="ProductCode"></param>
        /// <param name="Standard"></param>
        /// <param name="Unit"></param>
        /// <param name="Price"></param>
        /// <param name="StoreID"></param>
        /// <param name="StoreCount"></param>
        /// <param name="ProviderID"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult CreateProduct(string Title, int CatalogID, string Description, string ProductCode, string Standard, string Unit, float Price, int StoreID, int StoreCount, int? ProviderID)
        {
            var Product = new Product();
            Product.Title = Title;
            Product.ProductCode = ProductCode;
            Product.Standard = Standard;
            Product.Unit = Unit;
            Product.Price = Price;
            Product.StoreID = StoreID;
            Product.StoreCount = StoreCount;
            Product.ProviderID = ProviderID;
            Product.Description = Description;
            Product.CatalogID = CatalogID;
            #region 处理5张图片
            var Picture1 = Request.Files["Picture1"];
            if (Picture1 != null)
            {
                using (var binaryReader = new BinaryReader(Picture1.InputStream))
                {
                    Product.Picture1 = binaryReader.ReadBytes(Picture1.ContentLength);
                }
            }
            else
            {
                Product.Picture1 = null;
            }

            var Picture2 = Request.Files["Picture2"];
            if (Picture2 != null)
            {
                using (var binaryReader = new BinaryReader(Picture2.InputStream))
                {
                    Product.Picture2 = binaryReader.ReadBytes(Picture2.ContentLength);
                }
            }
            else
            {
                Product.Picture2 = null;
            }

            var Picture3 = Request.Files["Picture3"];
            if (Picture3 != null)
            {
                using (var binaryReader = new BinaryReader(Picture3.InputStream))
                {
                    Product.Picture3 = binaryReader.ReadBytes(Picture3.ContentLength);
                }
            }
            else
            {
                Product.Picture3 = null;
            }

            var Picture4 = Request.Files["Picture4"];
            if (Picture4 != null)
            {
                using (var binaryReader = new BinaryReader(Picture4.InputStream))
                {
                    Product.Picture4 = binaryReader.ReadBytes(Picture4.ContentLength);
                }
            }
            else
            {
                Product.Picture4 = null;
            }

            var Picture5 = Request.Files["Picture5"];
            if (Picture5 != null)
            {
                using (var binaryReader = new BinaryReader(Picture5.InputStream))
                {
                    Product.Picture5 = binaryReader.ReadBytes(Picture5.ContentLength);
                }
            }
            else
            {
                Product.Picture5 = null;
            }
            #endregion
            DB.Products.Add(Product);
            DB.SaveChanges();
            return RedirectToAction("Success", "Shared");
        }

        /// <summary>
        /// 编辑商品
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult EditProduct(int id)
        {
            var product = DB.Products.Find(id);
            ViewBag.Stores = DB.Stores.Where(x => !x.Delete).ToList();
            ViewBag.Providers = DB.Providers.Where(x => x.Status == ProviderStatus.审核通过 && !x.Delete).ToList();
            ViewBag.Level1 = (from c in DB.Catalogs
                              where c.Level == 0
                              && c.Type == CatalogType.商品分类
                              && !c.Delete
                              select c).ToList();
            ViewBag.Level2 = (from c in DB.Catalogs
                              where c.Level == 1
                              && c.Type == CatalogType.商品分类
                              && !c.Delete
                              select c).ToList();
            ViewBag.Level3 = (from c in DB.Catalogs
                              where c.Level == 2
                              && c.Type == CatalogType.商品分类
                              && !c.Delete
                              select c).ToList();
            return View(product);
        }

        /// <summary>
        /// 编辑商品
        /// </summary>
        /// <param name="id"></param>
        /// <param name="Title"></param>
        /// <param name="CatalogID"></param>
        /// <param name="Description"></param>
        /// <param name="ProductCode"></param>
        /// <param name="Standard"></param>
        /// <param name="Unit"></param>
        /// <param name="Price"></param>
        /// <param name="StoreID"></param>
        /// <param name="StoreCount"></param>
        /// <param name="ProviderID"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult EditProduct(int id, string Title, int Top, bool Recommend, int? CatalogID, string Description, string ProductCode, string Standard, string Unit, float Price, int StoreID, int StoreCount, int? ProviderID)
        {
            if (CatalogID == null)
                return Msg("请选择一个商品分类");
            var Product = DB.Products.Find(id);
            Product.Title = Title;
            Product.ProductCode = ProductCode;
            Product.Standard = Standard;
            Product.Unit = Unit;
            Product.Price = Price;
            Product.StoreID = StoreID;
            Product.StoreCount = StoreCount;
            Product.ProviderID = ProviderID;
            Product.Description = Description;
            Product.CatalogID = CatalogID.Value;
            Product.Top = Top;
            Product.Recommend = Recommend;
            #region 处理5张图片
            var Picture1 = Request.Files["Picture1"];
            if (Picture1 != null && Picture1.ContentLength > 0)
            {
                using (var binaryReader = new BinaryReader(Picture1.InputStream))
                {
                    Product.Picture1 = binaryReader.ReadBytes(Picture1.ContentLength);
                }
            }

            var Picture2 = Request.Files["Picture2"];
            if (Picture2 != null && Picture2.ContentLength > 0)
            {
                using (var binaryReader = new BinaryReader(Picture2.InputStream))
                {
                    Product.Picture2 = binaryReader.ReadBytes(Picture2.ContentLength);
                }
            }

            var Picture3 = Request.Files["Picture3"];
            if (Picture3 != null && Picture3.ContentLength > 0)
            {
                using (var binaryReader = new BinaryReader(Picture3.InputStream))
                {
                    Product.Picture3 = binaryReader.ReadBytes(Picture3.ContentLength);
                }
            }

            var Picture4 = Request.Files["Picture4"];
            if (Picture4 != null && Picture4.ContentLength > 0)
            {
                using (var binaryReader = new BinaryReader(Picture4.InputStream))
                {
                    Product.Picture4 = binaryReader.ReadBytes(Picture4.ContentLength);
                }
            }

            var Picture5 = Request.Files["Picture5"];
            if (Picture5 != null && Picture5.ContentLength > 0)
            {
                using (var binaryReader = new BinaryReader(Picture5.InputStream))
                {
                    Product.Picture5 = binaryReader.ReadBytes(Picture5.ContentLength);
                }
            }
            #endregion
            DB.SaveChanges();
            return RedirectToAction("Success", "Shared");
        }

        /// <summary>
        /// 供应商列表
        /// </summary>
        /// <param name="Title"></param>
        /// <param name="Phone"></param>
        /// <param name="Name"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public ActionResult Provider(string Title, string Phone, string Name, ProviderStatus? Status ,int p = 1)
        {
            IEnumerable<Provider> query = DB.Providers.Where(x => !x.Delete).ToList();
            if (Status.HasValue)
                query = query.Where(x => x.Status == Status.Value).ToList();
            if (!string.IsNullOrEmpty(Title))
                query = query.Where(x => x.Title.Contains(Title) || Title.Contains(x.Title)).ToList();
            if (!string.IsNullOrEmpty(Phone))
                query = query.Where(x => x.Phone == Phone).ToList();
            if (!string.IsNullOrEmpty(Name))
                query = query.Where(x => x.Name == Name).ToList();
            ViewBag.PageInfo = PagerHelper.Do(ref query, 50, p);
            return View(query);
        }

        /// <summary>
        /// 删除商品
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteProduct(int id)
        {
            var product = DB.Products.Find(id);
            product.Delete = true;
            DB.SaveChanges();
            return Content("ok");
        }

        /// <summary>
        /// 创建供应商
        /// </summary>
        /// <returns></returns>
        public ActionResult CreateProvider()
        {
            return View();
        }

        /// <summary>
        /// 创建供应商
        /// </summary>
        /// <param name="Title"></param>
        /// <param name="Description"></param>
        /// <param name="Address"></param>
        /// <param name="Name"></param>
        /// <param name="Phone"></param>
        /// <param name="Tel"></param>
        /// <param name="Email"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateProvider(string Title, string Description, string Address, string Name, string Phone, string Tel, string Email)
        {
            var Provider = new Provider();
            Provider.Title = Title;
            Provider.Description = Description;
            Provider.Address = Address;
            Provider.Name = Name;
            Provider.Phone = Phone;
            Provider.Tel = Tel;
            Provider.Email = Email;
            Provider.Status = ProviderStatus.审核通过;
            #region 图片上传
            var Picture = Request.Files["Picture"];
            if (Picture != null)
            {
                using (var binaryReader = new BinaryReader(Picture.InputStream))
                {
                    Provider.Picture = binaryReader.ReadBytes(Picture.ContentLength);
                }
            }
            else
            {
                Provider.Picture = null;
            }

            var BusinessLicence = Request.Files["BusinessLicence"];
            if (BusinessLicence != null)
            {
                using (var binaryReader = new BinaryReader(BusinessLicence.InputStream))
                {
                    Provider.BusinessLicence = binaryReader.ReadBytes(BusinessLicence.ContentLength);
                }
            }
            else
            {
                Provider.BusinessLicence = null;
            }

            var OrganizationRegistrationCodeCertificate = Request.Files["OrganizationRegistrationCodeCertificate"];
            if (OrganizationRegistrationCodeCertificate != null)
            {
                using (var binaryReader = new BinaryReader(OrganizationRegistrationCodeCertificate.InputStream))
                {
                    Provider.OrganizationRegistrationCodeCertificate = binaryReader.ReadBytes(OrganizationRegistrationCodeCertificate.ContentLength);
                }
            }
            else
            {
                Provider.OrganizationRegistrationCodeCertificate = null;
            }

            var TaxRegistrationCertificate = Request.Files["TaxRegistrationCertificate"];
            if (TaxRegistrationCertificate != null)
            {
                using (var binaryReader = new BinaryReader(TaxRegistrationCertificate.InputStream))
                {
                    Provider.TaxRegistrationCertificate = binaryReader.ReadBytes(TaxRegistrationCertificate.ContentLength);
                }
            }
            else
            {
                Provider.TaxRegistrationCertificate = null;
            }

            var ArtificialPersonIdentityCard = Request.Files["ArtificialPersonIdentityCard"];
            if (ArtificialPersonIdentityCard != null)
            {
                using (var binaryReader = new BinaryReader(ArtificialPersonIdentityCard.InputStream))
                {
                    Provider.ArtificialPersonIdentityCard = binaryReader.ReadBytes(ArtificialPersonIdentityCard.ContentLength);
                }
            }
            else
            {
                Provider.ArtificialPersonIdentityCard = null;
            }
            #endregion
            Provider.Time = DateTime.Now;
            Provider.UserID = null;
            DB.Providers.Add(Provider);
            DB.SaveChanges();
            return RedirectToAction("Success", "Shared");
        }

        /// <summary>
        /// 编辑供应商
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult EditProvider(int id)
        {
            var provider = DB.Providers.Find(id);
            return View(provider);
        }

        /// <summary>
        /// 编辑供应商
        /// </summary>
        /// <param name="id"></param>
        /// <param name="Title"></param>
        /// <param name="Description"></param>
        /// <param name="Reason"></param>
        /// <param name="Address"></param>
        /// <param name="Name"></param>
        /// <param name="Phone"></param>
        /// <param name="Tel"></param>
        /// <param name="Email"></param>
        /// <param name="Status"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProvider(int id, string Title, string Description, string Reason, string Address, string Name, string Phone, string Tel, string Email, ProviderStatus Status)
        {
            var provider = DB.Providers.Find(id);
            #region 图片上传
            var Picture = Request.Files["Picture"];
            if (Picture != null && Picture.ContentLength > 0)
            {
                using (var binaryReader = new BinaryReader(Picture.InputStream))
                {
                    provider.Picture = binaryReader.ReadBytes(Picture.ContentLength);
                }
            }

            var BusinessLicence = Request.Files["BusinessLicence"];
            if (BusinessLicence != null && BusinessLicence.ContentLength > 0)
            {
                using (var binaryReader = new BinaryReader(BusinessLicence.InputStream))
                {
                    provider.BusinessLicence = binaryReader.ReadBytes(BusinessLicence.ContentLength);
                }
            }

            var OrganizationRegistrationCodeCertificate = Request.Files["OrganizationRegistrationCodeCertificate"];
            if (OrganizationRegistrationCodeCertificate != null && OrganizationRegistrationCodeCertificate.ContentLength > 0)
            {
                using (var binaryReader = new BinaryReader(OrganizationRegistrationCodeCertificate.InputStream))
                {
                    provider.OrganizationRegistrationCodeCertificate = binaryReader.ReadBytes(OrganizationRegistrationCodeCertificate.ContentLength);
                }
            }

            var TaxRegistrationCertificate = Request.Files["TaxRegistrationCertificate"];
            if (TaxRegistrationCertificate != null && TaxRegistrationCertificate.ContentLength > 0)
            {
                using (var binaryReader = new BinaryReader(TaxRegistrationCertificate.InputStream))
                {
                    provider.TaxRegistrationCertificate = binaryReader.ReadBytes(TaxRegistrationCertificate.ContentLength);
                }
            }

            var ArtificialPersonIdentityCard = Request.Files["ArtificialPersonIdentityCard"];
            if (ArtificialPersonIdentityCard != null && ArtificialPersonIdentityCard.ContentLength > 0)
            {
                using (var binaryReader = new BinaryReader(ArtificialPersonIdentityCard.InputStream))
                {
                    provider.ArtificialPersonIdentityCard = binaryReader.ReadBytes(ArtificialPersonIdentityCard.ContentLength);
                }
            }
            #endregion
            provider.Title = Title;
            provider.Reason = Reason;
            provider.Status = Status;
            provider.Tel = Tel;
            provider.Phone = Phone;
            provider.Name = Name;
            DB.SaveChanges();
            return RedirectToAction("Success", "Shared");
        }

        /// <summary>
        /// 删除供应商
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteProvider(int id)
        {
            var provider = DB.Providers.Find(id);
            provider.Delete = true;
            DB.SaveChanges();
            return Content("ok");
        }

        /// <summary>
        /// 供应商图片接口
        /// </summary>
        /// <param name="id"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public ActionResult ProviderImg(int id, int index)
        {
            var provider = DB.Providers.Find(id);
            switch (index)
            {
                case 1: return File(provider.BusinessLicence, "image/jpeg");
                case 2: return File(provider.OrganizationRegistrationCodeCertificate, "image/jpeg");
                case 3: return File(provider.TaxRegistrationCertificate, "image/jpeg");
                case 4: return File(provider.ArtificialPersonIdentityCard, "image/jpeg");
                case 5: return File(provider.Picture, "image/jpeg");
                default: return File(new byte[0], "image/jpeg");
            }
        }

        /// <summary>
        /// 订单管理
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="Begin"></param>
        /// <param name="End"></param>
        /// <param name="Status"></param>
        /// <param name="Address"></param>
        /// <param name="Name"></param>
        /// <param name="Phone"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public ActionResult Order(Guid? ID, DateTime? Begin, DateTime? End, OrderStatus? Status, string Address, string Name, string Phone, int p = 0)
        {
            IEnumerable<Order> query = DB.Orders.ToList();
            if (ID.HasValue)
                query = query.Where(x => x.ID == ID.Value).ToList();
            if (Begin.HasValue)
                query = query.Where(x => x.Time >= Begin.Value).ToList();
            if (End.HasValue)
                query = query.Where(x => x.Time <= End.Value).ToList();
            if (Status.HasValue)
                query = query.Where(x => x.Status == Status.Value).ToList();
            if (!string.IsNullOrEmpty(Name))
                query = query.Where(x => x.User.Name.Contains(Name) || Name.Contains(x.User.Name)).ToList();
            if (!string.IsNullOrEmpty(Phone))
                query = query.Where(x => x.User.Username == Phone).ToList();
            if (!string.IsNullOrEmpty(Address))
                query = query.Where(x => x.Address.Contains(Address) || Address.Contains(x.Address)).ToList();
            query = query.OrderByDescending(x => x.Time);
            ViewBag.PageInfo = PagerHelper.Do(ref query, 50, p);
            return View(query);
        }

        public ActionResult Distribute(DateTime? Begin, DateTime? End, string Name, string Status, bool Raw = false, bool Xls = false)
        {
            IEnumerable<OrderDetail> orders = (from od in DB.OrderDetails
                          where od.OrderID != null
                          select od).ToList();
            if (string.IsNullOrEmpty(Status) || Status == "待发货")
                orders = orders.Where(x => !x.DistributeFlag && x.Order.Status == OrderStatus.待发货);
            else
                orders = orders.Where(x => x.DistributeFlag);
            try
            {
                if (Begin.HasValue)
                    orders = orders.Where(x =>
                    {
                        if (x.Order.Status == OrderStatus.待发货)
                            return x.Order.PayTime.Value >= Begin.Value;
                        else
                            return x.Order.DistributeTime.Value >= Begin.Value;
                    });
                if (End.HasValue)
                    orders = orders.Where(x =>
                    {
                        if (x.Order.Status == OrderStatus.待发货)
                            return x.Order.PayTime.Value <= End.Value;
                        else
                            return x.Order.DistributeTime.Value <= End.Value;
                    });
            }
            catch { }
            if (!string.IsNullOrEmpty(Name))
                orders = orders.Where(x => !string.IsNullOrEmpty(x.User.Name) && x.User.Name.Contains(Name));
            if (CurrentUser.Role == UserRole.库存管理员)
                orders = orders.Where(x => x.Product.Store.UserID == CurrentUser.ID);
            orders = orders.OrderByDescending(x => x.Order.PayTime).ThenBy(x => x.UserID).ToList();
            if (Raw == true)
                return View("DistributeRaw", orders);
            if (Xls == true)
            {
                HttpContext.Response.AddHeader("content-disposition", "attachment;filename=\"distribute.xls\"");
                Response.ContentType = "application/x-xls";
                return View("DistributeXls", orders);
            }
            return View(orders);
        }

        /// <summary>
        /// 财务报表
        /// </summary>
        /// <returns></returns>
        public ActionResult Finance(DateTime? Begin, DateTime? End, string Address, string ProductCode, string Name, string Username, bool Raw = false, bool Xls = false)
        {
            IEnumerable<OrderDetail> orders = DB.OrderDetails
                .Where(x => x.OrderID != null 
                && x.Order != null
                && x.Order.PayTime != null
                && x.Price > 0
                && (x.Order.Status == OrderStatus.待评价 
                || x.Order.Status == OrderStatus.已完成 
                || x.Order.Status == OrderStatus.待收货 
                || x.Order.Status == OrderStatus.待发货 
                || x.Order.Status == OrderStatus.退款驳回))
                .ToList();
            if (Begin.HasValue)
                orders = orders.Where(x => x.Order.PayTime != null && x.Order.PayTime.Value >= Begin.Value).ToList();
            if (End.HasValue)
                orders = orders.Where(x => x.Order.PayTime != null && x.Order.PayTime.Value <= End.Value).ToList();
            if (!string.IsNullOrEmpty(ProductCode))
                orders = orders.Where(x => x.Product.ProductCode.IndexOf(ProductCode) == 0).ToList();
            if (!string.IsNullOrEmpty(Name))
                orders = orders.Where(x => x.User.Name == Name).ToList();
            if (!string.IsNullOrEmpty(Username))
                orders = orders.Where(x => x.User.Username == Username).ToList();
            if (!string.IsNullOrEmpty(Address))
                orders = orders.Where(x => x.Order.Address.Contains(Address)).ToList();
            orders = orders.OrderByDescending(x => x.Order.PayTime).ToList();

            var service = new List<vService>();
            try
            {
                service = (from od in orders
                           where od.Price > 0
                           && od.User != null
                           && od.User.Name != null
                           && od.User.ManagerID != null
                           && od.User.Manager != null
                           && od.User.Manager.Name != null
                           && od.User.Role == UserRole.服务站
                           group od by new {
                               Name = od.User.Name,
                               Manager = new {
                                   Name = od.User.Manager.Name,
                                   Username = od.User.Manager.Username
                               }
                           } 
                           into g
                           select new vService
                           {
                               Service = g.Key.Name,
                               Price = g != null && g.Count() > 0 ? g.Sum(x => x.Price) : 0,
                               Manager = g.Key.Manager.Name + " " + g.Key.Manager.Username
                           }).ToList();
            }
            catch (Exception e) { throw new Exception("Service Issue " + orders.Count() +"\r\n" + e.ToString()); }
            var area = (from s in service
                        group s by s.Manager into g
                        select new vArea
                        {
                            Manager = g.Key,
                            Price = g != null && g.Count() > 0 ? g.Sum(x => x.Price) : 0,
                            Service = g.ToList()
                        }).ToList();

            ViewBag.Area = area;

            if (Raw == true)
                return View("FinanceRaw", orders);
            if (Xls == true)
            {
                HttpContext.Response.AddHeader("content-disposition", "attachment;filename=\"finance.xls\"");
                Response.ContentType = "application/x-xls";
                return View("FinanceXls", orders);
            }
            return View(orders);
        }

        /// <summary>
        /// 仓库列表
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public ActionResult Store(int p = 1)
        {
            IEnumerable<Store> query = DB.Stores.Where(x => !x.Delete);
            ViewBag.PageInfo = PagerHelper.Do(ref query, 50, p);
            return View(query);
        }

        /// <summary>
        /// 删除仓库
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteStore(int id)
        {
            var store = DB.Stores.Find(id);
            store.Delete = true;
            DB.SaveChanges();
            return Content("ok");
        }

        /// <summary>
        /// 编辑仓库
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult EditStore(int id)
        {
            ViewBag.Managers = (from u in DB.Users
                                where u.Role == UserRole.库存管理员 || u.Role == UserRole.系统管理员
                                select u).ToList();
            var store = DB.Stores.Find(id);
            return View(store);
        }

        /// <summary>
        /// 创建仓库
        /// </summary>
        /// <returns></returns>
        public ActionResult CreateStore()
        {
            ViewBag.Managers = (from u in DB.Users
                                where u.Role == UserRole.库存管理员 || u.Role == UserRole.系统管理员
                                select u).ToList();
            return View();
        }

        /// <summary>
        /// 创建仓库
        /// </summary>
        /// <param name="Store"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateStore(Store Store)
        {
            if (DB.Stores.Where(x => x.Title == Store.Title).Count() > 0)
                return Msg("该仓库已经存在，请更名后重试！");
            DB.Stores.Add(Store);
            DB.SaveChanges();
            return RedirectToAction("Success", "Shared");
        }

        /// <summary>
        /// 编辑仓库
        /// </summary>
        /// <param name="id"></param>
        /// <param name="Store"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditStore(int id, Store Store)
        {
            var store = DB.Stores.Find(id);
            if (store.Title != Store.Title && DB.Stores.Where(x => x.Title == Store.Title).Count() > 0)
                return Msg("该仓库已经存在，请更名后重试！");
            store.Title = Store.Title;
            store.UserID = Store.UserID;
            DB.SaveChanges();
            return RedirectToAction("Success", "Shared");
        }

        /// <summary>
        /// 商品详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Product(int id)
        {
            var product = DB.Products.Find(id);
            return View(product);
        }

        /// <summary>
        /// 订单详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult ShowOrder(Guid id)
        {
            var order = DB.Orders.Find(id);
            return View(order);
        }

        /// <summary>
        /// 修改订单状态
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult EditOrder(Guid id)
        {
            var order = DB.Orders.Find(id);
            return View(order);
        }

        /// <summary>
        /// 修改订单状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="Status"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditOrder(Guid id, OrderStatus Status)
        {
            var order = DB.Orders.Find(id);
            if (order.Status == OrderStatus.已取消 && Status != OrderStatus.已取消)
            {
                foreach (var od in order.OrderDetails)
                    od.Product.StoreCount -= od.Count;
            }
            else if (order.Status != OrderStatus.已取消 && Status == OrderStatus.已取消)
            {
                foreach (var od in order.OrderDetails)
                    od.Product.StoreCount += od.Count;
            }
            if (Status == OrderStatus.待评价 && order.Status != OrderStatus.待评价 || Status == OrderStatus.已完成 && order.Status != OrderStatus.已完成)
                order.PayTime = DateTime.Now;
            order.Status = Status;
            DB.SaveChanges();
            return RedirectToAction("Success", "Shared");
        }

        /// <summary>
        /// 改变子订单状态
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult StatusChange(Guid id)
        {
            var order = DB.OrderDetails.Find(id);
            order.DistributeFlag = true;
            if (order.Order.OrderDetails.All(x => x.DistributeFlag))
                order.Order.Status = OrderStatus.待收货;
            DB.SaveChanges();
            return Content("ok");
        }
    }
}