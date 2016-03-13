using System.Runtime.Serialization;

namespace zhihuDaily.Model
{
    [DataContract]
    public class StoryExtra
    {
        [DataMember(Name = "long_comments")]
        public int LongComments { get; set; }

        [DataMember(Name = "popularity")]
        public int Popularity { get; set; }

        [DataMember(Name = "short_comments")]
        public int ShortComments { get; set; }

        [DataMember(Name = "comments")]
        public int Comments { get; set; }
    }
}
