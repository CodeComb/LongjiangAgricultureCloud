﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;

namespace LongjiangAgricultureCloud.Models
{
    public enum OrderStatus
    {
        已取消,
        待付款,
        待发货,
        待收货,
        待评价,
        退款中,
        已完成
    }

    public enum PayMethod
    {
        支付宝,
        微信支付
    }

    public class Order
    {
        public Guid ID { get; set; }

        [Index]
        public DateTime Time { get; set; }

        [Index]
        public OrderStatus Status { get; set; }

        [ForeignKey("User")]
        public int UserID { get; set; }

        public virtual User User { get; set; }

        [Index]
        public PayMethod PayMethod { get; set; }

        [Index]
        public string PayCode { get; set; }

        public string Hint { get; set; }
    }
}