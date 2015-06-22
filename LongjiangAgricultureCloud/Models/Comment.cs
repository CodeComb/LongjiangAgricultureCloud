using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;

namespace LongjiangAgricultureCloud.Models
{
    /// <summary>
    /// 评论类型
    /// </summary>
    public enum CommentType
    {
        商品评论,
        本地通评论
    }

    public class Comment
    {
        /// <summary>
        /// 评论ID
        /// </summary>
        public Guid ID { get; set; }

        /// <summary>
        /// 评论者ID
        /// </summary>
        [ForeignKey("User")]
        public int? UserID { get; set; }

        public virtual User User { get; set; }

        /// <summary>
        /// 评论发表时间
        /// </summary>
        [Index]
        public DateTime Time { get; set; }

        /// <summary>
        /// 评论内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 评论所属实体ID（比如商品ID、本地通信息ID）
        /// </summary>
        [Index]
        public int TargetID { get; set; }

        /// <summary>
        /// 评分（仅对商品）
        /// </summary>
        [Index]
        public int? Score { get; set; }

        /// <summary>
        /// 评论类型
        /// </summary>
        [Index]
        public CommentType Type { get; set; }

        /// <summary>
        /// 审核标识
        /// </summary>
        [Index]
        public bool Verify { get; set; }
    }
}