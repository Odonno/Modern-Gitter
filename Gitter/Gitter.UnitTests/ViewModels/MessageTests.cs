using Gitter.ViewModel.Concrete;
using GitterSharp.Model;
using System;
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

        [Fact]
        public void CreateMessage_Should_SetDefaultProperties()
        {
            // Arrange
            var message = new Message
            {
                Id = "123456",
                Text = "my message",
                Html = "<div>my message</div>",
                SentDate = DateTime.Now,
                UnreadByCurrent = false
            };
            var messageViewModel = new MessageViewModel(message);

            // Act

            // Assert
            Assert.Equal("123456", messageViewModel.Id);
            Assert.Equal("my message", messageViewModel.Text);
            Assert.Equal("<div>my message</div>", messageViewModel.Html);
            Assert.Equal(DateTime.Now.ToString(), messageViewModel.SentDate.ToString());
            Assert.Null(messageViewModel.User);
            Assert.True(messageViewModel.Read);
        }

        [Fact]
        public void Message_Should_HaveAuthor()
        {
            // Arrange
            var user = new User
            {
                Id = "abcdef",
                Username = "Odonno"
            };
            var message = new Message
            {
                User = user
            };
            var messageViewModel = new MessageViewModel(message);

            // Act

            // Assert
            Assert.NotNull(messageViewModel.User);
            Assert.Same(user, messageViewModel.User);
        }
    }
}
