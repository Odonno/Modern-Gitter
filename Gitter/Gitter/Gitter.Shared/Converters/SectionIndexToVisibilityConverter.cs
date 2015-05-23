using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Gitter.Converters
{
    public class SectionIndexToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            int currentIndex = int.Parse(value.ToString());
            int requiredIndex = int.Parse(parameter.ToString());

            return (currentIndex == requiredIndex) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
