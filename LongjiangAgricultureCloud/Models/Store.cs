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

        /// <summary>
        /// 仓库名称
        /// </summary>
        [StringLength(128)]
        [Index]
        public string Title { get; set; }

        /// <summary>
        /// 库管员ID
        /// </summary>
        [ForeignKey("User")]
        public int? UserID { get; set; }

        public virtual User User { get; set; }

        /// <summary>
        /// 删除标识
        /// </summary>
        [Index]
        public bool Delete { get; set; }
    }
}