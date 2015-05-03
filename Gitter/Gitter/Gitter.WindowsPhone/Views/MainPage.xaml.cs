using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Pour en savoir plus sur le modèle d'élément Page vierge, consultez la page http://go.microsoft.com/fwlink/?LinkId=234238
using GalaSoft.MvvmLight.Messaging;
using GitHub.Common;
using Gitter.Messages;
using Gitter.ViewModel;
using WinRTXamlToolkit.Controls.Extensions;

namespace Gitter
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            // retrieve status bar
            var statusBar = StatusBar.GetForCurrentView();

            // set status bar color
            statusBar.ForegroundColor = Colors.Black;

            InitializeComponent();

            _navigationHelper = new NavigationHelper(this);
            _navigationHelper.LoadState += NavigationHelper_LoadState;
            _navigationHelper.SaveState += NavigationHelper_SaveState;

            Messenger.Default.Register<RoomRefreshedMessage>(this, _ => ScrollToBottom());
        }


        #region Navigation Helper

        private readonly NavigationHelper _navigationHelper;

        /// <summary>
        /// Obtient le <see cref="NavigationHelper"/> associé à ce <see cref="Page"/>.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return _navigationHelper; }
        }


        /// <summary>
        /// Remplit la page à l'aide du contenu passé lors de la navigation. Tout état enregistré est également
        /// fourni lorsqu'une page est recréée à partir d'une session antérieure.
        /// </summary>
        /// <param name="sender">
        /// La source de l'événement ; en général <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Données d'événement qui fournissent le paramètre de navigation transmis à
        /// <see cref="Frame.Navigate(Type, Object)"/> lors de la requête initiale de cette page et
        /// un dictionnaire d'état conservé par cette page durant une session
        /// antérieure.  L'état n'aura pas la valeur Null lors de la première visite de la page.</param>
        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
        }

        /// <summary>
        /// Conserve l'état associé à cette page en cas de suspension de l'application ou de
        /// suppression de la page du cache de navigation.  Les valeurs doivent être conformes aux
        /// exigences en matière de sérialisation de <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">La source de l'événement ; en général <see cref="NavigationHelper"/></param>
        /// <param name="e">Données d'événement qui fournissent un dictionnaire vide à remplir à l'aide de l'
        /// état sérialisable.</param>
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region Inscription de NavigationHelper

        /// <summary>
        /// Les méthodes fournies dans cette section sont utilisées simplement pour permettre
        /// NavigationHelper pour répondre aux méthodes de navigation de la page.
        /// <para>
        /// La logique spécifique à la page doit être placée dans les gestionnaires d'événements pour  
        /// <see cref="NavigationHelper.LoadState"/>
        /// et <see cref="NavigationHelper.SaveState"/>.
        /// Le paramètre de navigation est disponible dans la méthode LoadState 
        /// en plus de l'état de page conservé durant une session antérieure.
        /// </para>
        /// </summary>
        /// <param name="e">Fournit des données pour les méthodes de navigation et
        /// les gestionnaires d'événements qui ne peuvent pas annuler la requête de navigation.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _navigationHelper.OnNavigatedTo(e);

            // Remove SplashScreen page
            if (e.NavigationMode == NavigationMode.New)
            {
                Frame.BackStack.Remove(Frame.BackStack.LastOrDefault());
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            _navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        #endregion


        #region Scroll management (Message List)

        private ListView _messagesListView;
        private double _previousDiffOffset;
        private bool _check;
        private DateTime _lastRefresh = DateTime.Now;


        private void MessagesListView_OnLoaded(object sender, RoutedEventArgs e)
        {
            _messagesListView = sender as ListView;
            MessagesListView_TopEdgeDetection();
        }

        private void ScrollToBottom()
        {
            if (_messagesListView.Items == null)
                return;

            var selectedIndex = _messagesListView.Items.Count - 1;
            if (selectedIndex < 0)
                return;

            _messagesListView.SelectedIndex = selectedIndex;

            _messagesListView.UpdateLayout();
            _messagesListView.ScrollIntoView(_messagesListView.SelectedItem);
        }

        private void MessagesListView_TopEdgeDetection()
        {
            var scrollViewer = _messagesListView.GetFirstDescendantOfType<ScrollViewer>();

            scrollViewer.ViewChanged += async (sender, e) =>
            {
                if (scrollViewer.VerticalOffset <= 0 && DateTime.Now.Subtract(_lastRefresh).TotalSeconds > 0.5)
                {
                    _previousDiffOffset = scrollViewer.ExtentHeight;

                    await ViewModelLocator.Main.SelectedRoom.Messages.LoadMoreItemsAsync(
                        (uint)ViewModelLocator.Main.SelectedRoom.Messages.ItemsPerPage);

                    _lastRefresh = DateTime.Now;
                    _check = true;
                }

                if (_check && Math.Abs(_previousDiffOffset - scrollViewer.ExtentHeight) > 10)
                {
                    _previousDiffOffset = scrollViewer.ExtentHeight - _previousDiffOffset;
                    scrollViewer.ScrollToVerticalOffset(_previousDiffOffset);
                    _check = false;
                }
            };
        }

        #endregion
    }
}
