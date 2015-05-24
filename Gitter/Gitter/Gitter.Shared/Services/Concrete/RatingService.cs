using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Resources;
using Windows.Storage;
using Windows.System;
using Windows.UI.Popups;
using Gitter.Services.Abstract;

namespace Gitter.Services.Concrete
{
    public class RatingService : IRatingService
    {
        #region Properties

        private readonly ResourceLoader _resourceLoader = ResourceLoader.GetForCurrentView("Resources");

        public int StartedCount { get; private set; }
        public bool ReviewedBefore { get; private set; }
        public string LastVersionStarted { get; private set; }

        #endregion


        #region Methods

        /// <summary>
        /// Ask for a review after 5 starts of a new version of the Application
        /// </summary>
        public async void AskForRating()
        {
            // Don't ask for rating when in beta (0.X)
            if (Package.Current.Id.Version.Major < 1)
                return;

            // Variables to hold the current and the last version number that was started in a string
            string currentVersion = string.Format("{0} - {1} - {2} - {3}",
                Package.Current.Id.Version.Major, Package.Current.Id.Version.Minor, Package.Current.Id.Version.Build, Package.Current.Id.Version.Revision);
            LastVersionStarted = currentVersion;

            // Check if the variables were stored in the Roaming Settings before loading them
            if (ApplicationData.Current.RoamingSettings.Values.ContainsKey("startedCount"))
                StartedCount = (int)ApplicationData.Current.RoamingSettings.Values["startedCount"];

            if (ApplicationData.Current.RoamingSettings.Values.ContainsKey("reviewedBefore"))
                ReviewedBefore = (bool)ApplicationData.Current.RoamingSettings.Values["reviewedBefore"];

            if (ApplicationData.Current.RoamingSettings.Values.ContainsKey("lastVersionStarted"))
                LastVersionStarted = (string)ApplicationData.Current.RoamingSettings.Values["lastVersionStarted"];

            // If the current version is equal to the last version started
            if (currentVersion.Equals(LastVersionStarted))
                StartedCount++;
            else
                // Reset the count because the new version might have fixed some problems or be better
                StartedCount = 0;

            // And then save the variables in the Roaming Settings
            ApplicationData.Current.RoamingSettings.Values["startedCount"] = StartedCount;
            ApplicationData.Current.RoamingSettings.Values["lastVersionStarted"] = currentVersion;

            // If the app was started 5 times
            if (StartedCount == 5)
            {
                // Change the msg based on the fact that the application was reviewed before or not.
                string msg = !ReviewedBefore ?
                    _resourceLoader.GetString("ThanksToUseIt") :
                    string.Format("{1}{0}{2}{0}{3}", Environment.NewLine, _resourceLoader.GetString("ThanksReview1"), _resourceLoader.GetString("ThanksReview2"), _resourceLoader.GetString("ThanksReview3"));

                // Create a MessageDialog requesting a review
                var md = new MessageDialog(msg, _resourceLoader.GetString("MarkTheApp"));

                // Build the 3 potential responses using the Commands collection :

                // If they say yes, send them to the store
                md.Commands.Add(new UICommand(_resourceLoader.GetString("Yes"), async command =>
                {
                    // Find the FamilyName of the app package
                    string familyName = Package.Current.Id.FamilyName;

                    // Navigate to the store Review Page for the Application
                    await Launcher.LaunchUriAsync(new Uri(string.Format("ms-windows-store:REVIEW?PFN={0}", familyName)));

                    // Change the status to track the fact that they agreed to a review
                    ReviewedBefore = true;

                    // Store the reviewedBefore variable in local storage
                    ApplicationData.Current.RoamingSettings.Values["reviewedBefore"] = ReviewedBefore;
                }));

                // If they say ask later, reset the count to 0 giving a grace period of 5 again
                md.Commands.Add(new UICommand(_resourceLoader.GetString("Later"), command =>
                {
                    StartedCount = 0;
                    ApplicationData.Current.RoamingSettings.Values["startedCount"] = StartedCount;
                }));

                // Select index of commands
                md.DefaultCommandIndex = 0;
                md.CancelCommandIndex = 1;

                // Display the MessageBox
                await md.ShowAsync();
            }
        }

        #endregion

    }
}
