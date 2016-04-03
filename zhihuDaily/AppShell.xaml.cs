using GalaSoft.MvvmLight.Messaging;
using System;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Phone.UI.Input;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using zhihuDaily.DataService;
using zhihuDaily.Model;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace zhihuDaily
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class AppShell : Page
    {
        public static AppShell Current = null;

        /// <summary>
        /// Initializes a new instance of the AppShell, sets the static 'Current' reference,
        /// adds callbacks for Back requests and changes in the SplitView's DisplayMode, and
        /// provide the nav menu list with the data to display.
        /// </summary>
        public AppShell()
        {
            this.InitializeComponent();

            this.Loaded += async(sender, args) =>
            {
                Current = this;

                this.TogglePaneButton.Focus(FocusState.Programmatic);

                await SplashScreenAnimation();
                HardwareButtons.BackPressed += HardwareButtons_BackPressed;

            };

            Messenger.Default.Register<NotificationMessage>(this, (msg) =>
            {
                switch (msg.Notification)
                {
                    case "NotificationPanelClosed":
                        RootSplitView.IsPaneOpen = false;
                        break;
                    default:
                        break;
                }
            });
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: Prepare page for display here.
            if (e.NavigationMode == NavigationMode.New)
            {
               await SplashScreenAnimation();
            }
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            Messenger.Default.Unregister<NotificationMessage>(this);
            HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
        }
        private bool canexit = false;

        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            //e.Handled = !canexit;
            //ToastPrompt toast = new ToastPrompt();
            //toast.Completed += (o, ex) => { canexit = false; };
            //toast.Show("再按一次返回键退出程序 8-)");
            //canexit = true;

            //Get a hold of the current frame so that we can inspect the app back stack.

            //if (this.AppFrame == null)
            //    return;

            //// Check to see if this is the top-most page on the app back stack.
            //if (this.AppFrame.CanGoBack && !canexit)
            //{
            //    // If not, set the event to handled and go back to the previous page in the app.
            //    canexit = true;
            //    this.AppFrame.GoBack();
            //}

            bool handled = e.Handled;
            this.BackRequested(ref handled);
            e.Handled = handled;
        }

        private void BackRequested(ref bool handled)
        {
            // Get a hold of the current frame so that we can inspect the app back stack.

            if (this.AppFrame == null)
                return;

            // Check to see if this is the top-most page on the app back stack.
            if (this.AppFrame.CanGoBack && !handled)
            {
                // If not, set the event to handled and go back to the previous page in the app.
                handled = true;
                this.AppFrame.GoBack();
            }
        }

        ApplicationView view;
        /// <summary>
        /// app SplashScreen
        /// </summary>
        public async Task SplashScreenAnimation()
        {
            view = ApplicationView.GetForCurrentView();
            view.TryEnterFullScreenMode();

            Point centerPoint = new Point(grid_Splash.ActualWidth / 2.0, grid_Splash.ActualHeight / 2.0);
            this.sfr.CenterX = centerPoint.X;
            this.sfr.CenterY = centerPoint.Y;
            if (sfr.ScaleX < 0.3 && sfr.ScaleY < 0.3)
            {
                return;
            }

            DoubleAnimation ScaleXAnimation = new DoubleAnimation() { From = 1, To = 1.10, Duration = TimeSpan.FromSeconds(3) };
            Storyboard.SetTarget(ScaleXAnimation, sfr);
            Storyboard.SetTargetProperty(ScaleXAnimation, "ScaleX");


            DoubleAnimation ScaleYAnimation = new DoubleAnimation() { From = 1, To = 1.15, Duration = TimeSpan.FromSeconds(3) };
            Storyboard.SetTargetProperty(ScaleYAnimation, "ScaleY");
            Storyboard.SetTarget(ScaleYAnimation, sfr);

            //DoubleAnimation CenterXAnimation = new DoubleAnimation() { From = 0, To = centerPoint.X + 50, Duration = TimeSpan.FromSeconds(5) };
            //Storyboard.SetTargetProperty(CenterXAnimation, "CenterX");
            //Storyboard.SetTarget(CenterXAnimation, sfr);


            //DoubleAnimation Opacity = new DoubleAnimation() { From = 1, To = 0.2, Duration = TimeSpan.FromSeconds(3) };
            //Storyboard.SetTargetProperty(Opacity, "Opacity");
            //Storyboard.SetTarget(Opacity, img); //mark

            Storyboard storyboard = new Storyboard();
            storyboard.Completed += Storyboard_Completed;
            storyboard.Children.Add(ScaleXAnimation);
            storyboard.Children.Add(ScaleYAnimation);
           // storyboard.Children.Add(Opacity);
            storyboard.Begin();

            await ImageCache.CreateInstance();
            if (AppSettings.Instance.IsUsingTile)
            {
                LiveTileUtils.RegisterLiveTileTask();
            }
            await AppSettings.Instance.AppInit();

            AutoLogin();
        }

        private async void Storyboard_Completed(object sender, object e)
        {
            this.grid_Splash.Visibility = Visibility.Collapsed;
            this.grid_Main.Visibility = Visibility.Visible;
            if (view.IsFullScreenMode)
                view.ExitFullScreenMode();

            await AppSettings.ShowSystemTrayAsync();
        }

        private void lvPrev_Loaded(object sender, RoutedEventArgs e)
        {
            lvPrev.SelectedIndex = 0;
        }

        public Frame AppFrame { get { return this.frame; } }
        public DependencyObject ToastPanel { get { return this.RootSplitView; } }
        /// <summary>
        /// Default keyboard focus movement for any unhandled keyboarding
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AppShell_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            FocusNavigationDirection direction = FocusNavigationDirection.None;
            switch (e.Key)
            {
                case Windows.System.VirtualKey.Left:
                case Windows.System.VirtualKey.GamepadDPadLeft:
                case Windows.System.VirtualKey.GamepadLeftThumbstickLeft:
                case Windows.System.VirtualKey.NavigationLeft:
                    direction = FocusNavigationDirection.Left;
                    break;
                case Windows.System.VirtualKey.Right:
                case Windows.System.VirtualKey.GamepadDPadRight:
                case Windows.System.VirtualKey.GamepadLeftThumbstickRight:
                case Windows.System.VirtualKey.NavigationRight:
                    direction = FocusNavigationDirection.Right;
                    break;

                case Windows.System.VirtualKey.Up:
                case Windows.System.VirtualKey.GamepadDPadUp:
                case Windows.System.VirtualKey.GamepadLeftThumbstickUp:
                case Windows.System.VirtualKey.NavigationUp:
                    direction = FocusNavigationDirection.Up;
                    break;

                case Windows.System.VirtualKey.Down:
                case Windows.System.VirtualKey.GamepadDPadDown:
                case Windows.System.VirtualKey.GamepadLeftThumbstickDown:
                case Windows.System.VirtualKey.NavigationDown:
                    direction = FocusNavigationDirection.Down;
                    break;
            }

            if (direction != FocusNavigationDirection.None)
            {
                var control = FocusManager.FindNextFocusableElement(direction) as Control;
                if (control != null)
                {
                    control.Focus(FocusState.Programmatic);
                    e.Handled = true;
                }
            }
        }

        //#region BackRequested Handlers

        //private void SystemNavigationManager_BackRequested(object sender, BackRequestedEventArgs e)
        //{
        //    bool handled = e.Handled;
        //    this.BackRequested(ref handled);
        //    e.Handled = handled;
        //}

        //private void BackButton_Click(object sender, RoutedEventArgs e)
        //{
        //    bool ignored = false;
        //    this.BackRequested(ref ignored);
        //}

        //private void BackRequested(ref bool handled)
        //{
        //    // Get a hold of the current frame so that we can inspect the app back stack.

        //    if (this.AppFrame == null)
        //        return;

        //    // Check to see if this is the top-most page on the app back stack.
        //    if (this.AppFrame.CanGoBack && !handled)
        //    {
        //        // If not, set the event to handled and go back to the previous page in the app.
        //        handled = true;
        //        this.AppFrame.GoBack();
        //    }
        //}

        //#endregion

        #region Navigation



        /// <summary>
        /// Ensures the nav menu reflects reality when navigation is triggered outside of
        /// the nav menu buttons.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnNavigatingToPage(object sender, NavigatingCancelEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Back)
            {
                //if (e.SourcePageType == typeof(ThemePage))
                //    this.AppFrame.Navigate(typeof(HomePage));
                this.lvPrev.SelectedIndex = 0;
            }
        }

        private void OnNavigatedToPage(object sender, NavigationEventArgs e)
        {
            // After a successful navigation set keyboard focus to the loaded page
            if (e.Content is Page && e.Content != null)
            {
                var control = (Page)e.Content;
                control.Loaded += Page_Loaded;
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ((Page)sender).Focus(FocusState.Programmatic);
            ((Page)sender).Loaded -= Page_Loaded;
        }

        #endregion

        public Rect TogglePaneButtonRect
        {
            get;
            private set;
        }

        /// <summary>
        /// An event to notify listeners when the hamburger button may occlude other content in the app.
        /// The custom "PageHeader" user control is using this.
        /// </summary>
        public event TypedEventHandler<AppShell, Rect> TogglePaneButtonRectChanged;

        /// <summary>
        /// Callback when the SplitView's Pane is toggled open or close.  When the Pane is not visible
        /// then the floating hamburger may be occluding other content in the app unless it is aware.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TogglePaneButton_UnChecked(object sender, RoutedEventArgs e)
        {
            TogglePaneButton.Foreground = new SolidColorBrush(Windows.UI.Colors.White);
        }
        private void TogglePaneButton_Checked(object sender, RoutedEventArgs e)
        {
            TogglePaneButton.Foreground = new SolidColorBrush(Model.AppSettings.themeLightColor);
        }

        /// <summary>
        /// Check for the conditions where the navigation pane does not occupy the space under the floating
        /// hamburger button and trigger the event.
        /// </summary>
        private void CheckTogglePaneButtonSizeChanged()
        {
            if (this.RootSplitView.DisplayMode == SplitViewDisplayMode.Inline ||
                this.RootSplitView.DisplayMode == SplitViewDisplayMode.Overlay)
            {
                var transform = this.TogglePaneButton.TransformToVisual(this);
                var rect = transform.TransformBounds(new Rect(0, 0, this.TogglePaneButton.ActualWidth, this.TogglePaneButton.ActualHeight));
                this.TogglePaneButtonRect = rect;
            }
            else
            {
                this.TogglePaneButtonRect = new Rect();
            }

            var handler = this.TogglePaneButtonRectChanged;
            if (handler != null)
            {
                // handler(this, this.TogglePaneButtonRect);
                handler.DynamicInvoke(this, this.TogglePaneButtonRect);
            }
        }

        private void btn_NewsCollectionPage_Click(object sender, RoutedEventArgs e)
        {
            this.AppFrame.Navigate(typeof(NewsCollectionPage));
            RootSplitView.IsPaneOpen = false;
        }
        OfflineNewsDownloader _offlineNewsDownloader;
        bool downloadStatus = false;
        private async void btn_OfflineDownload_Click(object sender, RoutedEventArgs e)
        {
            if (_offlineNewsDownloader == null)
            {
                _offlineNewsDownloader = new OfflineNewsDownloader();
            }

            downloadStatus = !downloadStatus;//切换状态

            if (this.downloadStatus)
            {
                progInd =  StatusBar.GetForCurrentView().ProgressIndicator;
                await progInd.ShowAsync();
                _offlineNewsDownloader.OfflineProcessHandler += _offlineNewsDownloader_OfflineProcessHandler;
                _offlineNewsDownloader.BeginDownloadAsync();
            }
            else {
                _offlineNewsDownloader.EndDownload();
            }     
        }
        StatusBarProgressIndicator progInd;
        private async void _offlineNewsDownloader_OfflineProcessHandler(double process)
        {

            btn_Offline.Text = "完成:" + (process * 100).ToString("0.0")+"%" ;
            progInd.ProgressValue = process;
            if (Math.Abs(process) == 1)
            {
                btn_Offline.Text = "离线下载";
                if (process == 1)
                {
                    ToastPrompt.ShowToast("离线下载完成");
                }
                await progInd.HideAsync();
                _offlineNewsDownloader.OfflineProcessHandler -= _offlineNewsDownloader_OfflineProcessHandler;
                _offlineNewsDownloader = null;
            }
        }

        private async void StackPanel_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (loginName.Text != "请登录")
            {
                this.AppFrame.Navigate(typeof(View.UserInfoPage));
                RootSplitView.IsPaneOpen = false;
            }
            else
            {
                await new Functions().SinaLogin();
            }
        }

        public void  AutoLogin()
        {
            string userInfoJson = AppSettings.Instance.UserInfoJson;

            if (userInfoJson != string.Empty)
            {
                UserInfo userInfo = DataService.JsonConvertHelper.JsonDeserialize<UserInfo>(userInfoJson);

                ViewModel.ViewModelLocator.AppShell.UserInfo = userInfo;

            }
        }
    }
}
