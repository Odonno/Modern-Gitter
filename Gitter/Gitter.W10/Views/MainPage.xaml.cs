using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Gitter.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
        }


        private void SelectRoom_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (ActualWidth < 900)
                TogglePane();
        }

        private void TogglePane_Tapped(object sender, TappedRoutedEventArgs e)
        {
            TogglePane();
        }

        private void TogglePane()
        {
            splitView.IsPaneOpen = !splitView.IsPaneOpen;
        }
    }
}
