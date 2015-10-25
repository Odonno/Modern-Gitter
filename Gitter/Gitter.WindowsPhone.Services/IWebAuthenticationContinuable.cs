using Windows.ApplicationModel.Activation;

namespace Gitter.WindowsPhone.Services
{
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
