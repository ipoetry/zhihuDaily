using System;
using Windows.ApplicationModel.Store;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using zhihuDaily.DataService;
using zhihuDaily.Model;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace zhihuDaily
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class SettingPage : Page
    {
        AppSettings settings = AppSettings.Instance;
        public SettingPage()
        {
            this.InitializeComponent();
            this.DataContext = settings;
        }

        private async void btn_RateMe_Click(object sender, RoutedEventArgs e)
        {
            // ms-windows-store://pdp/?ProductId=9nblggh5lj9x
            ///ms-windows-store://review/?ProductId=9nblggh5lj9x
            var uri = new Uri(string.Format("ms-windows-store://review/?ProductId={0}", "9nblggh5lj9x"));
            await Windows.System.Launcher.LaunchUriAsync(uri);
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {        
            //Functions.SetTheme(this.grid_Content);
            await settings.GetCacheSize();
        }

        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PivotItem pi = pivot_Main.SelectedItem as PivotItem;
            if (pi.Tag.ToString() == "about")
            {
                AnimationToolkit.Animator.Use(AnimationToolkit.AnimationType.RollIn).PlayOn(this.image_Logo);
            }
        }

        private void btn_FeedBack_Click(object sender, RoutedEventArgs e)
        {
           Functions.SendFeedBackByEmail();
        }

        private async void btn_ClearCache_Click(object sender, RoutedEventArgs e)
        {
            await settings.ClearCache();
        }
    }
}
