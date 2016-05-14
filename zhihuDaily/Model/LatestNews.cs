using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace zhihuDaily.Model
{
    [DataContract]
    public class LatestNews: NotificationObject
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
    //[DataContract]
    //public class TopStory: NotificationObject
    //{
    //    private string _image;
    //    [DataMember(Name = "image")]
    //    public string Image
    //    {
    //        get
    //        {
    //            return _image;
    //        }
    //        set
    //        {
    //            _image = value;
    //            RaisePropertyChanged(() => Image);
    //        }
    //    }

    //    [DataMember(Name = "type")]
    //    public int Type { get; set; }

    //    [DataMember(Name = "id")]
    //    public int Id { get; set; }

    //    [DataMember(Name = "ga_prefix")]
    //    public string GaPrefix { get; set; }
    //    private string _title;
    //    [DataMember(Name = "title")]
    //    public string Title
    //    {
    //        get
    //        {
    //            return _title;
    //        }
    //        set
    //        {
    //            _title = value;
    //            RaisePropertyChanged(() => Title);
    //        }
    //    }
    //}
    [DataContract]
    public class Story:NotificationObject
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
                RaisePropertyChanged(() => Image);
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
                RaisePropertyChanged(() => Title);
            }
        }

        [DataMember(Name = "multipic")]
        public bool? Multipic { get; set; }

        public DateTime Date { get; set; }

        public Windows.UI.Xaml.Visibility IsDateTitleDisplay { get; set; } 

        public Windows.UI.Xaml.Visibility IsStoryItemDisplay { get; set; } 
    }

    [DataContract]
    public class ShareObject
    {
        [DataMember(Name = "text")]
        public string Text { get; set; }
        [DataMember(Name = "service")]
        public string Service { get; set; }
        [DataMember(Name = "story_id")]
        public int StoryId { get; set; }
        [DataMember(Name = "include_screenshot")]
        public bool IncludeScreenshot { get; set; }

        public string ShareUrl { get; set; }
    }
}
