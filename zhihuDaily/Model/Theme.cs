using System.Collections.Generic;
using System.Runtime.Serialization;
using GalaSoft.MvvmLight;
namespace zhihuDaily.Model
{
    [DataContract]
    public class Editor
    {

        [DataMember(Name = "url")]
        public string Url { get; set; }

        [DataMember(Name = "bio")]
        public string Bio { get; set; }

        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "avatar")]
        public string Avatar { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }
    }

    [DataContract]
    public class Theme : NotificationObject
    {
        private IEnumerable<Story> stories;
        [DataMember(Name = "stories")]
        public IEnumerable<Story> Stories
        {
            get
            {
                return stories;
            }
            set
            {
                stories = value;
                RaisePropertyChanged(() => Stories);
            }
        }

        [DataMember(Name = "description")]
        public string Description { get; set; }
        private string background;
        [DataMember(Name = "background")]
        public string Background
        {
            get { return background; }
            set {
                background = value;
                RaisePropertyChanged(()=>Background);
                }
        }

        [DataMember(Name = "color")]
        public int Color { get; set; }

        private string name;
        [DataMember(Name = "name")]
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                RaisePropertyChanged(() => Name);
            }
        }

        [DataMember(Name = "image")]
        public string Image { get; set; }

        private IEnumerable<Editor> editors;
        [DataMember(Name = "editors")]
        public IEnumerable<Editor> Editors
        {
            get
            {
                return editors;
            }
            set
            {
                editors = value;
                RaisePropertyChanged(() => Editors);
            }
        }

        [DataMember(Name = "image_source")]
        public string ImageSource { get; set; }
    }

}

