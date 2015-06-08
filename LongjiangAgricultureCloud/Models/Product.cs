using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;

namespace LongjiangAgricultureCloud.Models
{
    public class Product
    {
        /// <summary>
        /// 商品ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 规格
        /// </summary>
        public string Standard { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// 单价
        /// </summary>
        public float Price { get; set; }

        /// <summary>
        /// 库存剩余数量
        /// </summary>
        public int Store { get; set; }
        
        /// <summary>
        /// 供应商
        /// </summary>
        [ForeignKey("Provider")]
        public int ProviderID { get; set; }

        public virtual Provider Provider { get; set; }

        public byte[] Picture1 { get; set; }
        public byte[] Picture2 { get; set; }
        public byte[] Picture3 { get; set; }
        public byte[] Picture4 { get; set; }
        public byte[] Picture5 { get; set; }

        [ForeignKey("Catalog")]
        public int CatalogID { get; set; }

        public virtual Catalog Catalog { get; set; }

        /// <summary>
        /// 商品编码
        /// </summary>
        [Index(IsUnique = false)]
        public string ProductCode { get; set; }
    }
}