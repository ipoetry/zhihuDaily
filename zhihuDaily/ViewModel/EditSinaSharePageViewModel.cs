using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using zhihuDaily.DataService;
using zhihuDaily.Model;

namespace zhihuDaily.ViewModel
{
    class EditSinaSharePageViewModel:ViewModelBase
    {
        private RelayCommand<string> _sendShareCommand;

        private ShareObject _shareObject;

        public ShareObject ShareObject
        {
            get { return _shareObject; }
            set {
                _shareObject = value;
                RaisePropertyChanged(()=> ShareObject);
            }
        }

        public int SelectionStart { get; set; }

        public EditSinaSharePageViewModel(ShareObject shareObject)
        {
            _shareObject = shareObject;
            SelectionStart = shareObject.Text.Length;
        }

        private bool isActive = false;
        public bool IsActive
        {
            get { return isActive; }
            set
            {
                isActive = value;
                RaisePropertyChanged(() => IsActive);
            }
        }

        public RelayCommand<string> SendShareCommand
        {
            get
            {
                return _sendShareCommand
                       ?? (_sendShareCommand = new RelayCommand<string>(
                           async (content) =>
                           {
                               IsActive = true;
                               ShareObject.Text = ShareObject.Text + ShareObject.ShareUrl;
                               string jsonShare = JsonConvertHelper.JsonSerializer(ShareObject);
                               string resJson = await WebProvider.GetInstance().SendPostRequestAsync("https://news-at.zhihu.com/api/4/share", jsonShare, WebProvider.ContentType.ContentType3);
                               if (string.IsNullOrEmpty(resJson))
                               {
                                   ToastPrompt.ShowToast("分享成功");
                               }
                               else
                               {
                                   try
                                   {
                                       var jsonObj = JsonObject.Parse(resJson);
                                       double status = jsonObj["status"].GetNumber();
                                       if (status > 0)
                                           ToastPrompt.ShowToast(jsonObj["error_msg"].GetString());
                                       //else
                                       //ToastPrompt.ShowToast(jsonObj["debug_info"].GetString());
                                   }
                                   catch { }
                               }
                               IsActive = false;
                               Messenger.Default.Send(new NotificationMessage("gobackSharePage"));
                           }));
            }
        }
    }
}
