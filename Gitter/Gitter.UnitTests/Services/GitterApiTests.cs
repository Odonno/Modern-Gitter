using Gitter.API.Services.Abstract;
using Gitter.API.Services.Concrete;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gitter.UnitTests.Services
{
    [TestClass]
    public class GitterApiTests
    {
        private IGitterApiService _gitterApiService;


        [TestInitialize]
        public void TestInitialize()
        {
            _gitterApiService = new GitterApiService();
        }
    }
}
