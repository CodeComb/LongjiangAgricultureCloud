using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace LongjiangAgricultureCloud.Models
{
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

    public enum SupplyDemand
    {
        供,
        求
    }

    public class Information
    {
        public int ID { get; set; }

        public string Title { get; set; }

        public string Name { get; set; }

        public string Phone { get; set; }

        [Index]
        public bool Verify { get; set; }

        public string Description { get; set; }

        public decimal? Lon { get; set; }

        public decimal? Lat { get; set; }

        public float Price { get; set; }

        public byte[] Picture { get; set; }

        public string Address { get; set; }

        [ForeignKey("User")]
        public int? UserID { get; set; }

        public virtual User User { get; set; }

        [ForeignKey("Catalog")]
        public int? CatalogID { get; set; }

        public virtual Catalog Catalog { get; set; }

        [Index]
        public DateTime Time { get; set; }

        public InformationType Type { get; set; }

        public SupplyDemand? SupplyDemand { get; set; }
    }
}
