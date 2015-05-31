using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
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
using Gitter.Services.Abstract;
using Gitter.ViewModel;
using Microsoft.Practices.ServiceLocation;

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

            // navigate to the chat when user select a room
            Messenger.Default.Register<SelectRoomMessage>(this, message =>
            {
                hub.ScrollToSection(chatSection);
                ViewModelLocator.Main.CurrentSectionIndex++;
            });
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
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            _navigationHelper.OnNavigatedTo(e);

            // Remove SplashScreen page
            if (e.NavigationMode == NavigationMode.New)
            {
                // Remove Splashscreen
                Frame.BackStack.Remove(Frame.BackStack.LastOrDefault());

                // Register background tasks
                await ServiceLocator.Current.GetInstance<IBackgroundTaskService>().RegisterTasksAsync();

#if !DEBUG
                // Ask user to rate the app
                ServiceLocator.Current.GetInstance<IRatingService>().AskForRating();
#endif

                // Select room if there os a value in the app launcher
                if (!string.IsNullOrWhiteSpace(App.RoomName))
                    ViewModelLocator.Main.SelectRoom(App.RoomName);
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            _navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        #endregion


        #region Sending Message

        private void SendMessage_OnKeyDown(object sender, KeyRoutedEventArgs e)
        {
            // Hide the virtual keyboard when sending a message
            if (e.Key == VirtualKey.Enter)
                Focus(FocusState.Programmatic);
        }

        #endregion


        #region Hub Section

        private void hub_SectionsInViewChanged(object sender, SectionsInViewChangedEventArgs e)
        {
            var currentSection = hub.SectionsInView.First();
            var currentIndex = hub.Sections.IndexOf(currentSection);

            // Update index when changing current section
            ViewModelLocator.Main.CurrentSectionIndex = currentIndex;
        }

        #endregion
    }
}
