using System;
using Windows.UI.Xaml.Data;

namespace Gitter.Converters
{
    public class NullToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (parameter != null && parameter.ToString() == "inverse")
                return (value == null);

            return (value != null);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
