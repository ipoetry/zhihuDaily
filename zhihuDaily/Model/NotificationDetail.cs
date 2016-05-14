using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace zhihuDaily.Model
{
    [DataContract]
    class NotificationDetail
    {
        [DataMember(Name = "comment")]
        public Comment Comment { get; set; }
        [DataMember(Name = "story")]
        public Story Story { get; set; }
        [DataMember(Name = "users")]
        public List<User> Users { get; set; }
    }

    [DataContract]
    class NotificationReply
    {
        [DataMember(Name = "story")]
        public Story Story { get; set; }
        [DataMember(Name = "origin_comment")]
        public Comment OriginComment { get; set; }
        [DataMember(Name = "replies")]
        public List<ReplyTo> Replies { get; set; }
    }
}
