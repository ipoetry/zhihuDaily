using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace zhihuDaily
{
    class AppExitToast
    {
        public event EventHandler Completed;
        public AppExitToast(string message)
        {
            //Window.Current.Bounds.Height *  DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;
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
            OpenAnimation(border);
            DispatcherTimer dTimer = new DispatcherTimer();
            dTimer.Interval = TimeSpan.FromSeconds(3);
            dTimer.Tick += (sender, args) =>
            {
                po.IsOpen = false;
                dTimer.Stop();
                Completed?.Invoke(this, new EventArgs());
            };
            dTimer.Start();
        }

        static Storyboard openAnimation;
        private static void OpenAnimation(DependencyObject so)
        {
            openAnimation = new Storyboard();
            DoubleAnimationUsingKeyFrames dafOpacity = new DoubleAnimationUsingKeyFrames();
            dafOpacity.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = KeyTime.FromTimeSpan(TimeSpan.Zero), Value = 0 });
            dafOpacity.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.6)), Value = 1 });
            Storyboard.SetTarget(dafOpacity, so);
            Storyboard.SetTargetProperty(dafOpacity, "Opacity");

            openAnimation.Children.Add(dafOpacity);
            openAnimation.Begin();
        }
    }
}
