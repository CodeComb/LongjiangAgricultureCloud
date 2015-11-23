using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LongjiangAgricultureCloud.Models
{
    /// <summary>
    /// 用户角色
    /// </summary>
    public enum UserRole
    {
        普通用户,
        服务站,
        大区经理,
        库存管理员,
        配送管理员,
        报表统计员,
        信息审核员,
        系统管理员
    }

    public class User
    {
        public int ID { get; set; }

        /// <summary>
        /// 用户名/手机号
        /// </summary>
        [Index(IsUnique = true)]
        [StringLength(32)]
        public string Username { get; set; }

        /// <summary>
        /// 密码SHA1
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 密码提示问题
        /// </summary>
        public string Question { get; set; }

        /// <summary>
        /// 密码问题答案
        /// </summary>
        public string Answer { get; set; }

        /// <summary>
        /// 真实姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 地区ID
        /// </summary>
        [ForeignKey("Area")]
        public int? AreaID { get; set; }

        public virtual Area Area { get; set; }

        /// <summary>
        /// 详细地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 邮政编码
        /// </summary>
        public string PostCode { get; set; }

        /// <summary>
        /// 用户角色
        /// </summary>
        public UserRole Role { get; set; }

        /// <summary>
        /// 删除标记
        /// </summary>
        public bool Delete { get; set; }
        
        [Index]
        public int? ManagerID { get; set; }

        [NotMapped]
        public User Manager
        {
            get
            {
                using (LongjiangAgricultureCloudContext DB = new LongjiangAgricultureCloudContext())
                {
                    if (ManagerID.HasValue)
                        return DB.Users.Find(ManagerID);
                    else
                        return null;
                }
            }
        } 

        public virtual ICollection<Provider> Providers { get; set; }

        public virtual ICollection<Store> Stores { get; set; }
    }
}