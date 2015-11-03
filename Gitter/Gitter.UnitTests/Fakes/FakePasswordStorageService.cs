using Gitter.Services.Abstract;
using System;

namespace Gitter.UnitTests.Fakes
{
    public class FakePasswordStorageService : IPasswordStorageService
    {
        public string Content { get; set; }

        public string Retrieve(string key)
        {
            return Content;
        }

        public void Save(string key, string password)
        {
            throw new NotImplementedException();
        }
    }
}
