using System;
using Windows.Graphics.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

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
}
