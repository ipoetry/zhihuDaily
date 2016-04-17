using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace zhihuDaily.Converters
{
    class DateStyleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            DateTime newsDate;
            if (value != null)
            {
                //如 yyyy-MM-dd dddd HH:mm:ss   2014-10-27 星期一 13:26:30
                DateTime.TryParse(value.ToString(),out newsDate);
                if (newsDate.Date.Equals(DateTime.Now.Date))
                    return "今日热闻";
                else
                    return newsDate.ToString("MM月dd日  dddd", new CultureInfo("zh-CN"));
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
