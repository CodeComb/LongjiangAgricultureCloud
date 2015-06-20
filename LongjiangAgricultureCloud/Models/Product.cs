using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
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
        [StringLength(256)]
        [Index]
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
        [Index]
        public float Price { get; set; }

        /// <summary>
        /// 库存剩余数量
        /// </summary>
        public int StoreCount { get; set; }
        
        /// <summary>
        /// 供应商
        /// </summary>
        [ForeignKey("Provider")]
        public int? ProviderID { get; set; }

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
        [StringLength(256)]
        public string ProductCode { get; set; }

        [Index]
        public bool Delete { get; set; }

        [ForeignKey("Store")]
        public int StoreID { get; set; }

        public virtual Store Store { get; set; }

        public string Description { get; set; }

        [Index]
        public bool Top { get; set; }

        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}