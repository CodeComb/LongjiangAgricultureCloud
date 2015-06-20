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
        // GET: Mobile/Member
        public ActionResult Index()
        {
            return View(CurrentUser);
        }

        public ActionResult Config()
        {
            return View(CurrentUser);
        }

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

        public ActionResult Join()
        {
            return View(CurrentUser.Providers);
        }

        public ActionResult Join2()
        {
            return View();
        }

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

        public ActionResult Order()
        {
            return View();
        }

        public ActionResult OrderRaw(int p = 0)
        {
            var orders = (from o in DB.Orders
                          where o.UserID == CurrentUser.ID
                          orderby o.Time descending
                          select o).Skip(p * 10).Take(10).ToList();
            return View(orders);
        }

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

        public ActionResult GiveBack(Guid id)
        {
            var order = DB.Orders.Find(id);
            if (order.UserID != CurrentUser.ID || order.Status != OrderStatus.待评价)
                return Msg("非法操作");
            return View(order);
        }

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

        public ActionResult Comment(Guid id)
        {
            var order = DB.Orders.Find(id);
            if (order.UserID != CurrentUser.ID || order.Status != OrderStatus.待评价)
                return Msg("非法操作");
            return View(order);
        }

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
                    Score = Convert.ToInt32(Request.Form["Score-" + od.ID])
                };
                DB.Comments.Add(comment);
            }
            DB.SaveChanges();
            return Msg("评价成功！");
        }
    }
}