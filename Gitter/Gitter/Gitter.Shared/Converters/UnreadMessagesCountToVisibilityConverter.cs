using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Gitter.Converters
{
    public class UnreadMessagesCountToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            int unreadCount = (int)value;
            return (unreadCount > 0) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
