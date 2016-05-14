using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace zhihuDaily.Controls
{
    public sealed partial class SharePopupWindow : UserControl
    {
        private Popup m_Popup;
        public SharePopupWindow()
        {
            this.InitializeComponent();

            m_Popup = new Popup();
            this.Width = Window.Current.Bounds.Width;
            this.Height = Window.Current.Bounds.Height;
            m_Popup.Child = this;
            this.Loaded += MessagePopupWindow_Loaded;
            this.Unloaded += MessagePopupWindow_Unloaded;
        }

        private void MessagePopupWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Window.Current.SizeChanged += MessagePopupWindow_SizeChanged; ;
        }

        private void MessagePopupWindow_SizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        {
            this.Width = e.Size.Width;
            this.Height = e.Size.Height;
        }

        private void MessagePopupWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            Window.Current.SizeChanged -= MessagePopupWindow_SizeChanged; ;
        }


        public void ShowWindow()
        {
            m_Popup.IsOpen = true;
        }

        private void DismissWindow()
        {
            m_Popup.IsOpen = false;
        }

        private void LeftButton_Click(object sender, RoutedEventArgs e)
        {
            DismissWindow();
            //SinaShareClick?.Invoke(this, e);
        }

        private void RightButton_Click(object sender, RoutedEventArgs e)
        {
            DismissWindow();
            //WeChatShareClick?.Invoke(this, e);
        }

        public event EventHandler<RoutedEventArgs> SinaShareClick;
        public event EventHandler<RoutedEventArgs> WeChatShareClick;
        public event EventHandler<RoutedEventArgs> MoreShareClick;

        private void btnSina_Click(object sender, RoutedEventArgs e)
        {
            DismissWindow();
            SinaShareClick?.Invoke(this, e);
        }

        private void btnWeChat_Click(object sender, RoutedEventArgs e)
        {
            DismissWindow();
            WeChatShareClick?.Invoke(this, e);
        }

        private void btnMoreShare_Click(object sender, RoutedEventArgs e)
        {
            DismissWindow();
            MoreShareClick?.Invoke(this, e);
        }

        private void OutBorder_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            DismissWindow();
        }
    }
}
