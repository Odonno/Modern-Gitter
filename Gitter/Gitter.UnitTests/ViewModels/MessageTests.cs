using Gitter.ViewModel.Concrete;
using GitterSharp.Model;
using Xunit;

namespace Gitter.UnitTests.ViewModels
{
    public class MessageTests
    {
        [Fact]
        public void CreateMessage_Should_SetMessage()
        {
            // Arrange
            var message = new Message();
            var messageViewModel = new MessageViewModel(message);

            // Act

            // Assert
            Assert.Same(message, messageViewModel.Message);
        }
    }
}
