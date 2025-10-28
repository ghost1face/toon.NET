using Xunit;

namespace Toon.Test
{
    public partial class ToonSerializerAlignmentTests
    {
        [Fact]
        [Trait("Category", "Length Marker Option")]
        public void Adds_LengthMarker_To_Primitive_Arrays()
        {
            var serializer = new ToonSerializer(new ToonSerializerSettings { LengthMarker = '#' });
            var obj = new { tags = new[] { "reading", "gaming", "coding" } };

            var result = serializer.Serialize(obj);

            Assert.Equal("tags[#3]: reading,gaming,coding", result);
        }

        [Fact]
        [Trait("Category", "Length Marker Option")]
        public void Handles_Empty_Arrays_With_LengthMarker()
        {
            var serializer = new ToonSerializer(new ToonSerializerSettings { LengthMarker = '#' });
            var obj = new { items = new object[0] };

            var result = serializer.Serialize(obj);

            Assert.Equal("items[#0]:", result.Replace("\r\n", "\n"));
        }

        [Fact]
        [Trait("Category", "Length Marker Option")]
        public void Adds_LengthMarker_To_Tabular_Arrays()
        {
            var serializer = new ToonSerializer(new ToonSerializerSettings { LengthMarker = '#' });
            var obj = new { items = new[] { new { sku = "A1", qty = 2, price = 9.99 }, new { sku = "B2", qty = 1, price = 14.5 } } };

            var result = serializer.Serialize(obj);

            Assert.Contains("items[#2]{sku,qty,price}:", result.Replace("\r\n", "\n"));
        }

        [Fact]
        [Trait("Category", "Length Marker Option")]
        public void Adds_LengthMarker_To_Nested_Arrays()
        {
            var serializer = new ToonSerializer(new ToonSerializerSettings { LengthMarker = '#' });
            var obj = new { pairs = new[] { new[] { "a", "b" }, new[] { "c", "d" } } };

            var result = serializer.Serialize(obj);

            Assert.Contains("pairs[#2]:", result.Replace("\r\n", "\n"));
        }

        [Fact]
        [Trait("Category", "Length Marker Option")]
        public void Works_With_Delimiter_Option()
        {
            var serializer = new ToonSerializer(new ToonSerializerSettings { LengthMarker = '#', Delimiter = '|' });
            var obj = new { tags = new[] { "reading", "gaming", "coding" } };

            var result = serializer.Serialize(obj);

            Assert.Equal("tags[#3|]: reading|gaming|coding", result.Replace("\r\n", "\n"));
        }

        [Fact]
        [Trait("Category", "Length Marker Option")]
        public void Default_Is_False_No_LengthMarker()
        {
            var serializer = new ToonSerializer();
            var obj = new { tags = new[] { "reading", "gaming", "coding" } };

            var result = serializer.Serialize(obj);

            Assert.Equal("tags[3]: reading,gaming,coding", result.Replace("\r\n", "\n"));
        }
    }
}
