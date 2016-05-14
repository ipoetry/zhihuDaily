using System;
using Windows.Graphics.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace zhihuDaily.Converters
{
    public class WidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var bounds = Window.Current.Bounds;
            var dpiRatio = DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;
            var resolutionW = Math.Round(bounds.Width * dpiRatio);
            if (resolutionW == 1080)
                return 155;
            if (resolutionW == 720)
                return 145;
            if (resolutionW == 480)
                return 140;
            return 140;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    class TotalCommentsFormat : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return "共" + value.ToString() + "条评论";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// 
    /// </summary>
    class TimeFormat : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            long t_s = long.Parse(value.ToString());
            DateTime t = DateTime.Parse("1970/1/1").AddSeconds(t_s);
            return t.AddHours(8).ToString("MM-dd HH:mm", new System.Globalization.CultureInfo("zh-CN"));
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    class BorderBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var isLight = (ElementTheme)value == ElementTheme.Light;
            var startColor = isLight ? Windows.UI.Colors.Transparent : "#ff4b4b4b".ToColor();
            var endColor = isLight ? Windows.UI.Colors.LightGray : "#ff343434".ToColor();
            var lgb = new LinearGradientBrush();
            lgb.Opacity = 0.9;
            lgb.StartPoint = new Windows.Foundation.Point(0,0);
            lgb.EndPoint = new Windows.Foundation.Point(0,1);
            lgb.GradientStops.Add(new GradientStop { Color = endColor, Offset=1 });
            lgb.GradientStops.Add(new GradientStop { Color = startColor, Offset = 0 });
            
            return lgb;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    class NotificationTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            int type = System.Convert.ToInt32(value);
            string para = System.Convert.ToString(parameter);
            if (type == 1)
                return para=="des"?" 赞了你的点评：": "ms-appx:///Assets/FunIcon/message_vote.png";
            else if (type == 2)
                return para == "des" ? " 回复了你的点评：": "ms-appx:///Assets/FunIcon/message_reply.png";
            else
                return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
