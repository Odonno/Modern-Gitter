namespace Gitter.Configuration
{
    public static class NotificationConstants
    {
        /// <summary>
        /// Key used to identify notifications coming from the Modern Gitter Application
        /// </summary>
        public static string Tag = "Modern Gitter";

        /// <summary>
        /// Delay, in seconds, used to clear notification inside the app
        /// Used in method INotificationService.ClearNotificationGroupAsync()
        /// </summary>
        public static double RemoveNotificationDelay = 3;
    }
}