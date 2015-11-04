using Gitter.Services.Abstract;
using System;

namespace Gitter.UnitTests.Fakes
{
    public class FakeApplicationStorageService : IApplicationStorageService
    {
        public bool Exists(string key)
        {
            throw new NotImplementedException();
        }

        public void Remove(string key)
        {
            throw new NotImplementedException();
        }

        public object Retrieve(string key)
        {
            throw new NotImplementedException();
        }

        public void Save(string key, object value)
        {
            throw new NotImplementedException();
        }
    }
}
