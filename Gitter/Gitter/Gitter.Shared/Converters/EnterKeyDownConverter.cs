using System;
using Windows.System;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;

namespace Gitter.Converters
{
    public class EnterKeyDownConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var args = (KeyRoutedEventArgs)value;
            return (args != null && args.Key == VirtualKey.Enter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
