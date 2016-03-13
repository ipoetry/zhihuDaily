﻿using System;
using Windows.UI.Xaml.Data;

namespace zhihuDaily.Converters
{
    public class NullableBooleanToBoolean : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (value as bool?) == true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return new Nullable<bool>((bool)value);
        }
    }
}
