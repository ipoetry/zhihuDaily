using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SocialShare.Weibo.Model
{
    [DataContract]
    class TokenResult
    {
        [DataMember(Name = "access_token")]
        public string AccessToken { get; set; }
        [DataMember(Name = "uid")]
        public string Uid { get; set; }
        [DataMember(Name = "expires_in")]
        public long ExpiresIn { get; set; }
        [DataMember(Name = "remind_in")]
        public long RemindIn { get; set; }
        
    }
}
