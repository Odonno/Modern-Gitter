using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using GitHub.Common;
using Newtonsoft.Json;
using Gitter.Views;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Practices.ServiceLocation;
using Gitter.ViewModel;
using Windows.Globalization;
using Gitter.Services.Abstract;
#if WINDOWS_PHONE_APP
using Gitter.WindowsPhone.Services;
#endif

namespace Gitter
{
    public sealed partial class App : Application
    {
        #region Fields

#if WINDOWS_PHONE_APP
        private ContinuationManager _continuationManager;
#endif

        #endregion


        #region Properties

        /// <summary>
        /// Allows tracking page views, exceptions and other telemetry through the Microsoft Application Insights service.
        /// </summary>
        public static TelemetryClient TelemetryClient;

        /// <summary>
        /// Selected Room Name when the app is launched (from toast notification)
        /// </summary>
        public static string RoomName;

        #endregion


        #region Constructor

        /// <summary>
        /// Initialize Application (Entry Point)
        /// </summary>
        public App()
        {
            InitializeComponent();

            Resuming += OnResuming;
            Suspending += OnSuspending;
        }

        #endregion


        #region App Lifecycle Events

        /// <summary>
        /// Invoked when the application is launched normally by the end user
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
            // Retrieve room name (from toast notification)
            if (!string.IsNullOrWhiteSpace(e.Arguments))
            {
                var args = JsonConvert.DeserializeObject<Dictionary<string, string>>(e.Arguments);
                RoomName = args["id"].Split(new[] { '_' })[0];
            }

            var rootFrame = CreateRootFrame();
            await RestoreStatusAsync(e.PreviousExecutionState);

            rootFrame.Navigate(typeof(SplashScreenPage));

            // Ensure the current window is active
            Window.Current.Activate();
        }

#if WINDOWS_PHONE_APP
        /// <summary>
        /// Handle OnActivated event to handle continuation activation (Authentication)
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

        /// <summary>
        /// Called when the app is resumed
        /// </summary>
        /// <param name="sender">Source of the resuming request</param>
        /// <param name="e">Details of the resuming request</param>
        private void OnResuming(object sender, object e)
        {
            // Re-open all realtime streams
            if (ServiceLocator.Current != null)
                ViewModelLocator.Main.OpenRealtimeStreams();
        }

        /// <summary>
        /// Called when the app is suspended
        /// </summary>
        /// <param name="sender">Source of the suspension request</param>
        /// <param name="e">Details of the suspension request</param>
        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();

            // Save data from the Suspension Manager
            await SuspensionManager.SaveAsync();

            // Close all realtime streams
            if (ServiceLocator.Current != null)
                ViewModelLocator.Main.CloseRealtimeStreams();

            deferral.Complete();
        }

        #endregion


        #region Methods

        /// <summary>
        /// Create a Root Frame for the entire app navigation
        /// </summary>
        /// <returns></returns>
        private Frame CreateRootFrame()
        {
            var rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame { Language = ApplicationLanguages.Languages[0] };

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;

                rootFrame.ContentTransitions = new TransitionCollection();
            }

            return rootFrame;
        }

        /// <summary>
        /// Restore App Status if it was terminated
        /// </summary>
        /// <param name="previousExecutionState"></param>
        /// <returns></returns>
        private async Task RestoreStatusAsync(ApplicationExecutionState previousExecutionState)
        {
            if (previousExecutionState == ApplicationExecutionState.Running ||
                previousExecutionState == ApplicationExecutionState.Suspended)
            {
                // Re-open all realtime streams ?
            }

            // Do not repeat app initialization when the Window already has content
            if (previousExecutionState == ApplicationExecutionState.Terminated)
            {
                try
                {
                    // Restore the saved session state only when appropriate
                    await SuspensionManager.RestoreAsync();
                }
                catch (SuspensionManagerException)
                {
                    // Something went wrong restoring state
                    // Assume there is no state and continue
                }
            }
        }

        #endregion


        #region Transitions management

#if WINDOWS_PHONE_APP
        /// <summary>
        /// Once the app is initialized (after the Splashscreen), reset Transitions
        /// </summary>
        public static void FirstNavigate()
        {
            var rootFrame = Window.Current.Content as Frame;
            if (rootFrame != null)
                rootFrame.ContentTransitions = new TransitionCollection
                {
                    new NavigationThemeTransition()
                };
        }
#endif

        #endregion


        #region Telemetry management

        /// <summary>
        /// Start Telemetry client
        /// </summary>
        public static void StartTelemetry()
        {
#if DEBUG
            TelemetryClient = new TelemetryClient(new TelemetryConfiguration { DisableTelemetry = true });
#else
            TelemetryClient = new TelemetryClient();
#endif

            ServiceLocator.Current.GetInstance<ITelemetryService>().Create();
        }

        #endregion
    }
}