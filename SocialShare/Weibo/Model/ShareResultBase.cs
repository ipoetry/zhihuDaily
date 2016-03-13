using System.Collections.Generic;
using System.Runtime.Serialization;
namespace SocialShare.Weibo.Model
{
    [DataContract]
    public class ShareResultBase
    {
        [DataMember(Name = "errort")]
        public string Error
        {
            get;
            set;
        }

        [DataMember(Name ="error_code")]
        public int ErrorCode
        {
            get;
            set;
        }

        [DataMember(Name ="request")]
        public string Request
        {
            get;
            set;
        }

        public bool IsSuccess
        {
            get
            {
                return ErrorCode <= 0;
            }
        }
    }
}
