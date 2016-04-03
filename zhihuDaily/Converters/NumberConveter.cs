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
                    return (number / 1000d).ToString("0.0") + "k";
                }
            }
            return "...";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class ImageConveter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (parameter.ToString() == "comment")
            {
                return (bool)value ? "ms-appx:///Assets/FunIcon/collected.png" : "ms-appx:///Assets/FunIcon/collect.png";
            }
            else if (parameter.ToString() == "popularity")
            {
                int voteState = System.Convert.ToInt32(value);
                return voteState>=1 ? "ms-appx:///Assets/FunIcon/praised.png" : "ms-appx:///Assets/FunIcon/praise.png";
            }
            else if (parameter.ToString() == "editState")
            {
                return (bool)value ? "ms-appx:///Assets/FunIcon/profile_edit_done.png" : "ms-appx:///Assets/FunIcon/profile_edit.png";
            }
            else if (parameter.ToString() == "voted")
            {
                return (bool)value ? "ms-appx:///Assets/FunIcon/comment_voted.png" : "ms-appx:///Assets/FunIcon/comment_vote.png";
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class BoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class ForeBackbroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return !(bool)value ? "#666666" : "#000000";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
