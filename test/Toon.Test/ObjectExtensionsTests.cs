using Toon.Test.Models;
using Xunit;

namespace Toon.Test
{
    public class ObjectExtensionsTests
    {
        [Fact]
        public void GetPublicProperties_ReturnsProperties()
        {
            var user = new User { Id = 1, Name = "Test", Role = "role" };

            var props = Extensions.ObjectExtensions.GetPublicProperties(user);

            Assert.Contains(props, p => p.Name == "Id");
            Assert.Contains(props, p => p.Name == "Name");
            Assert.Contains(props, p => p.Name == "Role");
        }

        [Fact]
        public void GetValue_ReturnsCorrectValue()
        {
            var user = new User { Id = 42, Name = "Test", Role = "role" };

            var value = Extensions.ObjectExtensions.GetValue(user, "Id");

            Assert.Equal(42, value);
        }
    }
}
