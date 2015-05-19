using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Gitter.Converters
{
    public class NullToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (parameter != null && parameter.ToString() == "inverse")
                return (value != null) ? Visibility.Collapsed : Visibility.Visible;

            return (value == null) ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
