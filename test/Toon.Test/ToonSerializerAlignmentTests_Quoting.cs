using Xunit;

namespace Toon.Test
{
    public partial class ToonSerializerAlignmentTests
    {
        [Theory]
        [InlineData("true")]
        [InlineData("42")]
        [InlineData("-3.14")]
        [Trait("Category", "Delimiter-independent quoting rules")]
        public void Preserves_Ambiguity_Quoting_Regardless_Of_Delimiter(string input)
        {
            var serializerPipe = new ToonSerializer(new ToonSerializerSettings { Delimiter = '|' });
            var serializerTab = new ToonSerializer(new ToonSerializerSettings { Delimiter = '\t' });
            var obj = new { items = new[] { input } };

            Assert.Contains($"\"{input}\"", serializerPipe.Serialize(obj));
            Assert.Contains($"\"{input}\"", serializerTab.Serialize(obj));
        }

        [Fact]
        [Trait("Category", "Delimiter-independent quoting rules")]
        public void Preserves_Structural_Quoting_Regardless_Of_Delimiter()
        {
            var obj = new { items = new[] { "[5]", "{key}", "- item" } };

            var serializerPipe = new ToonSerializer(new ToonSerializerSettings { Delimiter = '|' });
            var serializerTab = new ToonSerializer(new ToonSerializerSettings { Delimiter = '\t' });

            Assert.Equal("items[3|]: \"[5]\"|\"{key}\"|\"- item\"", serializerPipe.Serialize(obj));
            Assert.Equal("items[3\t]: \"[5]\"\t\"{key}\"\t\"- item\"", serializerTab.Serialize(obj));
        }

        // TODO: Support Dictionary types and add the following tests
        // https://github.com/johannschopplich/toon/blob/main/test/index.test.ts#L691-L705
    }
}
