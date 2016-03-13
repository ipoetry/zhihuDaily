using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace zhihuDaily
{
    public partial class ToastBox : UserControl
    {
        public Storyboard OpenAnimation;
        public Storyboard CloseAnimation;
        public ToastBox()
        {
            this.InitializeComponent();
            InitOpenAnimation();
            InitClosenAnimation();
            this.Height = Windows.UI.ViewManagement.StatusBar.GetForCurrentView().OccludedRect.Height;
        }

        public void InitOpenAnimation()
        {
            OpenAnimation = new Storyboard();
            DoubleAnimationUsingKeyFrames dafGlobalOffsetX = new DoubleAnimationUsingKeyFrames();
            dafGlobalOffsetX.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = KeyTime.FromTimeSpan(TimeSpan.Zero), Value = -480 });
            dafGlobalOffsetX.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.3)), Value = 0 });
            Storyboard.SetTarget(dafGlobalOffsetX, planeProjection);
            Storyboard.SetTargetProperty(dafGlobalOffsetX, "GlobalOffsetX");

            DoubleAnimationUsingKeyFrames dafOpacity = new DoubleAnimationUsingKeyFrames();
            dafOpacity.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = KeyTime.FromTimeSpan(TimeSpan.Zero), Value = 0 });
            dafOpacity.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.3)), Value = 1 });
            Storyboard.SetTarget(dafOpacity, userControl);
            Storyboard.SetTargetProperty(dafOpacity, "Opacity");

            OpenAnimation.Children.Add(dafOpacity);
            OpenAnimation.Children.Add(dafGlobalOffsetX);
        }
        public void InitClosenAnimation()
        {
            CloseAnimation = new Storyboard();
            DoubleAnimationUsingKeyFrames dafGlobalOffsetX = new DoubleAnimationUsingKeyFrames();
            dafGlobalOffsetX.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = KeyTime.FromTimeSpan(TimeSpan.Zero), Value = 0 });
            dafGlobalOffsetX.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.3)), Value = 480 });
            Storyboard.SetTarget(dafGlobalOffsetX, planeProjection);
            Storyboard.SetTargetProperty(dafGlobalOffsetX, "GlobalOffsetX");

            DoubleAnimationUsingKeyFrames dafOpacity = new DoubleAnimationUsingKeyFrames();
            dafOpacity.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = KeyTime.FromTimeSpan(TimeSpan.Zero), Value = 1 });
            dafOpacity.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.3)), Value = 0});
            Storyboard.SetTarget(dafOpacity, userControl);
            Storyboard.SetTargetProperty(dafOpacity, "Opacity");

            CloseAnimation.Children.Add(dafOpacity);
            CloseAnimation.Children.Add(dafGlobalOffsetX);
        }

        public static readonly DependencyProperty MessageProperty

            = DependencyProperty.Register("Message", typeof(string), typeof(ToastBox), new PropertyMetadata(new PropertyChangedCallback(OnMessageChanged)));

        private static void OnMessageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)

        {

            if (d != null && d is ToastBox)

            {

                (d as ToastBox).SetMessage((string)e.NewValue);

            }

        }

        private void SetMessage(string toastBox)

        {

            message.Text = toastBox;

        }

        public string Message

        {

            get

            {

                return (string)GetValue(MessageProperty);

            }

            set

            {

                SetValue(MessageProperty, value);

            }

        }

    }
}
