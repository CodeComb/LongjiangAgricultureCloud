using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;

namespace LongjiangAgricultureCloud.Models
{
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

        public CatalogType Type { get; set; }

        [Index]
        public int Level { get; set; }

        public string Title { get; set; }
    }
}