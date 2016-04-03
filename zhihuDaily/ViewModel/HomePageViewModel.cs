using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using zhihuDaily.DataService;
using zhihuDaily.Model;

namespace zhihuDaily.ViewModel
{
    public class HomePageViewModel:ViewModelBase
    {
        private readonly ICommonService<LatestNews> _latestNewsService;
        public  static NewsBeforeDataSource CurrentNewsList;
        public HomePageViewModel(ICommonService<LatestNews> latestNewsService)
        {
            _latestNewsService = latestNewsService;
           
            this.NewsDS = new NewsBeforeDataSource(_latestNewsService);
            this.LoadMainSource();
            this.ItemClickCommand = new RelayCommand<object>((e) =>
            {
                Messenger.Default.Send(new NotificationMessage(e, "OnItemClick"));
            });

            this.RefreshCommand = new RelayCommand(async() =>
            {
                //Refresh the data
                await this.LoadMainSourceAsync();          
                ToastPrompt.ShowToast("已经刷新");
            });

        }

        private string pageTitle = "主页";

        public string PageTitle {
            get
            {
                return pageTitle;
            }
            set
            {
                pageTitle = value;
                RaisePropertyChanged(() => PageTitle);
            }
        }

        private StartImage splashInfo;
        /// <summary>
        /// 闪屏图片与信息
        /// </summary>
        public StartImage SplashInfo
        {
            get { return splashInfo; }
            set
            {
                splashInfo = value;
                RaisePropertyChanged(() => SplashInfo);
            }
        }

        private LatestNews latestNews;
        /// <summary>
        /// 最新消息
        /// </summary>
        public LatestNews LatestNews
        {
            get { return latestNews; }
            set
            {
                latestNews = value;
                RaisePropertyChanged(() => LatestNews);
            }
        }

        // if progress is complete property 
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

        private NewsBeforeDataSource newsDS;

        public NewsBeforeDataSource NewsDS
        {
            get { return newsDS; }
            set
            {
                newsDS = value;
                RaisePropertyChanged(() => NewsDS);
            }
        }

        //Event to Command
        public RelayCommand<object> ItemClickCommand { set; get; }
        public RelayCommand RefreshCommand { get; set; }

        /// <summary>
        /// Load all data
        /// </summary>
        private async void LoadMainSource()
        {
            try
            {
                this.IsActive = true;
                var latest = await _latestNewsService.GetObjectAsync2("news", "latest");

                //await when all task finish
                if (latest != null&& latest.Stories != null)
                {
                    
                    this.LatestNews = latest;
                    List<Story> list = new List<Story>();

                    //list.Add(new Story
                    //{
                    //    Date = DateTime.Now,
                    //    IsDateTitleDisplay = Visibility.Visible,
                    //    IsStoryItemDisplay = Visibility.Collapsed
                    //});// 今日热闻头部
                    list.AddRange(latest.Stories);
                    for (int i = 0; i < list.Count; i++)
                    {
                        list[i].Date = DateTime.Now;
                        list[i].IsDateTitleDisplay = Visibility.Collapsed;
                        list[i].IsStoryItemDisplay = Visibility.Visible;
                    }
                    //this.LatestNews.Stories = list;
                    foreach (var item in list)
                    {
                        NewsDS.Add(item);
                    }

                    this.IsActive = false;
                }
            }
            catch (Exception)
            {
               // throw new Exception(ex.Message);
            }

        }
        /// <summary>
        /// Refresh all data
        /// </summary>
        /// <returns></returns>
        private async Task LoadMainSourceAsync()
        {
            try
            {
                Messenger.Default.Send(new NotificationMessage("NotifyRefreshMenu"));

                var latest = await _latestNewsService.GetObjectAsync2("news", "latest");

                if (latest != null && latest.Stories != null)
                {

                    List<Story> newList = new List<Story>(latest.Stories);

                    if (this.LatestNews != null && this.LatestNews.Stories != null)
                    {
                        foreach (var story in this.LatestNews.Stories)
                        {
                            newList.RemoveAll(sy => sy.Id == story.Id);
                        }
                    }
                    else
                    {
                        this.NewsDS = new NewsBeforeDataSource(_latestNewsService);
                    }

                    this.LatestNews = latest;

                    for (int i = 0; i < newList.Count; i++)
                    {
                        newList[i].Date = DateTime.Now;
                        newList[i].IsDateTitleDisplay = Visibility.Collapsed;
                        newList[i].IsStoryItemDisplay = Visibility.Visible;
                        NewsDS.Insert(i, newList[i]);
                    }

                    this.IsActive = true;
                }
            }
            catch (Exception)
            {
                // throw new Exception(ex.Message);
            }

        }


    }
}
