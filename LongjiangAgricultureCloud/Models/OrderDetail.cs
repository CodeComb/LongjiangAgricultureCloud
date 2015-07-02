using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;

namespace LongjiangAgricultureCloud.Models
{
    public class OrderDetail
    {
        public Guid ID { get; set; }

        /// <summary>
        /// 订单ID
        /// </summary>
        [ForeignKey("Order")]
        public Guid? OrderID { get; set; }

        public virtual Order Order { get; set; }

        /// <summary>
        /// 总价
        /// </summary>
        [Index]
        public float Price { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        [Index]
        public int Count { get; set; }

        /// <summary>
        /// 商品ID
        /// </summary>
        [ForeignKey("Product")]
        public int ProductID { get; set; }

        public virtual Product Product { get; set; }

        /// <summary>
        /// 下单者ID
        /// </summary>
        [ForeignKey("User")]
        public int UserID { get; set; }

        public virtual User User { get; set; }

        /// <summary>
        /// 发货标记
        /// </summary>
        [Index]
        public bool DistributeFlag { get; set; }
    }
}