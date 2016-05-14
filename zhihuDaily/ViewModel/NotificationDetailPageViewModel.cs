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
    class NotificationDetailPageViewModel:ViewModelBase
    {
        private NotificationDetail _notificationDetail;
        public NotificationDetail NotificationDetail
        {
            get { return _notificationDetail; }
            set {
                _notificationDetail = value;
                RaisePropertyChanged(()=> NotificationDetail);
            }
        }

        public int UsersCount { get; set; }
        public NotificationDetailPageViewModel(long commentId)
        {
            LoadContent(commentId);
        }

        public async void LoadContent(long commentId)
        {
            string resJosn = await WebProvider.GetInstance().GetRequestDataAsync($"http://news-at.zhihu.com/api/4/comment/{commentId}/votes");
            if (resJosn != string.Empty)
            {
                NotificationDetail = JsonConvertHelper.JsonDeserialize<NotificationDetail>(resJosn);
                UsersCount = NotificationDetail.Users.Count;
            }
        }

    }
}
