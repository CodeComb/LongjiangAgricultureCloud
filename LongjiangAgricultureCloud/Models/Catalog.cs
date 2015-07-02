using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;

namespace LongjiangAgricultureCloud.Models
{
    /// <summary>
    /// 分类类别
    /// </summary>
    public enum CatalogType
    {
        商品分类,
        农机服务分类,
        本地通分类,
        农业信息分类
    }

    public class Catalog
    {
        public int ID { get; set; }

        /// <summary>
        /// 类别
        /// </summary>
        public CatalogType Type { get; set; }

        /// <summary>
        /// 分类级别
        /// </summary>
        [Index]
        public int Level { get; set; }

        /// <summary>
        /// 分类名称
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 父类ID
        /// </summary>
        [ForeignKey("Father")]
        public int? FatherID { get; set; }

        public virtual Catalog Father { get; set; }

        /// <summary>
        /// 子类集合
        /// </summary>
        public virtual ICollection<Catalog> Catalogs { get; set; }

        /// <summary>
        /// 删除标识
        /// </summary>
        [Index]
        public bool Delete { get; set; }

        public bool Commentable { get; set; }
    }
}