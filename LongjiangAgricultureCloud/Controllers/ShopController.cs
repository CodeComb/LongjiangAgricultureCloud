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
    [CheckRole(UserRole.系统管理员)]
    public class ShopController : BaseController
    {
        // GET: Shop
        public ActionResult Index(string Title, string ProductCode, string Provider, int? Store, int? StoreGte, int? StoreLte, int p = 0)
        {
            ViewBag.Stores = DB.Stores.ToList();
            IEnumerable<Product> query = DB.Products;
            if (!string.IsNullOrEmpty(Title))
                query = query.Where(x => x.Title.Contains(Title) || Title.Contains(x.Title));
            if (!string.IsNullOrEmpty(ProductCode))
                query = query.Where(x => x.ProductCode == ProductCode);
            if (!string.IsNullOrEmpty(Provider))
                query = query.Where(x => x.Provider.Title.Contains(Provider) || Provider.Contains(x.Provider.Title));
            if (StoreGte.HasValue)
                query = query.Where(x => x.StoreCount >= StoreGte.Value);
            if (StoreLte.HasValue)
                query = query.Where(x => x.StoreCount <= StoreLte.Value);
            if (Store.HasValue)
                query = query.Where(x => x.StoreID == Store.Value);
            ViewBag.PageInfo = PagerHelper.Do(ref query, 50, p);
            return View(query);
        }

        public ActionResult CreateProduct()
        {
            ViewBag.Stores = DB.Stores.ToList();
            ViewBag.Providers = DB.Providers.Where(x => x.Status == ProviderStatus.审核通过).ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateProduct(Product Product)
        {
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

        public ActionResult Provider(string Title, string Phone, string Name, int p = 1)
        {
            IEnumerable<Provider> query = DB.Providers.Where(x => !x.Delete);
            if (!string.IsNullOrEmpty(Title))
                query = query.Where(x => x.Title.Contains(Title) || Title.Contains(x.Title));
            if (!string.IsNullOrEmpty(Phone))
                query = query.Where(x => x.Phone == Phone);
            if (!string.IsNullOrEmpty(Name))
                query = query.Where(x => x.Name == Name);
            ViewBag.PageInfo = PagerHelper.Do(ref query, 50, p);
            return View(query);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteProduct(int id)
        {
            var product = DB.Products.Find(id);
            product.Delete = true;
            DB.SaveChanges();
            return Content("ok");
        }

        public ActionResult CreateProvider()
        {
            return View();
        }

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

        public ActionResult EditProvider(int id)
        {
            var provider = DB.Providers.Find(id);
            return View(provider);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProvider(int id, string Title, string Description, string Reason, string Address, string Name, string Phone, string Tel, string Email, ProviderStatus Status)
        {
            var provider = DB.Providers.Find(id);
            #region 图片上传
            var Picture = Request.Files["Picture"];
            if (Picture != null)
            {
                using (var binaryReader = new BinaryReader(Picture.InputStream))
                {
                    provider.Picture = binaryReader.ReadBytes(Picture.ContentLength);
                }
            }
            else
            {
                provider.Picture = null;
            }

            var BusinessLicence = Request.Files["BusinessLicence"];
            if (BusinessLicence != null)
            {
                using (var binaryReader = new BinaryReader(BusinessLicence.InputStream))
                {
                    provider.BusinessLicence = binaryReader.ReadBytes(BusinessLicence.ContentLength);
                }
            }
            else
            {
                provider.BusinessLicence = null;
            }

            var OrganizationRegistrationCodeCertificate = Request.Files["OrganizationRegistrationCodeCertificate"];
            if (OrganizationRegistrationCodeCertificate != null)
            {
                using (var binaryReader = new BinaryReader(OrganizationRegistrationCodeCertificate.InputStream))
                {
                    provider.OrganizationRegistrationCodeCertificate = binaryReader.ReadBytes(OrganizationRegistrationCodeCertificate.ContentLength);
                }
            }
            else
            {
                provider.OrganizationRegistrationCodeCertificate = null;
            }

            var TaxRegistrationCertificate = Request.Files["TaxRegistrationCertificate"];
            if (TaxRegistrationCertificate != null)
            {
                using (var binaryReader = new BinaryReader(TaxRegistrationCertificate.InputStream))
                {
                    provider.TaxRegistrationCertificate = binaryReader.ReadBytes(TaxRegistrationCertificate.ContentLength);
                }
            }
            else
            {
                provider.TaxRegistrationCertificate = null;
            }

            var ArtificialPersonIdentityCard = Request.Files["ArtificialPersonIdentityCard"];
            if (ArtificialPersonIdentityCard != null)
            {
                using (var binaryReader = new BinaryReader(ArtificialPersonIdentityCard.InputStream))
                {
                    provider.ArtificialPersonIdentityCard = binaryReader.ReadBytes(ArtificialPersonIdentityCard.ContentLength);
                }
            }
            else
            {
                provider.ArtificialPersonIdentityCard = null;
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteProvider(int id)
        {
            var provider = DB.Providers.Find(id);
            provider.Delete = true;
            DB.SaveChanges();
            return Content("ok");
        }

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

        public ActionResult Order()
        {
            return View();
        }

        public ActionResult Distribute()
        {
            return View();
        }

        public ActionResult Finance()
        {
            return View();
        }
    }
}