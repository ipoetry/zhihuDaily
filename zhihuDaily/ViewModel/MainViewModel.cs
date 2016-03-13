using System;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;
using zhihuDaily.Model;
using zhihuDaily.DataService;
using Windows.UI.Popups;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;

namespace zhihuDaily.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private readonly ICommonService<StartImage> _startImageService;
        private readonly ICommonService<Themes> _themesService;
        private readonly ICommonService<LatestNews> _latestNewsService;

        public MainViewModel(ICommonService<StartImage> startImageService, ICommonService<LatestNews> latestNewsService,
             ICommonService<Themes> themesService)
        {

            _startImageService = startImageService;
            _themesService = themesService;
            _latestNewsService = latestNewsService;

            this.LoadMainSource();
            this.NewsDS = new NewsBeforeDataSource(_latestNewsService);

            this.ItemClickCommand = new RelayCommand<object>((e) =>
            {
                Messenger.Default.Send(new NotificationMessage(e, "OnItemClick"));
            });

            this.ThemeItemClickCommand = new RelayCommand<Theme_Style>((e) =>
            {
                System.Diagnostics.Debug.WriteLine(e.Id);
                if (e.Id > 0)
                {
                    ((Frame)Window.Current.Content).Navigate(typeof(ThemePage),e.Id);
                }
                else
                {
                    ((Frame)Window.Current.Content).Navigate(typeof(MainPage));
                }
            });

            this.RefreshCommand = new RelayCommand(() =>
            {
                //Refresh the data
                this.LoadMainSource();
                this.NewsDS = new NewsBeforeDataSource(_latestNewsService);
            });
            this.SettingCommand = new RelayCommand(() =>
            {
                Messenger.Default.Send(new NotificationMessage("OnSettingButtonClick"));
            });
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

        private Themes themes;
        /// <summary>
        /// 主题列表
        /// </summary>
        public Themes Themes
        {
            get { return themes; }
            set
            {
                themes = value;
                RaisePropertyChanged(() => Themes);
            }
        }


        private List<Theme_Style> themes_Style;

        public List<Theme_Style> Themes_Style
        {
            get { return themes_Style; }
            set
            {
                themes_Style = value;
                RaisePropertyChanged(() => Themes_Style);
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
        private bool isCompleted = false;

        public bool IsCompleted
        {
            get { return isCompleted; }
            set
            {
                isCompleted = value;
                RaisePropertyChanged(() => IsCompleted);
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
        public RelayCommand<Theme_Style> ThemeItemClickCommand { get; set; }
        public RelayCommand RefreshCommand { get; set; }
        public RelayCommand SettingCommand { get; set; }

        /// <summary>
        /// Load all data
        /// </summary>
        private async void LoadMainSource()
        {
            Themes_Style = new List<Theme_Style>();
            Themes_Style.Add(new Theme_Style { Id=-1, PicUri = new Uri("ms-appx:///Assets/FunIcon/menu_home.png"), Title = "主页", FontColor = "#FF297ACD", TbMargin = "15,0,0,0" });
            //Themes_Style.Add(new Theme_Style { Id = 12, Title = "导航到某夜" });
            try
            {
                this.SplashInfo = await _startImageService.GetObjectAsync("start-image", "1080*1776");
                DownloadHelper.SaveImage(SplashInfo.Img);
                var themes = _themesService.GetObjectAsync("themes");
                var latest = _latestNewsService.GetObjectAsync("news", "latest");

                //await when all task finish
                await Task.WhenAll(themes, latest);
                if (themes != null && latest != null)
                {
                    this.Themes = themes.Result;
                    this.LatestNews = latest.Result;
                    IList<Story> list = new List<Story>(this.LatestNews.Stories);

                    list.Add(new Story { Date = DateTime.Now,
                    IsDateTitleDisplay = Visibility.Visible,
                    IsStoryItemDisplay = Visibility.Collapsed
                });
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
                    if (this.Themes.Others!=null)
                    foreach (Others other in this.Themes.Others)
                    {
                        Themes_Style.Add(new Theme_Style { Id = other.Id, Title = other.Name });
                    }

                    this.IsCompleted = true;
                }
                else
                {
                    MessageDialog msg = new MessageDialog(_startImageService.ExceptionsParameter, "提示");
                    await msg.ShowAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
    }
}