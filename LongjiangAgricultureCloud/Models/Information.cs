using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LongjiangAgricultureCloud.Models
{
    /// <summary>
    /// 信息类型
    /// </summary>
    public enum InformationType
    {
        农机找活,
        土地找机手,
        附近农手,
        二手农机,
        维修站,
        本地通信息,
        农业信息
    }

    /// <summary>
    /// 供求类型
    /// </summary>
    public enum SupplyDemand
    {
        供,
        求
    }

    public class Information
    {
        public int ID { get; set; }

        /// <summary>
        /// 信息标题
        /// </summary>
        [StringLength(256)]
        [Index]
        public string Title { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 审核标识
        /// </summary>
        [Index]
        public bool Verify { get; set; }

        /// <summary>
        /// 详细内容
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 经度
        /// </summary>
        public decimal? Lon { get; set; }

        /// <summary>
        /// 纬度
        /// </summary>
        public decimal? Lat { get; set; }

        /// <summary>
        /// 价格
        /// </summary>
        public string Price { get; set; }

        /// <summary>
        /// 维修站资质照片
        /// </summary>
        public byte[] Picture { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 信息发布者ID
        /// </summary>
        [ForeignKey("User")]
        public int? UserID { get; set; }

        public virtual User User { get; set; }

        /// <summary>
        /// 类别ID
        /// </summary>
        [ForeignKey("Catalog")]
        public int? CatalogID { get; set; }

        public virtual Catalog Catalog { get; set; }

        /// <summary>
        /// 发布时间
        /// </summary>
        [Index]
        public DateTime Time { get; set; }

        /// <summary>
        /// 信息类型
        /// </summary>
        public InformationType Type { get; set; }

        /// <summary>
        /// 供求类型
        /// </summary>
        public SupplyDemand? SupplyDemand { get; set; }

        /// <summary>
        /// 是否置顶
        /// </summary>
        [Index]
        public bool Top { get; set; }

        public string VideoURL { get; set; }
    }
}
