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
            AppTheme = AppSettings.Instance.CurrentTheme;
            Themes_Style = new ObservableCollection<Theme_Style>();

            this.SplashInfo = JsonConvertHelper.JsonDeserialize<StartImage>(AppSettings.Instance.SplashInfo);

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
               // RaisePropertyChanged(() => SplashInfo);
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

        private UserInfo userInfo=new UserInfo();
        public UserInfo UserInfo
        {
            get { return userInfo; }
            set
            {
                userInfo = value;
                RaisePropertyChanged(() => UserInfo);
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

        //Event to Command
        public RelayCommand<object> ItemClickCommand { set; get; }
        public RelayCommand<Theme_Style> ThemeItemClickCommand { get; set; }

        /// <summary>
        /// Load Splash
        /// </summary>
        public async Task UpdateSplashInfo()
        {
            if (NetWorkHelper.NetWrokState != 0)
            {
                var newSplash = await _startImageService.GetNotAvailableCacheObjAsync("start-image", "1080*1776");
                if (System.IO.Path.GetFileNameWithoutExtension(newSplash.Img) != System.IO.Path.GetFileNameWithoutExtension(this.SplashInfo.Img))
                {
                    DownloadHelper.SaveImage(newSplash.Img);
                    newSplash.Img = "ms-appdata:///local/" + System.IO.Path.GetFileName(newSplash.Img);
                    AppSettings.Instance.SplashInfo = JsonConvertHelper.JsonSerializer(newSplash);
                }
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
                    this.IsActive = true;
                }
                await this.UpdateSplashInfo();
            }
            catch (Exception)
            {
                
            }
        }
    }
}
