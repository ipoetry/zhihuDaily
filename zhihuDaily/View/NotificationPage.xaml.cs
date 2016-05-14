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
using zhihuDaily.Model;
using zhihuDaily.ViewModel;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace zhihuDaily.View
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class NotificationPage : Page
    {
        NotificationPageViewModel _viewModel;
        public NotificationPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            this.DataContext = _viewModel = new NotificationPageViewModel();
        }

        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            Notification notification = e.ClickedItem as Notification;
            if (notification.Type == 1)
            {
                this.Frame.Navigate(typeof(NotificationDetailPage), new object[] { e.ClickedItem });
            }
            else if (notification.Type == 2)
            {
                this.Frame.Navigate(typeof(NotificationReplyPage), new object[] { e.ClickedItem });
            }
        }
    }
}
