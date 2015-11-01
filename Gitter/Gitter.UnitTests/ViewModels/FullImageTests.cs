using System;
using Gitter.ViewModel.Concrete;
using Xunit;

namespace Gitter.UnitTests.ViewModels
{
    public class FullImageTests
    {
        [Theory]
        [InlineData("")]
        [InlineData("nothing")]
        [InlineData("http://www.github.com/octo.png")]
        public void SetImageSource_Should_ReturnTheSource(string source)
        {
            // Arrange
            var fullImageViewModel = new FullImageViewModel();

            // Act
            fullImageViewModel.Source = source;

            // Assert
            Assert.Equal(source, fullImageViewModel.Source);
        }
    }
}
