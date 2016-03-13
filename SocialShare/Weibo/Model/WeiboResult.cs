﻿using System.Runtime.Serialization;

namespace SocialShare.Weibo.Model
{
    [DataContract]
    public class WeiboResult : ShareResultBase
    {
        /// <summary>
        /// 微博创建时间
        /// </summary>
        [DataMember(Name ="created_at")]
        public string CreatedAt
        {
            get;
            set;
        }

        /// <summary>
        /// 微博ID
        /// </summary>
        [DataMember(Name ="id")]
        public long Id
        {
            get;
            set;
        }

        /// <summary>
        /// 微博MID
        /// </summary>
        [DataMember(Name ="mid")]
        public string Mid
        {
            get;
            set;
        }

        /// <summary>
        /// 字符串型的微博ID
        /// </summary>
        [DataMember(Name ="idstr")]
        public string IdStr
        {
            get;
            set;
        }

        /// <summary>
        /// 微博消息内容
        /// </summary>
        [DataMember(Name ="text")]
        public string Text
        {
            get;
            set;
        }

        /// <summary>
        /// 微博来源
        /// </summary>
        [DataMember(Name ="source")]
        public string Source
        {
            get;
            set;
        }

        /// <summary>
        /// 是否已收藏，true：是；false：否
        /// </summary>
        [DataMember(Name ="favorited")]
        public bool Favorited
        {
            get;
            set;
        }

        /// <summary>
        /// 是否被截断，true：是；false：否
        /// </summary>
        [DataMember(Name ="truncated")]
        public bool Truncated
        {
            get;
            set;
        }

        /// <summary>
        /// 缩略图片地址，没有时不返回此字段
        /// </summary>
        [DataMember(Name ="thumbnail_pic")]
        public string ThumbnailPic
        {
            get;
            set;
        }

        /// <summary>
        /// 中等尺寸图片地址，没有时不返回此字段
        /// </summary>
        [DataMember(Name ="bmiddle_pic")]
        public string BmiddlePic
        {
            get;
            set;
        }

        /// <summary>
        /// 原始图片地址，没有时不返回此字段
        /// </summary>
        [DataMember(Name ="original_pic")]
        public string OriginalPic
        {
            get;
            set;
        }

        /// <summary>
        /// 微博作者的用户信息字段
        /// </summary>
        [DataMember(Name ="user")]
        public WeiboUser User
        {
            get;
            set;
        }

        /// <summary>
        /// 转发数
        /// </summary>
        [DataMember(Name ="reposts_count")]
        public int RepostsCount
        {
            get;
            set;
        }

        /// <summary>
        /// 评论数
        /// </summary>
        [DataMember(Name ="comments_count")]
        public int CommentsCount
        {
            get;
            set;
        }

        /// <summary>
        /// 表态数
        /// </summary>
        [DataMember(Name ="attitudes_count")]
        public int AttitudesCount
        {
            get;
            set;
        }

        /// <summary>
        /// 微博的可见性及指定可见分组信息。该object中type取值，0：普通微博，1：私密微博，3：指定分组微博，4：密友微博；list_id为分组的组号
        /// </summary>
        [DataMember(Name ="visible")]
        public Visible Visible
        {
            get;
            set;
        }
    }
}
