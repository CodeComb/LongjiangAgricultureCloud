using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;

namespace LongjiangAgricultureCloud.Models
{
    public enum CommentType
    {
        商品评论,
        本地通评论
    }

    public class Comment
    {
        public Guid ID { get; set; }

        [ForeignKey("User")]
        public int? UserID { get; set; }

        public virtual User User { get; set; }

        [Index]
        public DateTime Time { get; set; }

        public string Content { get; set; }

        [Index]
        public int TargetID { get; set; }

        [Index]
        public int? Score { get; set; }

        [Index]
        public CommentType Type { get; set; }
    }
}