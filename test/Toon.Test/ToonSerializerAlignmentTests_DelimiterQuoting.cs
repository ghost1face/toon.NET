using Xunit;

namespace Toon.Test
{
    public partial class ToonSerializerAlignmentTests
    {
        [Theory]
        [InlineData('\t', new[] { "a", "b\tc", "d" }, "a\t\"b\\tc\"\td")]
        [InlineData('|', new[] { "a", "b|c", "d" }, "a|\"b|c\"|d")]
        [Trait("Category", "Delimiter-aware quoting")]
        public void Quotes_Strings_Containing_Delimiter(char delimiter, string[] input, string expected)
        {
            var serializer = new ToonSerializer(new ToonSerializerSettings { Delimiter = delimiter });
            var obj = new { items = input };
            var result = serializer.Serialize(obj);
            var header = $"items[{input.Length}{delimiter}]: ";
            Assert.Equal(header + expected, result.Replace("\r\n", "\n"));
        }

        [Theory]
        [InlineData('\t', new[] { "a,b", "c,d" }, "a,b\tc,d")]
        [InlineData('|', new[] { "a,b", "c,d" }, "a,b|c,d")]
        [Trait("Category", "Delimiter-aware quoting")]
        public void Does_Not_Quote_Commas_With_NonComma_Delimiter(char delimiter, string[] input, string expected)
        {
            var serializer = new ToonSerializer(new ToonSerializerSettings { Delimiter = delimiter });
            var obj = new { items = input };
            var result = serializer.Serialize(obj);
            var header = $"items[{input.Length}{delimiter}]: ";
            Assert.Equal(header + expected, result.Replace("\r\n", "\n"));
        }
    }
}
