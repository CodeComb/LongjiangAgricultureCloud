using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;

namespace LongjiangAgricultureCloud.Models
{
    public enum AreaLevel
    {
        省,
        市,
        区县,
        乡镇,
        村,
        屯
    }

    public class Area
    {
        public int ID { get; set; }

        public string Title { get; set; }
        
        [ForeignKey("Father")]
        public int? FatherID { get; set; }

        public virtual Area Father { get; set; }

        public AreaLevel Level { get; set; }

        public virtual ICollection<Area> Children { get; set; }

        public virtual ICollection<User> Users { get; set; }
    }
}