﻿using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Gitter.Converters
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (parameter != null && parameter.ToString() == "inverse")
                return (bool) value ? Visibility.Collapsed : Visibility.Visible;

            return (bool) value ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
