using Xunit;

namespace Toon.Test
{
    public partial class ToonSerializerAlignmentTests
    {
        [Theory]
        [InlineData('\t')]
        [InlineData('|')]
        [Trait("Category", "Formatting Invariants with Delimiters")]
        public void Produces_No_Trailing_Spaces(char delimiter)
        {
            var serializer = new ToonSerializer(new ToonSerializerSettings { Delimiter = delimiter });
            var obj = new { user = new { id = 123, name = "Ada" }, items = new[] { "a", "b" } };
            var result = serializer.Serialize(obj);
            var lines = result.Replace("\r\n", "\n").Split('\n');
            foreach (var line in lines)
            {
                Assert.False(line.EndsWith(" "));
            }
        }

        [Theory]
        [InlineData('\t')]
        [InlineData('|')]
        [Trait("Category", "Formatting Invariants with Delimiters")]
        public void Produces_No_Trailing_Newline(char delimiter)
        {
            var serializer = new ToonSerializer(new ToonSerializerSettings { Delimiter = delimiter });
            var obj = new { id = 123 };
            var result = serializer.Serialize(obj);
            Assert.False(result.EndsWith("\n") || result.EndsWith("\r\n"));
        }
    }
}
