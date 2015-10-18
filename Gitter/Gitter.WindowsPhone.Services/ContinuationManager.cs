using System;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace GitHub.Services
{
    /// <summary>
    /// The continuation manager.
    /// </summary>
    public class ContinuationManager
    {
        private IContinuationActivatedEventArgs _continuationActivatedEventArgs = null;
        private bool _handled;
        private Guid _id = Guid.Empty;

        /// <summary>
        /// Sets the ContinuationArgs for this instance. Using default Frame of current Window
        /// Should be called by the main activation handling code in App.xaml.cs.
        /// </summary>
        /// <param name="args">The activation args.</param>
        internal void Continue(IContinuationActivatedEventArgs args)
        {
            Continue(args, Window.Current.Content as Frame);
        }

        /// <summary>
        /// Sets the ContinuationArgs for this instance. Should be called by the main activation
        /// handling code in App.xaml.cs.
        /// </summary>
        /// <param name="args">
        /// The activation args.
        /// </param>
        /// <param name="rootFrame">
        /// The frame control that contains the current page.
        /// </param>
        internal void Continue(IContinuationActivatedEventArgs args, Frame rootFrame)
        {
            if (args == null)
            {
                throw new ArgumentNullException("args");
            }

            if (_continuationActivatedEventArgs != null && !_handled)
            {
                throw new InvalidOperationException("Can't set args more than once");
            }

            _continuationActivatedEventArgs = args;
            _handled = false;
            _id = Guid.NewGuid();

            if (rootFrame == null)
            {
                return;
            }

            switch (args.Kind)
            {
                case ActivationKind.WebAuthenticationBrokerContinuation:
                    var wabPage = rootFrame.Content as IWebAuthenticationContinuable;
                    if (wabPage != null)
                    {
                        wabPage.ContinueWebAuthentication(args as WebAuthenticationBrokerContinuationEventArgs);
                    }
                    break;
            }
        }

        /// <summary>
        /// Marks the contination data as 'stale', meaning that it is probably no longer of
        /// any use. Called when the app is suspended (to ensure future activations don't appear
        /// to be for the same continuation) and whenever the continuation data is retrieved 
        /// (so that it isn't retrieved on subsequent navigations).
        /// </summary>
        internal void MarkAsStale()
        {
            _handled = true;
        }

        /// <summary>
        /// Gets the continuation args, if they have not already been retrieved, and
        /// prevents further retrieval via this property (to avoid accidentla double-usage)
        /// </summary>
        /// <value>
        /// The continuation arguments.
        /// </value>
        public IContinuationActivatedEventArgs ContinuationArgs
        {
            get
            {
                if (_handled)
                {
                    return null;
                }
                MarkAsStale();
                return _continuationActivatedEventArgs;
            }
        }

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public Guid Id
        {
            get { return _id; }
        }

        /// <summary>
        /// Retrieves the continuation args, optionally retrieving them even if they have already
        /// been retrieved.
        /// </summary>
        /// <param name="includeStaleArgs">
        /// Set to true to return args even if they have previously been returned.
        /// </param>
        /// <returns>
        /// The continuation args, or null if there aren't any.
        /// </returns>
        public IContinuationActivatedEventArgs GetContinuationArgs(bool includeStaleArgs)
        {
            if (!includeStaleArgs && _handled)
            {
                return null;
            }
            MarkAsStale();
            return _continuationActivatedEventArgs;
        }
    }

    /// <summary>
    /// Implement this interface if your page invokes the web authentication
    /// broker.
    /// </summary>
    public interface IWebAuthenticationContinuable
    {
        /// <summary>
        /// This method is invoked when the web authentication broker returns
        /// with the authentication result.
        /// </summary>
        /// <param name="args">
        /// Activated event args object that contains returned authentication token.
        /// </param>
        void ContinueWebAuthentication(WebAuthenticationBrokerContinuationEventArgs args);
    }
}
