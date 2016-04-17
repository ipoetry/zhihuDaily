using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using zhihuDaily.DataService;
using zhihuDaily.Model;

namespace zhihuDaily.ViewModel
{
    class NewsContentViewModel:ViewModelBase
    {
        public delegate void MessageNotice(NewsContent newsContent);
        public event MessageNotice MessageNoticeHanlder;
        public NewsContentViewModel(string id,List<string> list)
        {
            AppTheme = AppSettings.Instance.CurrentTheme;
            this.IdList = list;
            this.CurrentIndex = list.IndexOf(id);
            this.LoadNewsContent(CurrentIndex);
        }

        private List<string> idList;
        public List<string> IdList
        {
            get { return idList; }
            set
            {
                idList = value;
                RaisePropertyChanged(() => IdList);
            }
        }

        public int currentIndex;
        public int CurrentIndex {
            get { return currentIndex; }
            set {
                currentIndex = value;
                RaisePropertyChanged(()=>CurrentIndex);

            }
        }

        private NewsContent newsContent;

        public NewsContent NewsContent
        {
            get { return newsContent; }
            set
            {
                newsContent = value;
                RaisePropertyChanged(() => NewsContent);
            }
        }

        private StoryExtra storyExtra;

        public StoryExtra StoryExtra
        {
            get { return storyExtra; }
            set
            {
                storyExtra = value;
                RaisePropertyChanged(() => StoryExtra);
            }
        }

        private bool isActive = true;

        public bool IsActive
        {
            get { return isActive; }
            set
            {
                isActive = value;
                RaisePropertyChanged(() => IsActive);
            }
        }

        private ElementTheme _appTheme;
        public ElementTheme AppTheme
        {
            get
            {
                return _appTheme;
            }

            set
            {
                _appTheme = value;
                RaisePropertyChanged(() => AppTheme);
            }
        }

        public async void LoadNewsContent(int index)
        {
            this.IsActive = true;
            string id = IdList[index];
            try
            {
                ICommonService<NewsContent> newsContentService = new CommonService<NewsContent>();
                NewsContent content = await newsContentService.GetObjectAsync("story", id);

                if (content.Body != null)
                {                
                    this.NewsContent = content;
                    NewsContent newsContent = new NewsContent { Body = content.Body, Css = content.Css, Image = content.Image, Title = content.Title, ImageSource = content.ImageSource, ShareUrl = content.ShareUrl };
                    await Task.Delay(100);
                    MessageNoticeHanlder?.Invoke(newsContent);
                    //  Messenger.Default.Send<NotificationMessage>(new NotificationMessage(newsContent, "OnLoadCompleted"));
                    //Delay to destroy animation

                    ICommonService<StoryExtra> storyExtraService = new CommonService<StoryExtra>();
                    this.StoryExtra  = await storyExtraService.GetNotAvailableCacheObjAsync("story-extra", id);
                
                    this.IsActive = false;
                    
                }
                else
                {
                    ToastPrompt.ShowToast("未获取到数据");
                }
            }
            catch (Exception)
            {
                // throw new Exception(ex.Message);
                ToastPrompt.ShowToast("内容加载失败");
            }

        }
    }
}
