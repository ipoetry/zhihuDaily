using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace zhihuDaily.Converters
{
    public class BoolVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null || parameter == null) 
                    return Visibility.Collapsed;
            string para = parameter.ToString();
            int Id = (int)value;
            System.Diagnostics.Debug.WriteLine(para + ":::" + Id);
            if (para=="news")
                return Id<0 ? Visibility.Visible : Visibility.Collapsed;
            else
                return Id>0 ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
