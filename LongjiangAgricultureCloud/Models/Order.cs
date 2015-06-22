using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LongjiangAgricultureCloud.Models
{
    /// <summary>
    /// 订单状态
    /// </summary>
    public enum OrderStatus
    {
        已取消,
        待付款,
        待发货,
        待收货,
        待评价,
        退款中,
        退款驳回,
        已退货,
        已完成
    }

    /// <summary>
    /// 支付类型
    /// </summary>
    public enum PayMethod
    {
        支付宝,
        微信支付
    }

    public class Order
    {
        public Guid ID { get; set; }

        /// <summary>
        /// 下单时间
        /// </summary>
        [Index]
        public DateTime Time { get; set; }

        /// <summary>
        /// 订单状态
        /// </summary>
        [Index]
        public OrderStatus Status { get; set; }

        /// <summary>
        /// 下单者ID
        /// </summary>
        [ForeignKey("User")]
        public int UserID { get; set; }

        public virtual User User { get; set; }

        /// <summary>
        /// 支付方式
        /// </summary>
        [Index]
        public PayMethod PayMethod { get; set; }

        /// <summary>
        /// 收货地址
        /// </summary>
        [Index]
        [StringLength(512)]
        public string Address { get; set; }
        
        /// <summary>
        /// 支付代码
        /// </summary>
        [Index]
        [StringLength(256)]
        public string PayCode { get; set; }

        /// <summary>
        /// 买家备注
        /// </summary>
        public string Hint { get; set; }

        public virtual ICollection<OrderDetail> OrderDetails { get; set; }

        /// <summary>
        /// 发票姓名
        /// </summary>
        public string InvoiceName { get; set; }

        /// <summary>
        /// 发票地址
        /// </summary>
        public string InvoiceAddress { get; set; }

        /// <summary>
        /// 退货原因
        /// </summary>
        public string GiveBackReason { get; set; }
    }
}