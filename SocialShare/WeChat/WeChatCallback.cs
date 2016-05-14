using MicroMsg.sdk;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace SocialShare.WeChat
{
    public class WeChatCallback: WXEntryBasePage
    {
        public override void OnSendMessageToWXResponse(SendMessageToWX.Resp response)
        {
            base.OnSendMessageToWXResponse(response);
            string tips = response.ErrCode == 0 ? "分享成功" : "分享失败";
            this.Show_CenterToast(tips);
        }

        public override void OnSendAuthResponse(SendAuth.Resp response)
        {
            base.OnSendAuthResponse(response);
        }

        private void Show_CenterToast(string message)
        {
            double bHeight = Window.Current.Bounds.Height;
            double bWidth = Window.Current.Bounds.Width;
            Popup po = new Popup { VerticalOffset = (bHeight / 3) * 2 };
            po.Transitions = new TransitionCollection();
            po.Transitions.Add(new PopupThemeTransition { FromHorizontalOffset = 0, FromVerticalOffset = 100 });

            var imgBrush = new ImageBrush();
            imgBrush.ImageSource = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri("ms-appx:///Assets/FunIcon/bg_tips.png"));

            Border border = new Border { Background = imgBrush, Width = message.Length * 22, Height = 50, HorizontalAlignment = HorizontalAlignment.Center, CornerRadius = new CornerRadius(2) };
            border.Child = new TextBlock { Text = message, Foreground = new SolidColorBrush(Windows.UI.Colors.White), FontSize = 14, Margin = new Thickness(5, 0, 5, 0), VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center };
            double s2 = bWidth / 2 - border.Width / 2;
            border.Margin = new Thickness(s2, 0, 0, 0);
            po.Child = border;
            po.IsOpen = true;
            DispatcherTimer dTimer = new DispatcherTimer();
            dTimer.Interval = TimeSpan.FromSeconds(2);
            dTimer.Tick += (sender, args) =>
            {
                po.IsOpen = false;
                dTimer.Stop();
            };
            dTimer.Start();
        }
    }
}
