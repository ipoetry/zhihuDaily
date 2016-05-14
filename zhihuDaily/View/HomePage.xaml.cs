using System;
using System.Collections.Generic;
using System.Reflection;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using zhihuDaily.DataService;
using zhihuDaily.Model;
using zhihuDaily.View;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace zhihuDaily
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class HomePage : Page
    {
        public HomePage()
        {
            this.InitializeComponent();
        }

        private void flipView_Loaded(object sender, RoutedEventArgs e)
        {
            if (flipView.Items.Count > 0)
            {
                var dTimer = new DispatcherTimer();
                dTimer.Interval = TimeSpan.FromSeconds(6.0); //mark          
                dTimer.Tick += ((s, args) =>
                {
                    if (flipView.SelectedIndex < flipView.Items.Count - 1)
                        flipView.SelectedIndex++;
                    else
                        flipView.SelectedIndex = 0;
                });

                dTimer.Start();
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: Prepare page for display here.
            // HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            // TODO: If your application contains multiple pages, ensure that you are
            // handling the hardware Back button by registering for the
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
            // If you are using the NavigationHelper provided by some templates,
            // this event is handled for you.

            this.btn_LightModeSwitch.DataContext =AppSettings.Instance; //设置开关的datacontext
            //Messenger.Default.Register<NotificationMessage>(this, (msg) =>
            //{
            //    switch (msg.Notification)
            //    {
            //        case "OnItemClick":
            //            dynamic arges = msg.Sender;
            //            if (arges != null && arges.ClickItem.Id.ToString() != "0")
            //            {
            //                Frame.Navigate(typeof(NewsContentPage), msg.Sender);
            //            }
            //            break;
            //        default:
            //            break;
            //    }
            //});
        }


        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            //Messenger.Default.Unregister<NotificationMessage>(this);
        }

        private void flipView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FlipView fv = sender as FlipView;
            fv.Focus(FocusState.Pointer);
        }

        private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            Functions.SwitchTheme();
            ViewModel.ViewModelLocator.HomePage.AppTheme = AppSettings.Instance.CurrentTheme;
            ViewModel.ViewModelLocator.AppShell.AppTheme = AppSettings.Instance.CurrentTheme;
        }

        private void flipView_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            var stories = ViewModel.ViewModelLocator.HomePage.LatestNews.TopStories;
            if (stories != null)
            {
                List<string> s = new List<string>();
                foreach (var story in stories)
                {
                    s.Add(story.Id.ToString());
                }
                Frame.Navigate(typeof(NewsContentPage), new NavigationArgs { CurrentList = s, ClickItem = flipView.SelectedItem });
            }
        }

        private void MenuFlyoutItem_Click_1(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SettingPage));
        }

        ItemsStackPanel _itemsPanel;
        ScrollViewer _scrollView;
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            List<ScrollViewer> list = new List<ScrollViewer>();
            FindChildren(list, newsList);  //先找到ScrollViewer 注册ViewChanged事件
            if (list.Count > 0)
            {
                _scrollView = list[0];
                _scrollView.ViewChanged += _scrollView_ViewChanged;
            }
            List<ItemsStackPanel> list2 = new List<ItemsStackPanel>();
            FindChildren(list2, newsList);  //找到ItemStackPanel 它包含FirstVisibleIndex属性
            if (list.Count > 0)
            {
                _itemsPanel = list2[0];
            }
        }

        private void _scrollView_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            if (_scrollView.VerticalOffset > 220)
            {
                if (_itemsPanel != null)
                {
                    DateTime newsDate = (newsList.Items[_itemsPanel.FirstVisibleIndex] as Story).Date;
                    string pageTitle = newsDate.Date.Equals(DateTime.Now.Date) ? "今日热闻" : 
                        newsDate.ToString("MM月dd日 dddd", new System.Globalization.CultureInfo("zh-CN"));
                    ViewModel.ViewModelLocator.HomePage.PageTitle = pageTitle;
                }
            }
            else
            {
                ViewModel.ViewModelLocator.HomePage.PageTitle = "主页";
            }
        }

        static void FindChildren<T>(List<T> results, DependencyObject startNode)
                where T : DependencyObject
        {
            int count = Windows.UI.Xaml.Media.VisualTreeHelper.GetChildrenCount(startNode);
            for (int i = 0; i < count; i++)
            {
                DependencyObject current = Windows.UI.Xaml.Media.VisualTreeHelper.GetChild(startNode, i);
                if ((current.GetType()).Equals(typeof(T)) || (current.GetType().GetTypeInfo().IsSubclassOf(typeof(T))))
                {
                    T asType = (T)current;
                    results.Add(asType);
                }
                FindChildren<T>(results, current);
            }
        }

        private void newsList_ItemClick(object sender, ItemClickEventArgs e)
        {
            var selectedStory = e.ClickedItem as Story;
            if (selectedStory.IsStoryItemDisplay==Visibility.Visible)
            {
                List<string> currentList = new List<string>();
                for (int i = 0; i < ViewModel.ViewModelLocator.HomePage.NewsDS.Count; i++)
                {
                    currentList.Add(ViewModel.ViewModelLocator.HomePage.NewsDS[i].Id.ToString());
                }
                Frame.Navigate(typeof(NewsContentPage), new NavigationArgs { CurrentList = currentList, ClickItem = e.ClickedItem }, new SlideNavigationTransitionInfo()); 
            }
           
        }

        private async void messageBtn_Click(object sender, RoutedEventArgs e)
        {
            if (AppSettings.Instance.UserInfoJson == string.Empty)
            {
                await new Functions().SinaLogin();
                if (AppSettings.Instance.UserInfoJson == string.Empty)
                    return;
            }
            Frame.Navigate(typeof(NotificationPage));
        }
    }
}
