using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// Pour plus d'informations sur le modèle Application vide, consultez la page http://go.microsoft.com/fwlink/?LinkId=234227
using GitHub.Common;
using Newtonsoft.Json;
using Gitter.Views;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
#if WINDOWS_PHONE_APP
using GitHub.Services;
#endif

namespace Gitter
{
    /// <summary>
    /// Fournit un comportement spécifique à l'application afin de compléter la classe Application par défaut.
    /// </summary>
    public sealed partial class App : Application
    {
#region Fields

#if WINDOWS_PHONE_APP
        private static TransitionCollection _transitions;
        private ContinuationManager _continuationManager;
#endif

#endregion

#region Properties

        /// <summary>
        /// Allows tracking page views, exceptions and other telemetry through the Microsoft Application Insights service.
        /// </summary>
        public static TelemetryClient TelemetryClient;

        /// <summary>
        /// Selected Room Name when the app is launched
        /// </summary>
        public static string RoomName;

#endregion

#region Constructor

        /// <summary>
        /// Initialise l'objet d'application de singleton.  Il s'agit de la première ligne du code créé
        /// à être exécutée. Elle correspond donc à l'équivalent logique de main() ou WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();
            Suspending += OnSuspending;
        }

#endregion

#region Launched events

        /// <summary>
        /// Invoked when the application is launched normally by the end user.
        /// Other entry points will be used when the application is launched to open a specific file, 
        /// to display search results, and so forth.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
            // Retrieve room name (from toast notification)
            if (!string.IsNullOrWhiteSpace(e.Arguments))
            {
                var args = JsonConvert.DeserializeObject<Dictionary<string, string>>(e.Arguments);
                RoomName = args["id"];
            }
           
            var rootFrame = CreateRootFrame();
            await RestoreStatusAsync(e.PreviousExecutionState);

            rootFrame.Navigate(typeof(SplashScreenPage), e.Arguments);

            // Ensure the current window is active
            Window.Current.Activate();
        }

        private Frame CreateRootFrame()
        {
            var rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                // Set the default language
                rootFrame = new Frame { Language = Windows.Globalization.ApplicationLanguages.Languages[0] };

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;

                rootFrame.ContentTransitions = new TransitionCollection();
            }

            return rootFrame;
        }

        private async Task RestoreStatusAsync(ApplicationExecutionState previousExecutionState)
        {
            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (previousExecutionState == ApplicationExecutionState.Terminated)
            {
                // Restore the saved session state only when appropriate
                try
                {
                    await SuspensionManager.RestoreAsync();
                }
                catch (SuspensionManagerException)
                {
                    // Something went wrong restoring state.
                    // Assume there is no state and continue
                }
            }
        }

#if WINDOWS_PHONE_APP
        /// <summary>
        /// Handle OnActivated event to deal with File Open/Save continuation activation kinds
        /// </summary>
        /// <param name="e">Application activated event arguments, it can be casted to proper sub-type based on ActivationKind</param>
        protected async override void OnActivated(IActivatedEventArgs e)
        {
            base.OnActivated(e);

            _continuationManager = new ContinuationManager();

            Frame rootFrame = CreateRootFrame();

            await RestoreStatusAsync(e.PreviousExecutionState);

            if (rootFrame.Content == null)
            {
                rootFrame.Navigate(typeof(SplashScreenPage));
            }

            var continuationEventArgs = e as IContinuationActivatedEventArgs;
            if (continuationEventArgs != null)
            {
                // Call ContinuationManager to handle continuation activation
                _continuationManager.Continue(continuationEventArgs, rootFrame);
            }

            Window.Current.Activate();
        }
#endif

#endregion

#region Suspending Events

        /// <summary>
        /// Appelé lorsque l'exécution de l'application est suspendue.  L'état de l'application est enregistré
        /// sans savoir si l'application pourra se fermer ou reprendre sans endommager
        /// le contenu de la mémoire.
        /// </summary>
        /// <param name="sender">Source de la requête de suspension.</param>
        /// <param name="e">Détails de la requête de suspension.</param>
        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            await SuspensionManager.SaveAsync();
            deferral.Complete();
        }

#endregion

#region Transitions management

#if WINDOWS_PHONE_APP
        public static void FirstNavigate()
        {
            var rootFrame = Window.Current.Content as Frame;
            if (rootFrame != null)
                rootFrame.ContentTransitions = _transitions ?? new TransitionCollection { new NavigationThemeTransition() };
        }
#endif

#endregion

#region Telemetry management

        public static void StartTelemetry()
        {
#if DEBUG
            TelemetryClient = new TelemetryClient(new TelemetryConfiguration { DisableTelemetry = true });
#else
            TelemetryClient = new TelemetryClient();
#endif
        }

#endregion
    }
}