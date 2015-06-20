using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LongjiangAgricultureCloud.Models
{
    public class vConfig
    {
        public bool VerifyLocalTongComment { get; set; }
        public bool InformationComment { get; set; }
        public bool VerifyProductComment { get; set; }
        public bool VerifyService { get; set; }
        public bool VerifyLocalTong { get; set; }
        public string ServiceTel { get; set; }
    }
}