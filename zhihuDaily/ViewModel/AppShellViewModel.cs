using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using zhihuDaily.DataService;
using zhihuDaily.Model;

namespace zhihuDaily.ViewModel
{

    public class AppShellViewModel: ViewModelBase
    {
        private readonly ICommonService<Themes> _themesService;
        private readonly ICommonService<StartImage> _startImageService;
        public AppShellViewModel(ICommonService<StartImage> startImageService,ICommonService<Themes> themesService)
        {

            _startImageService = startImageService;
            _themesService = themesService;

            Themes_Style = new ObservableCollection<Theme_Style>();

            LocalInfo = JsonConvertHelper.JsonDeserialize<StartImage>(AppSettings.Instance.SplashInfo);
            this.SplashInfo = LocalInfo;

            this.LoadMainSource();

            this.ThemeItemClickCommand = new RelayCommand<Theme_Style>((e) =>
            {
                AppShell rootFrame = Window.Current.Content as AppShell;
                if (rootFrame != null)
                {
                    if (e.Id > 0)
                    {
                        rootFrame.AppFrame.Navigate(typeof(ThemePage), e.Id);
                    }
                    else
                    {
                        rootFrame.AppFrame.Navigate(typeof(HomePage));
                    }
                    Messenger.Default.Send<NotificationMessage>(new NotificationMessage("NotificationPanelClosed"));
                }
            });

            Messenger.Default.Register<NotificationMessage>(this, (msg) =>
            {
                switch (msg.Notification)
                {
                    case "NotifyRefreshMenu":
                        this.LoadMainSource();
                        break;
                    default:
                        break;
                }
            });

            System.Diagnostics.Debug.WriteLine("执行次数多了很蛋疼啊，，我说的是真的都是真的");
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
        public StartImage LocalInfo { get; set; }

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


        private ObservableCollection<Theme_Style> themes_Style;

        public ObservableCollection<Theme_Style> Themes_Style
        {
            get { return themes_Style; }
            set
            {
                themes_Style = value;
              //  RaisePropertyChanged(() => Themes_Style);
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


        //Event to Command
        public RelayCommand<object> ItemClickCommand { set; get; }
        public RelayCommand<Theme_Style> ThemeItemClickCommand { get; set; }

        /// <summary>
        /// Load Splash
        /// </summary>
        public async Task LoadSplashImage()
        {
            this.SplashInfo = await _startImageService.GetNotAvailableCacheObjAsync("start-image", "1080*1776");
            if (string.IsNullOrEmpty(this.SplashInfo.Img))
            {
                this.SplashInfo.Text = LocalInfo.Text;
                return;
            }

            if (this.SplashInfo.Img != LocalInfo.Img)
            {
                AppSettings.Instance.SplashInfo = JsonConvertHelper.JsonSerializer(this.SplashInfo);
                DownloadHelper.SaveImage(SplashInfo.Img);
            }
        }

        /// <summary>
        /// Load Theme data
        /// </summary>
        private async void LoadMainSource()
        { 
            try
            {
                Themes_Style.Clear();
                Themes_Style.Add(new Theme_Style { Id = -1, PicUri = new Uri("ms-appx:///Assets/FunIcon/menu_home.png"), Title = "主页", FontColor = AppSettings.themeLightString, TbMargin = "15,0,0,0" });
                var themes = await _themesService.GetObjectAsync("themes");
                if (themes != null)
                {
                    this.Themes = themes;
                    if (this.Themes.Others != null)
                        foreach (Others other in this.Themes.Others)
                        {                         
                            Themes_Style.Add(new Theme_Style { Id = other.Id, Title = other.Name });
                        }
                    this.IsCompleted = true;
                }
                await this.LoadSplashImage();
            }
            catch (Exception)
            {
                
            }
        }
    }
}
