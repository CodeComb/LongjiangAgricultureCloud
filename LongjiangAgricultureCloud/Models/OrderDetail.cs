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

        [ForeignKey("Order")]
        public Guid OrderID { get; set; }

        public virtual Order Order { get; set; }

        [Index]
        public float Price { get; set; }

        [Index]
        public int Count { get; set; }

        [ForeignKey("Product")]
        public int ProductID { get; set; }

        public virtual Product Product { get; set; }
    }
}