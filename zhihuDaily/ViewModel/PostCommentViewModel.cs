using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using SocialShare.Weibo;
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
    class PostCommentViewModel: ViewModelBase
    {
        private string _stroyId;
        private Comment _replyTo;
        private RelayCommand<string> _sendCommentCommand;
        public PostCommentViewModel(string storyId,string replyTo)
        {
            _stroyId = storyId;
            _replyTo = !string.IsNullOrEmpty(replyTo)? DataService.JsonConvertHelper.JsonDeserialize<Comment>(replyTo):null;
        }
        private string _commentContent;
        public string CommentContent {
            get { return _commentContent; }
            set {
                _commentContent = value;
                RaisePropertyChanged(()=> CommentContent);
            }
        }
        private bool isActive=false;
        public bool IsActive
        {
            get { return isActive; }
            set {
                isActive = value;
                RaisePropertyChanged(()=> IsActive);
            }
        }
        public RelayCommand<string> SendCommentCommand
        {
            get
            {
                return _sendCommentCommand
                       ?? (_sendCommentCommand = new RelayCommand<string>(
                           async (content) =>
                           {
                               IsActive = true;
                               string postJosn = string.Empty;
                               string returnJson = string.Empty;
                               if (_replyTo == null)
                               {
                                   postJosn = "{" + $"\"content\":\"{content}\"" + "}";
                                   returnJson = "{" + $"\"own\":true,\"author\":\"{ViewModelLocator.AppShell.UserInfo.Name}\",\"content\":\"{content}\",\"avatar\":\"{ViewModelLocator.AppShell.UserInfo.Avatar}\",\"time\":{Untils.ToTimestamp(DateTime.Now)},\"voted\":false,\"id\":-1,\"likes\":0" + "}";
                               }
                               else
                               {
                                   string replyJson = "{" + $"\"content\":\"{_replyTo.Content}\",\"status\":0,\"id\":{_replyTo.Id},\"author\":\"{_replyTo.Author }\"" + "}";
                                   postJosn = "{" + $"\"content\":\"{content}\",\"reply_to\":\"{_replyTo.Id}\"" + "}";
                                   returnJson = "{" + $"\"own\":true,\"author\":\"{ViewModelLocator.AppShell.UserInfo.Name}\",\"content\":\"{content}\",\"avatar\":\"{ViewModelLocator.AppShell.UserInfo.Avatar}\",\"time\":{Untils.ToTimestamp(DateTime.Now)},\"voted\":false,\"id\":-1,\"likes\":0,\"reply_to\":{replyJson}" + "}";
                               }
                               string resJosn = await WebProvider.GetInstance().SendPostRequestAsync($"http://news-at.zhihu.com/api/4/news/{_stroyId}/comment", postJosn, WebProvider.ContentType.ContentType3);
                               if (resJosn != string.Empty)
                               { 
                                    var commentId = JsonObject.Parse(resJosn)["comment"].GetObject()["id"].GetNumber();
                                    var returnObj = DataService.JsonConvertHelper.JsonDeserialize<Comment>(returnJson);
                                    returnObj.Id = (long)commentId;
                                    Messenger.Default.Send(returnObj);
                               }
                               IsActive = false;
                               Messenger.Default.Send(new NotificationMessage("goback"));
                           }));
            }
        }

    }
}
