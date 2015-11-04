using Gitter.Services.Abstract;
using System;
using System.Collections.Generic;

namespace Gitter.UnitTests.Fakes
{
    public class FakeApplicationStorageService : IApplicationStorageService
    {
        #region Properties

        public Dictionary<string, object> Results { get; } = new Dictionary<string, object>();

        #endregion


        #region Methods

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
            Results.Add(key, value);
        }

        #endregion
    }
}
