using Xunit;

namespace Toon.Test
{
    public partial class ToonSerializerAlignmentTests
    {
        [Fact]
        [Trait("Category", "arrays of arrays (primitives only)")]
        public void Handles_Empty_Inner_Arrays()
        {
            var serializer = new ToonSerializer();
            var obj = new { pairs = new object[][] { new object[0], new object[0] } };

            var result = serializer.Serialize(obj);

            Assert.Equal("pairs[2]:\n  - [0]:\n  - [0]:", result.Replace("\r\n", "\n"));
        }

        [Fact]
        [Trait("Category", "arrays of arrays (primitives only)")]
        public void Handles_Mixed_Length_Inner_Arrays()
        {
            var serializer = new ToonSerializer();
            var obj = new { pairs = new object[][] { new object[] { 1 }, new object[] { 2, 3 } } };

            var result = serializer.Serialize(obj);

            Assert.Equal("pairs[2]:\n  - [1]: 1\n  - [2]: 2,3", result.Replace("\r\n", "\n"));
        }
    }
}
