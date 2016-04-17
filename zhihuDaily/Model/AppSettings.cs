using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using zhihuDaily.DataService;

namespace zhihuDaily.Model
{
    public sealed class AppSettings: NotificationObject
    {


        #region static
        public static string themeLightString = "#FF297ACD";

        public static string themeStatusBarString = "#FF327EC0";

        public static string themeLightString2 = "#FF008BED";//

        public static Color themeLightColor = themeLightString2.ToColor();

        #endregion

        public RelayCommand<object> ToggledCommand { get; set; }

        private  string lightMode;
        public string LightMode {
            get {
                return lightMode;
            }
            set {
                lightMode = value;
                RaisePropertyChanged(()=>LightMode);
            }
        }


        private ApplicationDataContainer settings = ApplicationData.Current.LocalSettings;
        private ApplicationDataContainer roaming = ApplicationData.Current.RoamingSettings;

        
        #region localSettings
        const string SettingKey_NightModeTheme = "app_night_mode_theme";
        const string SettingKey_PicMode = "app_pictual_mode";
        const string SettingKey_SplashInfo = "app_splash_screen";
        const string SettingKey_ThemeMode = "app_theme_mode";
        const string SettingKey_UsingTile = "app_tile_mode";
        const string SettingKey_SinaLogin = "app_login_info";
        const string SettingKey_UserInfo = "app_user_info";
        #endregion

        #region RoamingSetting

        const string rSettingKey_CollectionNews = "app_collection_news";
        #endregion

        public string LoginInfoJson
        {
            get {
                var obj = this.settings.Values[SettingKey_SinaLogin];
                return obj == null ? string.Empty: obj.ToString();
            }
            set
            {
                this.settings.Values[SettingKey_SinaLogin] = value;
                RaisePropertyChanged(() => LoginInfoJson);
            }
        }

        public string UserInfoJson
        {
            get
            {
                var obj = this.settings.Values[SettingKey_UserInfo];
                return obj == null ? string.Empty : obj.ToString();
            }
            set
            {
                this.settings.Values[SettingKey_UserInfo] = value;
                RaisePropertyChanged(() => UserInfoJson);
            }
        }


        public string SplashInfo
        {
            get
            {
                var obj = this.settings.Values[SettingKey_SplashInfo];
                return obj == null ? "{\"img\":\"ms-appx:///Assets/Images/splash.png\",\"text\":\"Image : 1tu / pitrs\"}" : obj.ToString();
            }
            set
            {
                this.settings.Values[SettingKey_SplashInfo] = value;
                //RaisePropertyChanged(() => SplashInfo);
            }
        }



        public ObservableCollection<string> CollectionNews
        {
            get {
                var obj = roaming.Values["app_collection_news"];
                return obj==null?new ObservableCollection<string>():JsonConvertHelper.JsonDeserialize<ObservableCollection<string>>(obj.ToString());
            }
            set {
                roaming.Values["app_collection_news"] = JsonConvertHelper.JsonSerializer(value);
               // RaisePropertyChanged(()=>this.CollectionNews);
            }
        }

        /// <summary>
        /// default is DayMode, this value = false
        /// </summary>
        public bool NightModeTheme
        {
            get
            {
                var obj = this.settings.Values[SettingKey_NightModeTheme];
                return obj == null ? false : (bool)obj;
            }
            set
            {
                this.settings.Values[SettingKey_NightModeTheme] = value;
                RaisePropertyChanged(()=> NightModeTheme);
            }
        }


        public bool IsNightMode
        {
            get
            {
                return CurrentTheme != ElementTheme.Light;
            }
        }
        public string _cacheSze="计算中...";
        public string CacheSize {
            get { return _cacheSze; }
            set {
                _cacheSze = value;
                RaisePropertyChanged(()=>CacheSize);
            }
        }

        public async Task<double> GetCacheSize()
        {
            long cacheSize = -1L;
            var lf = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Cache",CreationCollisionOption.OpenIfExists);
            IReadOnlyList<StorageFile> cacheFiles = await lf.GetFilesAsync();
            if (cacheFiles == null || cacheFiles.Count == 0)
            {
                CacheSize = "0KB";
                return 0L;
            }
            foreach (var sf in cacheFiles)
            {
                using (var stream = await sf.OpenStreamForReadAsync())
                {
                    cacheSize += stream.Length;
                }
            }
            if (cacheSize / 1024 > 1024)
            {
                CacheSize = cacheSize / (2 << 19) + "MB";
            }
            else
            {
                CacheSize = cacheSize / 1024 + "KB";
            }
            return cacheSize;
        }

        public async Task ClearCache()
        {
            try
            {
                if (await StorageHelper.Instance.DeleteDirectoryAsync("Cache", true))
                {
                    // var lf = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Cache", CreationCollisionOption.ReplaceExisting);
                    await this.GetCacheSize();
                }
            }
            catch(Exception)
            {
                ToastPrompt.ShowToast("缓存清除失败");
            }
        }

        /// <summary>
        /// 当前主题
        /// </summary>
        public ElementTheme CurrentTheme
        {
            get
            {
                var obj = this.settings.Values[SettingKey_ThemeMode];
                return obj == null ? ElementTheme.Light : (ElementTheme)Enum.Parse(typeof(ElementTheme),obj.ToString());
            }
            set
            {
                this.settings.Values[SettingKey_ThemeMode] = value.ToString();
                RaisePropertyChanged(() => CurrentTheme);
            }
        }


        public bool IsNonePicMode
        {
            get
            {
                var obj = this.settings.Values[SettingKey_PicMode];
                return obj == null ? false : bool.Parse(obj.ToString());
            }
            set
            {
                this.settings.Values[SettingKey_PicMode] = value.ToString();
                RaisePropertyChanged(() => IsNonePicMode);
            }

        }

        public bool IsUsingTile
        {
            get
            {
                var obj = this.settings.Values[SettingKey_UsingTile];
                return obj == null ? false : bool.Parse(obj.ToString());
            }
            set
            {
                this.settings.Values[SettingKey_UsingTile] = value.ToString();
                RaisePropertyChanged(() => IsUsingTile);
            }
        }


        /// <summary>
        /// App当前版本号
        /// </summary>
        public string CurrentVersion {
            get {
                return DataService.Functions.GetVersionString();
            }
        }

        private static volatile AppSettings _instance;
        private static object _locker = new object();

        private AppSettings() {
            this.ToggledCommand = new RelayCommand<object>((e) =>
            {
                if (e.ToString() == "true")
                {
                    LiveTileUtils.RegisterLiveTileTask();
                }
                else {
                    var updater = TileUpdateManager.CreateTileUpdaterForApplication();
                    updater.Clear();
                    LiveTileUtils.UnRegisterLiveTileTask();
                }
            });
        }

        public static AppSettings Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_locker)
                    {
                        if (_instance == null)
                        {
                            _instance = new AppSettings();
                        }
                    }
                }
                return _instance;
            }
        }
        /// <summary>
        /// 完成系统的一些初始化操作
        /// </summary>
        /// <returns></returns>
        public async Task AppInit()
        {
            await ApplicationData.Current.LocalFolder.CreateFolderAsync("Cache", CreationCollisionOption.OpenIfExists);
        }

    }
}
