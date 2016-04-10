using GalaSoft.MvvmLight.Messaging;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
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
    public sealed partial class NewsCollectionPage : Page
    {
        public NewsCollectionPage()
        {
            this.InitializeComponent();
        }
        NewsCollectionViewMode _viewModel;
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

            if (e.NavigationMode == NavigationMode.New)
            {
                this.DataContext =_viewModel = new NewsCollectionViewMode();
                //this.LoadData();
            }
            //register message
            this.btn_LightModeSwitch.DataContext = AppSettings.Instance;
            Functions.SetTheme(this.grid_Content);
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
            Messenger.Default.Unregister<NotificationMessage>(this);
        }

        private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            Functions.btn_NightMode_Click(this.grid_Content);
        }

        private void MenuFlyoutItem_Click_1(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SettingPage));
        }

        private  void LoadData()
        {
            this.newsList.ItemsSource = CollectionDS.Instance;
        }

        private async void btn_DelFav_Click(object sender, RoutedEventArgs e)
        {
            Story selectedStory = ((MenuFlyoutItem)sender).DataContext as Story;
            if (selectedStory != null)
            {
                //await CollectionDS.Instance.RemoveFav(selectedStory);
                await WebProvider.GetInstance().SendDeleteRequestAsync($"http://news-at.zhihu.com/api/4/favorite/{selectedStory.Id}");
                if (_viewModel.CollectionStories!=null&&_viewModel.CollectionStories.Count > 0)
                {
                    _viewModel.CollectionStories.Remove(selectedStory);
                    _viewModel.UpdateCount();
                }
                ToastPrompt.ShowToast("已成功取消收藏");
            }
        }
    }
}
