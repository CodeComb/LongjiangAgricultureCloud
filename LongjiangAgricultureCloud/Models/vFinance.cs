using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LongjiangAgricultureCloud.Models
{
    public class vService
    {
        public string Service { get; set; }
        public float Price { get; set; }
        public string Manager { get; set; }
    }

    public class vArea
    {
        public string Manager { get; set; }
        public float Price { get; set; }
        public List<vService> Service { get; set; }
    }
}