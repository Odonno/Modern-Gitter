﻿#if !WINDOWS_UWP
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
using Microsoft.Xaml.Interactivity;

namespace Gitter.Behaviors
{
    public class OpenMenuFlyoutAction : DependencyObject, IAction
    {
        public object Execute(object sender, object parameter)
        {
            var senderElement = sender as FrameworkElement;
            var flyoutBase = FlyoutBase.GetAttachedFlyout(senderElement);

            flyoutBase.ShowAt(senderElement);

            return null;
        }
    }
}
#endif