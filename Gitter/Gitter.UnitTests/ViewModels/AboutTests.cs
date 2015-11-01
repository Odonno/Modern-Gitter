using System;
using System.Linq;
using Gitter.ViewModel.Concrete;
using Xunit;

namespace Gitter.UnitTests.ViewModels
{
    public class AboutTests
    {
        [Fact]
        public void ApplicationVersion_Should_ContainsThreePoints()
        {
            // Arrange
            var aboutViewModel = new AboutViewModel();

            // Act
            int result = aboutViewModel.ApplicationVersion.Count(c => c == '.');

            // Assert
            Assert.Equal(3, result);
        }
    }
}
