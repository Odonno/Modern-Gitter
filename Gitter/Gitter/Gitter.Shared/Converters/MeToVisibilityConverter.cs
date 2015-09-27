using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Gitter.ViewModel;
using GitterSharp.Model;

namespace Gitter.Converters
{
    public class MeToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var user = value as User;

            if (user == null)
                throw new ArgumentNullException(nameof(value));

            bool isCurrentUser = (user.Id == ViewModelLocator.Main.CurrentUser.Id);

            if (parameter != null && parameter.ToString() == "inverse")
                return isCurrentUser ? Visibility.Collapsed : Visibility.Visible;

            return isCurrentUser ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
