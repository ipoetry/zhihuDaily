using System.Runtime.Serialization;

namespace zhihuDaily.Model
{
    [DataContract]
    public class NewsContent
    {
        [DataMember(Name = "body")]
        public string Body { get; set; }

        [DataMember(Name = "image_source")]
        public string ImageSource { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "image")]
        public string Image { get; set; }

        [DataMember(Name = "share_url")]
        public string ShareUrl { get; set; }

        [DataMember(Name = "js")]
        public object[] Js { get; set; }

        [DataMember(Name = "ga_prefix")]
        public string GaPrefix { get; set; }

        [DataMember(Name = "type")]
        public int Type { get; set; }

        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "css")]
        public string[] Css { get; set; }
    }
}
