using System;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace zhihuDaily.Controls
{
    public class CustomToast_QQ
    {
        private static readonly Color defaultForegroundColor = Colors.White;
        private static readonly Color defaultBackgroundColor = Model.AppSettings.themeLightColor;

        public static void ShowAsync(string text,Action complete = null)
        {
            double height = StatusBar.GetForCurrentView().OccludedRect.Height;
            Show(text, defaultForegroundColor, defaultBackgroundColor, complete, height);
        }

        public static void Show(string text, Color foregroundColor, Color backgroundColor, Action complete = null, double height = 40)
        {
            Show(text, new SolidColorBrush(foregroundColor), new SolidColorBrush(backgroundColor), complete, height);
        }

        public static void Show(string text, Brush foregroundBrush, Brush backgroundBrush, Action complete = null, double height = 40)
        {
            var p = new Popup
            {
                Child = new Border
                {
                    Width = Window.Current.Bounds.Width,
                    Background = backgroundBrush,
                    Child = new TextBlock
                    {
                        Text = text,
                        Foreground = foregroundBrush,
                        FontSize = 14,
                        VerticalAlignment = VerticalAlignment.Center,
                        Margin = new Thickness(Window.Current.Bounds.Width/4d, 0, 0, 0)
                    }
                }
            };

            Show(p, complete, height);
        }

        private static void Show(Popup popup, Action complete = null, double height = 40)
        {
            if (!(popup.Child.RenderTransform is CompositeTransform))
            {
                popup.Child.RenderTransform = new CompositeTransform();
            }
            ((CompositeTransform)popup.Child.RenderTransform).TranslateY = -height;

            System.Diagnostics.Debug.Assert(popup.Child is FrameworkElement);

            var element = popup.Child as FrameworkElement;
            element.Height = height;

            var storyboard = new Storyboard
            {
                AutoReverse = false
            };

            var doubleAnimaionUsingKeyFrames = new DoubleAnimationUsingKeyFrames();
            Storyboard.SetTarget(doubleAnimaionUsingKeyFrames, popup.Child);
            Storyboard.SetTargetProperty(doubleAnimaionUsingKeyFrames, "UIElement.Opacity");
            storyboard.Children.Add(doubleAnimaionUsingKeyFrames);

            //0.5秒透明度从0-1
            var doubleKeyFrame = new EasingDoubleKeyFrame
            {
                KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.5)),
                Value = 1,
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };
            doubleAnimaionUsingKeyFrames.KeyFrames.Add(doubleKeyFrame);
            doubleKeyFrame = new EasingDoubleKeyFrame
            {
                KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(2.4)),
                Value = 0.995,
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseIn }
            };
            doubleAnimaionUsingKeyFrames.KeyFrames.Add(doubleKeyFrame);
            doubleKeyFrame = new EasingDoubleKeyFrame
            {
                KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(3)),
                Value = 0.1,
            };
            doubleAnimaionUsingKeyFrames.KeyFrames.Add(doubleKeyFrame);



            doubleAnimaionUsingKeyFrames = new DoubleAnimationUsingKeyFrames();
            Storyboard.SetTarget(doubleAnimaionUsingKeyFrames, popup.Child);
            Storyboard.SetTargetProperty(doubleAnimaionUsingKeyFrames, "(UIElement.RenderTransform).(CompositeTransform.TranslateY)");
            storyboard.Children.Add(doubleAnimaionUsingKeyFrames);
            doubleKeyFrame = new EasingDoubleKeyFrame
            {
                KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.5)),
                Value = 0,
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };
            doubleAnimaionUsingKeyFrames.KeyFrames.Add(doubleKeyFrame);
            doubleKeyFrame = new EasingDoubleKeyFrame
            {
                KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(2.4)),
                Value = 0,
            };
            doubleAnimaionUsingKeyFrames.KeyFrames.Add(doubleKeyFrame);
            doubleKeyFrame = new EasingDoubleKeyFrame
            {
                KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(3)),
                Value = -height,
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseIn }
            };
            doubleAnimaionUsingKeyFrames.KeyFrames.Add(doubleKeyFrame);

            var objectAnimaionUsingKeyFrames = new ObjectAnimationUsingKeyFrames();
            Storyboard.SetTarget(objectAnimaionUsingKeyFrames, popup);
            Storyboard.SetTargetProperty(objectAnimaionUsingKeyFrames, "Popup.IsOpen");
            storyboard.Children.Add(objectAnimaionUsingKeyFrames);

            var discreteObjectKeyFrame = new DiscreteObjectKeyFrame
            {
                KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0)),
                Value = true
            };
            objectAnimaionUsingKeyFrames.KeyFrames.Add(discreteObjectKeyFrame);

            discreteObjectKeyFrame = new DiscreteObjectKeyFrame
            {
                KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(3)),
                Value = false
            };
            objectAnimaionUsingKeyFrames.KeyFrames.Add(discreteObjectKeyFrame);

            if (complete != null)
            {
                storyboard.Completed += (s, e) => complete.Invoke();
            }
            storyboard.Begin();
        }
    }
}
