using GalaSoft.MvvmLight.Messaging;
using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using zhihuDaily.DataService;
using zhihuDaily.Model;
using zhihuDaily.ViewModel;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace zhihuDaily
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class ThemePage : Page
    {
        public ThemePage()
        {
            this.InitializeComponent();
        }
        ThemePageViewModel tvw;
        string themeId = string.Empty;
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            
          //  if (e.NavigationMode == NavigationMode.New)
            {
                string id = e.Parameter.ToString();
                themeId = id;
                tvw = new ThemePageViewModel(id);
                this.DataContext = tvw;
            }
            //register message
            this.btn_LightModeSwitch.DataContext = AppSettings.Instance;
            Functions.SetTheme(this.grid_Theme);
            Messenger.Default.Register<NotificationMessage>(this, (msg) =>
            {
                switch (msg.Notification)
                {
                    case "OnItemClick":
                        Frame.Navigate(typeof(NewsContentPage), msg.Sender);
                        break;
                    default:
                        break;
                }
            });
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            //if (e.SourcePageType != typeof(NewsReadingPage))
            //{
                
            //}
            Messenger.Default.Unregister<NotificationMessage>(this);
        }

        public void HeaderImageAnimation()
        {
            Point centerPoint = new Point(HeadeImagerPanel.ActualWidth / 2.0, HeadeImagerPanel.ActualHeight / 2.0);
            this.sfr1.CenterX = centerPoint.X;
            this.sfr1.CenterY = centerPoint.Y;
            if (sfr1.ScaleX < 0.3 && sfr1.ScaleY < 0.3)
            {
                return;
            }
            DoubleAnimation ScaleXAnimation = new DoubleAnimation() { From = 1, To = 1.20, Duration = TimeSpan.FromSeconds(8) };
            Storyboard.SetTarget(ScaleXAnimation, sfr1);
            Storyboard.SetTargetProperty(ScaleXAnimation, "ScaleX");

            DoubleAnimation ScaleYAnimation = new DoubleAnimation() { From = 1, To = 1.18, Duration = TimeSpan.FromSeconds(8) };
            Storyboard.SetTargetProperty(ScaleYAnimation, "ScaleY");
            Storyboard.SetTarget(ScaleYAnimation, sfr1);

            DoubleAnimation CenterXAnimation = new DoubleAnimation() { From = centerPoint.X, To = centerPoint.X - 50, Duration = TimeSpan.FromSeconds(5) };
            Storyboard.SetTargetProperty(CenterXAnimation, "CenterX");
            Storyboard.SetTarget(CenterXAnimation, sfr1);

            Storyboard storyboard = new Storyboard();
            storyboard.AutoReverse = true;
            storyboard.RepeatBehavior = RepeatBehavior.Forever;
            storyboard.Children.Add(ScaleXAnimation);
            storyboard.Children.Add(ScaleYAnimation);
            storyboard.Begin();
        }


        private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            Functions.btn_NightMode_Click(this.grid_Theme);
        }

        private void MenuFlyoutItem_Click_1(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SettingPage));
        }

        private void PullToRefreshBox_RefreshInvoked(DependencyObject sender, object args)
        {
            tvw.LoadTheme(themeId);
        }

        private void HeaderImage_ImageOpened(object sender, RoutedEventArgs e)
        {
            HeaderImageAnimation();
        }
    }
}
