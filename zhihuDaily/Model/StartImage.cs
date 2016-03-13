using System.Runtime.Serialization;

namespace zhihuDaily.Model
{
    [DataContract]
    public class StartImage:NotificationObject
    {
        private string _text;
        [DataMember(Name = "text")]
        public string Text
        {
            set
            {
                _text = value;
                RaisePropertyChanged(() => Text);
            }
            get
            {
                return _text;
            }
        }
        private string _img;
        [DataMember(Name = "img")]
        public string Img
        {
            set
            {
                _img = value;
                RaisePropertyChanged(()=> Img);
            }
            get
            {
                return _img;
            }
        }
    }
}
