using Xunit;

namespace Toon.Test
{
    public partial class ToonSerializerAlignmentTests
    {
        [Fact]
        [Trait("Category", "arrays of objects (tabular and list items)")]
        public void Uses_List_Format_For_Objects_With_Different_Fields()
        {
            var serializer = new ToonSerializer();
            var obj = new { items = new object[] { new { id = 1, name = "First" }, new { id = 2, name = "Second", extra = true } } };

            var result = serializer.Serialize(obj);

            Assert.Contains("items[2]:", result.Replace("\r\n", "\n"));
            Assert.Contains("- id: 1", result.Replace("\r\n", "\n"));
            Assert.Contains("name: First", result.Replace("\r\n", "\n"));
            Assert.Contains("- id: 2", result.Replace("\r\n", "\n"));
            Assert.Contains("extra: true", result.Replace("\r\n", "\n"));
        }

        [Fact]
        [Trait("Category", "arrays of objects (tabular and list items)")]
        public void Uses_List_Format_For_Objects_With_Nested_Values()
        {
            var serializer = new ToonSerializer();
            var obj = new { items = new object[] { new { id = 1, nested = new { x = 1 } } } };

            var result = serializer.Serialize(obj);

            Assert.Contains("items[1]:", result.Replace("\r\n", "\n"));
            Assert.Contains("- id: 1", result.Replace("\r\n", "\n"));
            Assert.Contains("nested:", result.Replace("\r\n", "\n"));
            Assert.Contains("x: 1", result.Replace("\r\n", "\n"));
        }
    }
}
