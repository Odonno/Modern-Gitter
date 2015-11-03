using Gitter.Services.Abstract;
using System;
using System.Threading.Tasks;

namespace Gitter.UnitTests.Fakes
{
    public class FakeSessionService : ISessionService
    {
        public Task<bool?> LoginAsync()
        {
            throw new NotImplementedException();
        }

        public void Logout()
        {
            throw new NotImplementedException();
        }
    }
}
