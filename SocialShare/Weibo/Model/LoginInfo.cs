using System;
using System.Runtime.Serialization;

namespace SocialShare.Weibo.Model
{
    [DataContract]
    public class LoginInfo
    {
        [DataMember(Name = "access_token")]
        public string AccessToken { get; set; }
        [DataMember(Name = "refresh_token")]
        public string RefreshToken { get; set; } = string.Empty;
        [DataMember(Name = "user")]
        public string User { get; set; }
        [DataMember(Name = "expires_in")]
        public long ExpiresIn { get; set; }
        [DataMember(Name = "source")]
        public string Source { get; set; } = "sina";
        public long ExpiresAt { get; set; }

        //public string access_token { get; set; }
        //public string refresh_token { get; set; } = string.Empty;
        //public string user { get; set; }
        //public long expires_in { get; set; }
        //public string source { get; set; } = "sina";
        public bool CheckUseable()
        {
            return !string.IsNullOrEmpty(AccessToken) &&
                !string.IsNullOrEmpty(User) &&
                Untils.FromTimestamp(ExpiresAt) > DateTime.Now;
        }
    }
}
