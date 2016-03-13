using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
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

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

            //  if (e.NavigationMode == NavigationMode.New)
            {
                this.DataContext = new NewsCollectionViewMode();
                this.LoadData();
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
            //if (e.SourcePageType != typeof(NewsReadingPage))
            //{

            //}
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
                await CollectionDS.Instance.RemoveFav(selectedStory);
                ToastPrompt.ShowToast("删除成功");
            }
        }
    }
}
