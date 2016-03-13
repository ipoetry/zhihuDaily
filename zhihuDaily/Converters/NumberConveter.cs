using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Data;

namespace zhihuDaily.Converters
{
    public class NumberConveter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value != null)
            {
               double number = double.Parse(value.ToString());
                if (number < 1000)
                {
                    return number;
                }
                else {
                    return (number / 1000d).ToString("0.0")+"k";
                }
            }
            return "...";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
