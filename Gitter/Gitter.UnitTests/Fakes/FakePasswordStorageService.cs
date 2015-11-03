using Gitter.Services.Abstract;
using System;

namespace Gitter.UnitTests.Fakes
{
    public class FakePasswordStorageService : IPasswordStorageService
    {
        public string Retrieve(string key)
        {
            return "123456";
        }

        public void Save(string key, string password)
        {
            throw new NotImplementedException();
        }
    }
}
