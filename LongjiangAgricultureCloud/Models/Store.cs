using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LongjiangAgricultureCloud.Models
{
    public class Store
    {
        public int ID { get; set; }

        [StringLength(128)]
        [Index]
        public string Title { get; set; }

        [ForeignKey("User")]
        public int? UserID { get; set; }

        public virtual User User { get; set; }
    }
}