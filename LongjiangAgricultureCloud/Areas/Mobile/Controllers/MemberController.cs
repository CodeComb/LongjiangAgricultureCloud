using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using LongjiangAgricultureCloud.Schema;
using LongjiangAgricultureCloud.Models;
using LongjiangAgricultureCloud.Helpers;

namespace LongjiangAgricultureCloud.Areas.Mobile.Controllers
{
    [MobileAuthorize]
    public class MemberController : BaseController
    {
        /// <summary>
        /// 会员中心首页
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View(CurrentUser);
        }

        /// <summary>
        /// 个人资料设定
        /// </summary>
        /// <returns></returns>
        public ActionResult Config()
        {
            return View(CurrentUser);
        }

        /// <summary>
        /// 个人资料设定
        /// </summary>
        /// <param name="User"></param>
        /// <param name="Confirm"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Config(User User, string Confirm)
        {
            CurrentUser.AreaID = User.AreaID;
            CurrentUser.Address = User.Address;
            CurrentUser.Name = User.Name;
            if (!string.IsNullOrEmpty(User.Password))
            {
                if (Confirm != User.Password)
                    return Msg("两次密码输入不一致，请返回重试！");
                CurrentUser.Password = Security.SHA1(Confirm);
            }
            DB.SaveChanges();
            return Msg("个人资料保存成功！");
        }

        /// <summary>
        /// 商城加盟申请
        /// </summary>
        /// <returns></returns>
        public ActionResult Join()
        {
            return View(CurrentUser.Providers);
        }

        /// <summary>
        /// 商城加盟申请
        /// </summary>
        /// <returns></returns>
        public ActionResult Join2()
        {
            return View();
        }

        /// <summary>
        /// 商城加盟申请
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
        public ActionResult Join2(string Title, string Description, string Address, string Name, string Phone, string Tel, string Email)
        {
            var Provider = new Provider();
            Provider.Title = Title;
            Provider.Description = Description;
            Provider.Address = Address;
            Provider.Name = Name;
            Provider.Phone = Phone;
            Provider.Tel = Tel;
            Provider.Email = Email;
            Provider.UserID = CurrentUser.ID;
            Provider.Status = ProviderStatus.等待审核;
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
            return Msg("申请提交成功！请耐心等待管理员审核，在此期间请确保登记的联系方式畅通，切勿重复提交申请。");
        }

        /// <summary>
        /// 我的订单
        /// </summary>
        /// <returns></returns>
        public ActionResult Order()
        {
            return View();
        }

        /// <summary>
        /// 我的订单
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public ActionResult OrderRaw(int p = 0)
        {
            var orders = (from o in DB.Orders
                          where o.UserID == CurrentUser.ID
                          orderby o.Time descending
                          select o).Skip(p * 10).Take(10).ToList();
            return View(orders);
        }

        /// <summary>
        /// 取消订单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Cancel(Guid id)
        {
            var order = DB.Orders.Find(id);
            if (order.UserID != CurrentUser.ID || order.Status != OrderStatus.待付款)
                return Msg("非法操作");
            order.Status = OrderStatus.已取消;
            DB.SaveChanges();
            return Msg("订单取消成功！");
        }

        /// <summary>
        /// 确认收货
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Verify(Guid id)
        {
            var order = DB.Orders.Find(id);
            if (order.UserID != CurrentUser.ID || order.Status != OrderStatus.待收货)
                return Msg("非法操作");
            order.Status = OrderStatus.待评价;
            DB.SaveChanges();
            return Msg("确认收货成功！");
        }

        /// <summary>
        /// 退款申请
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult GiveBack(Guid id)
        {
            var order = DB.Orders.Find(id);
            if (order.UserID != CurrentUser.ID || order.Status != OrderStatus.待评价)
                return Msg("非法操作");
            return View(order);
        }

        /// <summary>
        /// 退款申请
        /// </summary>
        /// <param name="id"></param>
        /// <param name="Reason"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GiveBack(Guid id, string Reason)
        {
            var order = DB.Orders.Find(id);
            if (order.UserID != CurrentUser.ID || order.Status != OrderStatus.待评价)
                return Msg("非法操作");
            order.GiveBackReason = Reason;
            order.Status = OrderStatus.退款中;
            DB.SaveChanges();
            return Msg("退款申请已提交，请耐心等待，客服将与您联系！");
        }

        /// <summary>
        /// 商品评价
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Comment(Guid id)
        {
            var order = DB.Orders.Find(id);
            if (order.UserID != CurrentUser.ID || order.Status != OrderStatus.待评价)
                return Msg("非法操作");
            return View(order);
        }

        /// <summary>
        /// 商品评价
        /// </summary>
        /// <param name="id"></param>
        /// <param name="Dif"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Comment(Guid id, bool? Dif)
        {
            var order = DB.Orders.Find(id);
            if (order.UserID != CurrentUser.ID || order.Status != OrderStatus.待评价)
                return Msg("非法操作");
            order.Status = OrderStatus.已完成;
            foreach (var od in order.OrderDetails)
            {
                var comment = new Comment
                {
                    UserID = CurrentUser.ID,
                    TargetID = od.ProductID,
                    Type = CommentType.商品评论,
                    Content = Request.Form["Content-" + od.ID].ToString(),
                    Score = Convert.ToInt32(Request.Form["Score-" + od.ID]),
                    Verify = ViewBag.VerifyProductComment ? false : true
                };
                DB.Comments.Add(comment);
            }
            DB.SaveChanges();
            return Msg("评价成功！");
        }

        /// <summary>
        /// 我的本地通
        /// </summary>
        /// <returns></returns>
        public ActionResult Local()
        {
            return View();
        }

        /// <summary>
        /// 我的本地通
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public ActionResult LocalRaw(int p = 0)
        {
            var informations = (from i in DB.Informations
                                where i.Type == InformationType.本地通信息
                                && i.UserID == CurrentUser.ID
                                orderby i.Time descending
                                select i).Skip(p * 20).Take(20).ToList();
            return View(informations);
        }

        /// <summary>
        /// 发布本地通信息
        /// </summary>
        /// <returns></returns>
        public ActionResult CreateLocal()
        {
            ViewBag.Level1 = (from c in DB.Catalogs
                              where c.Level == 0
                              && !c.Delete
                              && c.Type == CatalogType.本地通分类
                              select c).ToList();
            ViewBag.Level2 = (from c in DB.Catalogs
                              where c.Level == 1
                              && !c.Delete
                              && c.Type == CatalogType.本地通分类
                              select c).ToList();
            ViewBag.Level3 = (from c in DB.Catalogs
                              where c.Level == 2
                              && !c.Delete
                              && c.Type == CatalogType.本地通分类
                              select c).ToList();
            return View();
        }

        /// <summary>
        /// 编辑本地通信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult EditLocal(int id)
        {
            ViewBag.Level1 = (from c in DB.Catalogs
                              where c.Level == 0
                              && !c.Delete
                              && c.Type == CatalogType.本地通分类
                              select c).ToList();
            ViewBag.Level2 = (from c in DB.Catalogs
                              where c.Level == 1
                              && !c.Delete
                              && c.Type == CatalogType.本地通分类
                              select c).ToList();
            ViewBag.Level3 = (from c in DB.Catalogs
                              where c.Level == 2
                              && !c.Delete
                              && c.Type == CatalogType.本地通分类
                              select c).ToList();
            var information = DB.Informations.Find(id);
            if (information.UserID != CurrentUser.ID)
                return Msg("非法操作");
            return View(information);
        }

        /// <summary>
        /// 发布本地通信息
        /// </summary>
        /// <param name="Information"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateLocal(Information Information)
        {
            Information.Time = DateTime.Now;
            Information.Type = InformationType.本地通信息;
            Information.UserID = CurrentUser.ID;
            if (ViewBag.VerifyLocalTong)
                Information.Verify = false;
            else
                Information.Verify = true;
            var Video = Request.Files["Video"];
            if (Video != null)
            {
                var fname = Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(Video.FileName);
                Video.SaveAs(Server.MapPath("~/Files/Video/" + fname));
                Information.VideoURL = fname;
            }
            DB.Informations.Add(Information);
            DB.SaveChanges();
            return Msg("本地通信息已提交");
        }

        /// <summary>
        /// 编辑本地通信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="Information"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditLocal(int id, Information Information)
        {
            var information = DB.Informations.Find(id);
            if (information.UserID != CurrentUser.ID)
                return Msg("非法操作");
            information.Title = Information.Title;
            information.Description = Information.Description;
            information.Name = Information.Name;
            information.Address = Information.Address;
            information.Phone = Information.Phone;
            information.CatalogID = Information.CatalogID;
            information.SupplyDemand = Information.SupplyDemand;
            if (ViewBag.VerifyLocalTong)
                information.Verify = false;
            else
                information.Verify = true;
            var Video = Request.Files["Video"];
            if (Video != null)
            {
                var fname = Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(Video.FileName);
                Video.SaveAs(Server.MapPath("~/Files/Video/" + fname));
                information.VideoURL = fname;
            }
            DB.SaveChanges();
            return Msg("本地通信息编辑成功");
        }

        /// <summary>
        /// 我的农机服务信息
        /// </summary>
        /// <returns></returns>
        public ActionResult Service()
        {
            return View();
        }

        /// <summary>
        /// 我的农机服务信息
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public ActionResult ServiceRaw(int p = 0)
        {
            var informations = (from i in DB.Informations
                                where (i.Type == InformationType.二手农机
                                || i.Type == InformationType.农机找活
                                || i.Type == InformationType.土地找机手
                                || i.Type == InformationType.维修站)
                                && i.UserID == CurrentUser.ID
                                orderby i.Time descending
                                select i).Skip(p * 20).Take(20).ToList();
            return View(informations);
        }

        /// <summary>
        /// 删除农机服务信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteService(int id)
        {
            var service = DB.Informations.Find(id);
            if (service.UserID != CurrentUser.ID)
                return Msg("非法操作");
            DB.Informations.Remove(service);
            DB.SaveChanges();
            return Msg("农机信息删除成功！");
        }

        /// <summary>
        /// 发布农机服务信息
        /// </summary>
        /// <returns></returns>
        public ActionResult CreateService()
        {
            ViewBag.Level1 = (from c in DB.Catalogs
                              where c.Level == 0
                              && !c.Delete
                              && c.Type == CatalogType.农机服务分类
                              select c).ToList();
            ViewBag.Level2 = (from c in DB.Catalogs
                              where c.Level == 1
                              && !c.Delete
                              && c.Type == CatalogType.农机服务分类
                              select c).ToList();
            ViewBag.Level3 = (from c in DB.Catalogs
                              where c.Level == 2
                              && !c.Delete
                              && c.Type == CatalogType.农机服务分类
                              select c).ToList();
            ViewBag.Level4 = (from c in DB.Catalogs
                              where c.Level == 3
                              && !c.Delete
                              && c.Type == CatalogType.农机服务分类
                              select c).ToList();
            return View();
        }

        /// <summary>
        /// 发布农机服务信息
        /// </summary>
        /// <param name="Title"></param>
        /// <param name="Type"></param>
        /// <param name="CatalogID"></param>
        /// <param name="Lat"></param>
        /// <param name="Lon"></param>
        /// <param name="Name"></param>
        /// <param name="Address"></param>
        /// <param name="Description"></param>
        /// <param name="Phone"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateService(string Title, InformationType Type, int? CatalogID, decimal? Lat, decimal? Lon, string Name, string Address, string Description, string Phone)
        {
            var service = new Information
            {
                Title = Title,
                CatalogID = CatalogID,
                Lat = Lat,
                Lon = Lon,
                Name = Name,
                Address = Address,
                Description = Description,
                Phone = Phone,
                UserID = CurrentUser.ID,
                Type = Type,
                Time = DateTime.Now,
                Verify = ViewBag.VerifyService ? false : true
            };

            if (Type == InformationType.维修站)
            {
                var Picture = Request.Files["Picture"];
                if (Picture != null)
                {
                    using (var binaryReader = new BinaryReader(Picture.InputStream))
                    {
                        service.Picture = binaryReader.ReadBytes(Picture.ContentLength);
                    }
                }
            }

            DB.Informations.Add(service);
            DB.SaveChanges();
            return Msg("农机信息发布成功");
        }

        /// <summary>
        /// 编辑农机服务信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult EditService(int id)
        {
            var service = DB.Informations.Find(id);
            if (service.UserID != CurrentUser.ID)
                return Msg("非法操作");
            return View(service);
        }

        /// <summary>
        /// 编辑农机服务信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="Title"></param>
        /// <param name="Price"></param>
        /// <param name="Name"></param>
        /// <param name="Address"></param>
        /// <param name="Description"></param>
        /// <param name="Phone"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditService(int id, string Title, string Price, string Name, string Address, string Description, string Phone)
        {
            var service = DB.Informations.Find(id);
            if (service.UserID != CurrentUser.ID)
                return Msg("非法操作");
            service.Title = Title;
            service.Name = Name;
            service.Address = Address;
            service.Description = Description;
            service.Phone = Phone;
            service.Price = Price;
            if (service.Type == InformationType.维修站)
            {
                var Picture = Request.Files["Picture"];
                if (Picture != null)
                {
                    using (var binaryReader = new BinaryReader(Picture.InputStream))
                    {
                        service.Picture = binaryReader.ReadBytes(Picture.ContentLength);
                    }
                }
            }
            DB.SaveChanges();
            return Msg("农机信息编辑成功");
        }
    }
}