using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BackgroundTaskLibrary
{
    [DataContract]
    public sealed class LatestNews
    {
        [DataMember(Name = "date")]
        public string Date { get; set; }

        [DataMember(Name = "stories")]
        public IEnumerable<Story> Stories { get; set; }

        private IEnumerable<Story> top_stories;
        [DataMember(Name = "top_stories")]
        public IEnumerable<Story> TopStories
        {
            get
            {
                return top_stories;
            }
            set
            {
                top_stories = value;
               // RaisePropertyChanged(()=> TopStories);
            }
        }
    }

    [DataContract]
    public sealed class Story
    {
        private string _image;

        [DataMember(Name = "image")]  
        public string Image
        {
            get
            {
                return _image;
            }
            set
            {
                _image = value;
               // RaisePropertyChanged(() => Image);
            }
        }

        [DataMember(Name = "images")]
        public string[] Images { get; set; }

        [DataMember(Name = "type")]
        public int Type { get; set; }

        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "ga_prefix")]
        public string GaPrefix { get; set; }

        private string _title;
        [DataMember(Name = "title")]
        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
              //  RaisePropertyChanged(() => Title);
            }
        }

        [DataMember(Name = "multipic")]
        public bool? Multipic { get; set; }
    }
}
