using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Phone.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Navigation;
using zhihuDaily.DataService;
using zhihuDaily.Model;

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
        DispatcherTimer dTimer;
        private void flipView_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            FlipView flipView = sender as FlipView;
            if (flipView.SelectedIndex == -1) { return; }
            if (dTimer == null)
            {
                dTimer = new DispatcherTimer();
            }
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
            Functions.SetTheme(this.grid_Content);
            Messenger.Default.Register<NotificationMessage>(this, (msg) =>
            {
                switch (msg.Notification)
                {
                    case "OnItemClick":
                        dynamic arges = msg.Sender;
                        if (arges != null && arges.ClickItem.Id.ToString() != "0")
                        {
                            Frame.Navigate(typeof(NewsContentPage), msg.Sender);
                        }
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

        private void flipView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FlipView fv = sender as FlipView;
            fv.Focus(FocusState.Pointer);
        }

        private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            Functions.btn_NightMode_Click(this.grid_Content);
        }

        private void flipView_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            var control = sender as FlipView;
            dynamic dobj = control.ItemsSource;
            if (dobj != null)
            {
                List<string> s = new List<string>();
                foreach (var i in dobj)
                {
                    s.Add(i.Id.ToString());
                }
                Frame.Navigate(typeof(NewsContentPage), new Model.NavigationArgs { CurrentList = s, ClickItem = flipView.SelectedItem });

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
    }
}
