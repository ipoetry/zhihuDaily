using System.Runtime.Serialization;

namespace SocialShare.Weibo.Model
{
    [DataContract]
    public class WeiboUser
    {
        /// <summary>
        /// 用户UID
        /// </summary>
        [DataMember(Name ="id")]
        public long Id
        {
            get;
            set;
        }

        /// <summary>
        /// 字符串型的用户UID
        /// </summary>
        [DataMember(Name ="idstr")]
        public string IdStr
        {
            get;
            set;
        }

        /// <summary>
        /// 用户昵称
        /// </summary>
        [DataMember(Name ="screen_name")]
        public string ScreenName
        {
            get;
            set;
        }

        /// <summary>
        /// 友好显示名称
        /// </summary>
        [DataMember(Name ="name")]
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// 用户所在省级ID
        /// </summary>
        [DataMember(Name ="province")]
        public string Province
        {
            get;
            set;
        }

        /// <summary>
        /// 用户所在城市ID
        /// </summary>
        [DataMember(Name ="city")]
        public string City
        {
            get;
            set;
        }

        /// <summary>
        /// 用户所在地
        /// </summary>
        [DataMember(Name ="location")]
        public string Location
        {
            get;
            set;
        }

        /// <summary>
        /// 用户个人描述
        /// </summary>
        [DataMember(Name ="decription")]
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// 用户博客地址
        /// </summary>
        [DataMember(Name ="url")]
        public string Url
        {
            get;
            set;
        }

        /// <summary>
        /// 用户头像地址（中图），50×50像素
        /// </summary>
        [DataMember(Name ="profile_image_url")]
        public string ProfileImageUrl
        {
            get;
            set;
        }

        /// <summary>
        /// 用户的微博统一URL地址
        /// </summary>
        [DataMember(Name ="profile_url")]
        public string ProfileUrl
        {
            get;
            set;
        }

        /// <summary>
        /// 用户的个性化域名
        /// </summary>
        [DataMember(Name ="domain")]
        public string Domain
        {
            get;
            set;
        }

        /// <summary>
        /// 用户的微号
        /// </summary>
        [DataMember(Name ="weihao")]
        public string Weihao
        {
            get;
            set;
        }

        /// <summary>
        /// 性别，m：男、f：女、n：未知
        /// </summary>
        [DataMember(Name ="gender")]
        public string Gender
        {
            get;
            set;
        }

        /// <summary>
        /// 粉丝数
        /// </summary>
        [DataMember(Name ="followers_count")]
        public int FollowersCount
        {
            get;
            set;
        }

        /// <summary>
        /// 关注数
        /// </summary>
        [DataMember(Name ="friends_count")]
        public int FriendsCount
        {
            get;
            set;
        }

        /// <summary>
        /// 微博数
        /// </summary>
        [DataMember(Name ="statuses_count")]
        public int StatusesCount
        {
            get;
            set;
        }

        /// <summary>
        /// 收藏数
        /// </summary>
        [DataMember(Name ="favourites_count")]
        public int FavouritesCount
        {
            get;
            set;
        }

        /// <summary>
        /// 用户创建（注册）时间
        /// </summary>
        [DataMember(Name ="created_at")]
        public string CreatedAt
        {
            get;
            set;
        }

        /// <summary>
        /// 是否允许所有人给我发私信，true：是；false：否
        /// </summary>
        [DataMember(Name ="allow_all_act_msg")]
        public bool AllowAllActMsg
        {
            get;
            set;
        }

        /// <summary>
        /// 是否允许标识用户的地理位置，true：是；false：否
        /// </summary>
        [DataMember(Name ="geo_enabled")]
        public bool GeoEnabled
        {
            get;
            set;
        }

        /// <summary>
        /// 是否是微博认证用户，即加V用户，true：是；false：否
        /// </summary>
        [DataMember(Name ="verified")]
        public bool Verified
        {
            get;
            set;
        }

        /// <summary>
        /// 是否允许所有人对我的微博进行评论，true：是，false：否
        /// </summary>
        [DataMember(Name ="allow_all_comment")]
        public bool AllowAllComment
        {
            get;
            set;
        }

        /// <summary>
        /// 用户头像地址（大图），180×180像素
        /// </summary>
        [DataMember(Name ="avatar_large")]
        public string AvatarLarge
        {
            get;
            set;
        }

        /// <summary>
        /// 用户头像地址（高清），高清头像原图
        /// </summary>
        [DataMember(Name ="avatar_hd")]
        public string AvatarHD
        {
            get;
            set;
        }

        /// <summary>
        /// 认证原因
        /// </summary>
        [DataMember(Name ="verified_reason")]
        public string VerifiedReason
        {
            get;
            set;
        }

        /// <summary>
        /// 该用户是否关注当前登录用户，true：是，false：否
        /// </summary>
        [DataMember(Name ="follow_me")]
        public bool FollowMe
        {
            get;
            set;
        }

        /// <summary>
        /// 用户的在线状态，0：不在线、1：在线
        /// </summary>
        [DataMember(Name ="online_status")]
        public int OnlineStatus
        {
            get;
            set;
        }

        /// <summary>
        /// 用户的互粉数
        /// </summary>
        [DataMember(Name ="bi_followers_count")]
        public int BiFollowersCount
        {
            get;
            set;
        }

        /// <summary>
        /// 用户当前的语言版本，zh-cn：简体中文，zh-tw：繁体中文，en：英语
        /// </summary>
        [DataMember(Name ="lang")]
        public string Lang
        {
            get;
            set;
        }
    }
}
