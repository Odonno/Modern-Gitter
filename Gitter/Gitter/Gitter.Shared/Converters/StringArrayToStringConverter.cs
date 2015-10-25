using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Data;

namespace Gitter.Converters
{
    public class StringArrayToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var strings = value as IEnumerable<string>;
            return string.Join("\n", strings);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
