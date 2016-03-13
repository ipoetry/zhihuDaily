
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace zhihuDaily.Model
{
    [DataContract]
    public class Themes:NotificationObject
    {
        [DataMember(Name = "limit")]
        public int Limit { get; set; }

        [DataMember(Name = "subscribed")]
        public object[] Subscribed { get; set; }

        [DataMember(Name = "others")]
        private IEnumerable<Others> others;
        public IEnumerable<Others> Others
        {
            get { return others; }
            set {
                others = value;
                RaisePropertyChanged(()=>Others);
            }
        }
    }
    [DataContract]
    public class Others
    {

        [DataMember(Name = "color")]
        public int Color { get; set; }

        [DataMember(Name = "thumbnail")]
        public string Thumbnail { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }
    }
}
