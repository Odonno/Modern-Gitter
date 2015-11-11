using Gitter.Services.Abstract;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;

namespace Gitter.UnitTests.Fakes
{
    public class FakeSessionServiceWithResult : ISessionService
    {
        #region Fake Properties

        public bool? Result { get; set; }

        #endregion


        #region Methods

        public Task<bool?> LoginAsync()
        {
            return Task.FromResult(Result);
        }

        public void Logout()
        {
            throw new NotImplementedException();
        }

        public Task<bool> FinalizeAsync(WebAuthenticationBrokerContinuationEventArgs args)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
