using Xunit;

namespace Toon.Test
{
    public partial class ToonSerializerAlignmentTests
    {
        [Fact]
        public void Uses_List_Format_For_Nested_Object_Arrays_With_Mismatched_Keys()
        {
            var serializer = new ToonSerializer();
            var obj = new { items = new[] { new { users = new object[] { new { id = 1, name = "Ada" }, new { id = 2 } }, status = "active" } } };
            var result = serializer.Serialize(obj);
            Assert.Contains("items[1]:", result.Replace("\r\n", "\n"));
            Assert.Contains("- users[2]:", result.Replace("\r\n", "\n"));
            Assert.Contains("- id: 1", result.Replace("\r\n", "\n"));
            Assert.Contains("name: Ada", result.Replace("\r\n", "\n"));
            Assert.Contains("- id: 2", result.Replace("\r\n", "\n"));
            Assert.Contains("status: active", result.Replace("\r\n", "\n"));
        }

        [Fact]
        public void Uses_List_Format_For_Objects_With_Multiple_Array_Fields()
        {
            var serializer = new ToonSerializer();
            var obj = new { items = new[] { new { a = new[] { 1, 2 }, b = new[] { 3, 4 } } } };
            var result = serializer.Serialize(obj);
            Assert.Contains("items[1]:", result.Replace("\r\n", "\n"));
            Assert.Contains("- a[2]: 1,2", result.Replace("\r\n", "\n"));
            Assert.Contains("b[2]: 3,4", result.Replace("\r\n", "\n"));
        }
    }
}
