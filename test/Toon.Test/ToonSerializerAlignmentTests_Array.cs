using Xunit;

namespace Toon.Test
{
    public partial class ToonSerializerAlignmentTests
    {
        [Fact]
        public void Encodes_Arrays_Of_Primitives_At_Root_Level()
        {
            var arr = new object[] { "x", "y", "true", true, 10 };
            var serializer = new ToonSerializer();
            var result = serializer.Serialize(arr);
            Assert.Equal("[5]: x,y,\"true\",true,10", result);
        }

        [Fact]
        public void Encodes_Arrays_Of_Similar_Objects_Tabular()
        {
            var arr = new[] { new { id = 1 }, new { id = 2 } };
            var serializer = new ToonSerializer();
            var result = serializer.Serialize(arr);
            Assert.Equal("[2]{id}:\n  1\n  2", result.Replace("\r\n", "\n"));
        }
    }
}
