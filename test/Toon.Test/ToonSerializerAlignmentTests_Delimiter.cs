using Xunit;

namespace Toon.Test
{
    public partial class ToonSerializerAlignmentTests
    {
        [Theory]
        [InlineData('\t', "reading\tgaming\tcoding")]
        [InlineData('|', "reading|gaming|coding")]
        [InlineData(',', "reading,gaming,coding")]
        [Trait("Category", "Basic delimiter usage")]
        public void Encodes_Primitive_Arrays_With_Delimiters(char delimiter, string expected)
        {
            var serializer = new ToonSerializer(new ToonSerializerSettings { Delimiter = delimiter });
            var obj = new { tags = new[] { "reading", "gaming", "coding" } };
            var result = serializer.Serialize(obj);
            var expectedHeader = $"tags[3{(delimiter != ',' ? delimiter.ToString() : "")}]: ";

            Assert.Equal(expectedHeader + expected, result.Replace("\r\n", "\n"));
        }
    }
}
