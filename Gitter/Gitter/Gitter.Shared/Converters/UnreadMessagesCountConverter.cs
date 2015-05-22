using System;
using Windows.UI.Xaml.Data;

namespace Gitter.Converters
{
    public class UnreadMessagesCountConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            int unreadCount = (int) value;
            return (unreadCount > 50) ? "50+" : unreadCount.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
