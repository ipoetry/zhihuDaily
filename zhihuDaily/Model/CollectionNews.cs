using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace zhihuDaily.Model
{
    [DataContract]
    class CollectionNews : NotificationObject
    {
        [DataMember(Name = "count")]
        public int Count { get; set; }
        [DataMember(Name = "last_time")]
        public long LastTime { get; set; }
        [DataMember(Name = "stories")]
        public List<Story> Stories { get; set; } = new List<Story>();

    }
}
