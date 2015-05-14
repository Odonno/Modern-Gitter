using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Gitter.Converters
{
    public class SamePreviousAuthorMarginConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool samePreviousAuthor = (bool)new SamePreviousAuthorConverter().Convert(value, null, null, string.Empty);
            return samePreviousAuthor ? new Thickness() : new Thickness(0, 0, 0, 6);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
