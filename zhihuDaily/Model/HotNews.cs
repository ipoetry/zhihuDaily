using System.Collections.Generic;
using System.Runtime.Serialization;

namespace zhihuDaily.Model
{
    [DataContract]
    public class HotNews
    {
        [DataMember(Name = "recent")]
        public IEnumerable<Recent> Recent { get; set; }
    }
    [DataContract]
    public class Recent
    {
        [DataMember(Name = "news_id")]
        public int Id { get; set; }

        [DataMember(Name = "url")]
        public string Url { get; set; }

        [DataMember(Name = "thumbnail")]
        public string Thumbnail { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }
    }

}
