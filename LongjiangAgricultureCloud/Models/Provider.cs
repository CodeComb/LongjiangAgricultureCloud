using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LongjiangAgricultureCloud.Models
{
    /// <summary>
    /// 供应商状态
    /// </summary>
    public enum ProviderStatus
    {
        等待审核,
        审核驳回,
        审核通过
    }

    public class Provider
    {
        public int ID { get; set; }

        /// <summary>
        /// 供应商名称
        /// </summary>
        [StringLength(128)]
        [Index]
        public string Title { get; set; }

        /// <summary>
        /// 供应商介绍
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 供应商地址
        /// </summary>
        [StringLength(256)]
        [Index]
        public string Address { get; set; }

        /// <summary>
        /// 营业执照
        /// </summary>
        public byte[] BusinessLicence { get; set; }

        /// <summary>
        /// 税务登记证
        /// </summary>
        public byte[] TaxRegistrationCertificate { get; set; }

        /// <summary>
        /// 组织机构代码证
        /// </summary>
        public byte[] OrganizationRegistrationCodeCertificate { get; set; }

        /// <summary>
        /// 法人身份证
        /// </summary>
        public byte[] ArtificialPersonIdentityCard { get; set; }

        /// <summary>
        /// 供应商状态
        /// </summary>
        [Index]
        public ProviderStatus Status { get; set; }

        /// <summary>
        /// 驳回原因
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// 申请时间
        /// </summary>
        [Index]
        public DateTime Time { get; set; }

        /// <summary>
        /// 座机
        /// </summary>
        public string Tel { get; set; }

        /// <summary>
        /// 手机
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 电子邮箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 说明图片
        /// </summary>
        public byte[] Picture { get; set; }

        /// <summary>
        /// 申请者ID
        /// </summary>
        [ForeignKey("User")]
        public int? UserID { get; set; }

        public virtual User User { get; set; }
           
        /// <summary>
        /// 删除标识
        /// </summary>
        [Index]
        public bool Delete { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
