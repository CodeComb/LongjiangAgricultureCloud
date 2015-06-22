using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;

namespace LongjiangAgricultureCloud.Models
{
    /// <summary>
    /// 地区级别
    /// </summary>
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

        /// <summary>
        /// 地区名称
        /// </summary>
        public string Title { get; set; }
        
        /// <summary>
        /// 父地区ID
        /// </summary>
        [ForeignKey("Father")]
        public int? FatherID { get; set; }

        public virtual Area Father { get; set; }

        /// <summary>
        /// 地区级别
        /// </summary>
        public AreaLevel Level { get; set; }

        /// <summary>
        /// 子地区
        /// </summary>
        public virtual ICollection<Area> Children { get; set; }

        /// <summary>
        /// 该地区的用户集合
        /// </summary>
        public virtual ICollection<User> Users { get; set; }
    }
}