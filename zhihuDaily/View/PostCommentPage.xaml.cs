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
using zhihuDaily.ViewModel;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace zhihuDaily.View
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class PostCommentPage : Page
    {
        public PostCommentPage()
        {
            this.InitializeComponent();
        }

        PostCommentViewModel _viewModel;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            object[] parameters = e.Parameter as object[];
            if (parameters != null && parameters[0] != null && parameters[1] != null)
            {
                this.DataContext = _viewModel = new PostCommentViewModel(parameters[0].ToString(), parameters[1].ToString());
            }

            Messenger.Default.Register<NotificationMessage>(this, (msg) =>
            {
                switch (msg.Notification)
                {
                    case "goback":
                        this.Frame.GoBack();
                        break;
                    default:
                        break;
                }
            });
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            Messenger.Default.Unregister<NotificationMessage>(this);
        }
    }
}
