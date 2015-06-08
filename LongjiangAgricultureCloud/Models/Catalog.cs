using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;

namespace LongjiangAgricultureCloud.Models
{
    public enum CatalogType
    {
        Product
    }

    public class Catalog
    {
        public int ID { get; set; }

        public CatalogType Type { get; set; }

        [Index]
        public int Level { get; set; }
    }
}