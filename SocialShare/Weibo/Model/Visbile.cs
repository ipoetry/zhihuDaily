using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SocialShare.Weibo.Model
{
    [DataContract]
    public class Visible
    {
        [DataMember(Name ="type")]
        public int Type
        {
            get;
            set;
        }

        [DataMember(Name ="list_id")]
        public int ListId
        {
            get;
            set;
        }
    }
}
