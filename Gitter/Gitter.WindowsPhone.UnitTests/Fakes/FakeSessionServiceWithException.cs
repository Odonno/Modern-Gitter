using Gitter.Services.Abstract;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;

namespace Gitter.UnitTests.Fakes
{
    public class FakeSessionServiceWithException : ISessionService
    {
        public Task<bool?> LoginAsync()
        {
            throw new NotImplementedException();
        }

        public void Logout()
        {
            throw new NotImplementedException();
        }

        public Task<bool> FinalizeAsync(WebAuthenticationBrokerContinuationEventArgs args)
        {
            throw new NotImplementedException();
        }
    }
}
