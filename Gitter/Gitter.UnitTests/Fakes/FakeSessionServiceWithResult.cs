using Gitter.Services.Abstract;
using System;
using System.Threading.Tasks;

namespace Gitter.UnitTests.Fakes
{
    public class FakeSessionServiceWithResult : ISessionService
    {
        #region Properties

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

        #endregion
    }
}
