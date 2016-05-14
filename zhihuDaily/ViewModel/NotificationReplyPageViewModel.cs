using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using zhihuDaily.DataService;
using zhihuDaily.Model;

namespace zhihuDaily.ViewModel
{
    class NotificationReplyPageViewModel:ViewModelBase
    {
        private NotificationReply _notificationReply;
        public NotificationReply NotificationReply
        {
            get { return _notificationReply; }
            set {
                _notificationReply = value;
                RaisePropertyChanged(()=> NotificationReply);
            }
        }

        public NotificationReplyPageViewModel(long commentId)
        {
            LoadContent(commentId);
        }

        public async void LoadContent(long commentId)
        {
            string resJosn = await WebProvider.GetInstance().GetRequestDataAsync($"http://news-at.zhihu.com/api/4/comment/{commentId}/replies");
            if (resJosn != string.Empty)
            {
                NotificationReply = JsonConvertHelper.JsonDeserialize<NotificationReply>(resJosn);
            }
        }
    }
}
