using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LongjiangAgricultureCloud.Models
{
    public enum UserRole
    {
        普通用户,
        服务站,
        大区经理,
        库存管理员,
        系统管理员
    }

    public class User
    {
        public int ID { get; set; }

        [Index(IsUnique = true)]
        [StringLength(32)]
        public string Username { get; set; }

        public string Password { get; set; }

        public string Question { get; set; }

        public string Answer { get; set; }

        [ForeignKey("Area")]
        public int? AreaID { get; set; }

        public virtual Area Area { get; set; }

        public UserRole Role { get; set; }

        public virtual ICollection<Provider> Providers { get; set; }
    }
}