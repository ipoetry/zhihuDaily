using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using zhihuDaily.DataService;

namespace zhihuDaily.Converters
{
    class LightModeConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value != null)
            {
             var themeValue = (ElementTheme)Enum.Parse(typeof(ElementTheme),value.ToString());
                if (themeValue == ElementTheme.Dark)
                {
                    return Functions.LoadResourceString("DayModeText");
                }
                else
                {
                    return Functions.LoadResourceString("NightModeText");
                }
            }
            return null;            
        }

        public object ConvertBack(object value, Type targetType, object parameter, string culture)
        {
            throw new NotImplementedException();
        }
    }
}
