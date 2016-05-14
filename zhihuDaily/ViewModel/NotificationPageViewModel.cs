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
    class NotificationPageViewModel: ViewModelBase
    {
        public NotificationPageViewModel()
        {
            LoadContent();
        }

        private List<Notification> notifications;

        public List<Notification> Notifications
        {
            get { return notifications; }
            set {
                notifications = value;
                RaisePropertyChanged(()=> Notifications);
            }
        }


        public async void LoadContent()
        {
           string resJosn = await WebProvider.GetInstance().GetRequestDataAsync("http://news-at.zhihu.com/api/4/notifications");
            if (resJosn != string.Empty)
            {
               var jsonObj = Windows.Data.Json.JsonObject.Parse(resJosn);
               Notifications = JsonConvertHelper.JsonDeserialize<List<Notification>>(jsonObj["notifications"].ToString());
            }
        }
    }
}
