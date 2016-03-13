using System;

namespace zhihuDaily.Model
{
    public class Theme_Style:NotificationObject
    {
        private int id;
        public int Id
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
                RaisePropertyChanged(()=>Id);
            }
        }
        private string title;
        public string Title
        {
            get { return title; }
            set {
                title = value;
                RaisePropertyChanged(()=>Title);
            }
        }
        public Uri PicUri { get; set; }
        public string TbMargin { get; set; } = "-20 0 0 0";
        public string FontColor { get; set; }
    }
}
