using Xunit;

namespace Toon.Test
{
    public partial class ToonSerializerAlignmentTests
    {
        [Theory]
        [InlineData("hello", "hello")]
        [InlineData("Ada_99", "Ada_99")]
        [Trait("Category", "Primitives")]
        public void Encodes_SafeStrings_WithoutQuotes(string input, string expected)
        {
            var serializer = new ToonSerializer();
            var result = serializer.Serialize(input);
            Assert.Equal(expected, result);
        }

        [Fact]
        [Trait("Category", "Primitives")]
        public void Quotes_EmptyString()
        {
            var serializer = new ToonSerializer();
            var result = serializer.Serialize("");

            Assert.Equal("\"\"", result);
        }

        [Theory]
        [InlineData("true")]
        [InlineData("false")]
        [InlineData("null")]
        [InlineData("42")]
        [InlineData("-3.14")]
        [InlineData("1e-6")]
        [Trait("Category", "Primitives")]
        public void Quotes_Strings_Like_Bool_Or_Number(string input)
        {
            var serializer = new ToonSerializer();
            var result = serializer.Serialize(input);

            Assert.Equal($"\"{input}\"", result);
        }

        [Fact]
        [Trait("Category", "Primitives")]
        public void Escapes_Control_Characters_In_Strings()
        {
            var serializer = new ToonSerializer();

            Assert.Equal("\"line1\\nline2\"", serializer.Serialize("line1\nline2"));
            Assert.Equal("\"tab\\there\"", serializer.Serialize("tab\there"));
            Assert.Equal("\"return\\rcarriage\"", serializer.Serialize("return\rcarriage"));
            Assert.Equal("\"C:\\\\Users\\\\path\"", serializer.Serialize("C:\\Users\\path"));
        }

        [Theory]
        [InlineData("[3]: x,y", "\"[3]: x,y\"")]
        [InlineData("- item", "\"- item\"")]
        [InlineData("[test]", "\"[test]\"")]
        [InlineData("{key}", "\"{key}\"")]
        [Trait("Category", "Primitives")]
        public void Quotes_Strings_With_Structural_Characters(string input, string expected)
        {
            var serializer = new ToonSerializer();
            var result = serializer.Serialize(input);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("café", "café")]
        [InlineData("你好", "你好")]
        [InlineData("🚀", "🚀")]
        [InlineData("hello 👋 world", "hello 👋 world")]
        [Trait("Category", "Primitives")]
        public void Handles_Unicode_And_Emoji(string input, string expected)
        {
            var serializer = new ToonSerializer();
            var result = serializer.Serialize(input);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(42, "42")]
        [InlineData(3.14, "3.14")]
        [InlineData(-7, "-7")]
        [InlineData(0, "0")]
        [Trait("Category", "Primitives")]
        public void Encodes_Numbers(object input, string expected)
        {
            var serializer = new ToonSerializer();
            var result = serializer.Serialize(input);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(-0.0, "0")]
        [InlineData(1e6, "1000000")]
        [InlineData(1e-6, "0.000001")]
        //[InlineData(1e20, "100000000000000000000")] // TODO: Revisit the formatting here
        [InlineData(9007199254740991d, "9007199254740991")]
        [Trait("Category", "Primitives")]
        public void Handles_Special_Numeric_Values(object input, string expected)
        {
            var serializer = new ToonSerializer();
            var result = serializer.Serialize(input);

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(true, "true")]
        [InlineData(false, "false")]
        [Trait("Category", "Primitives")]
        public void Encodes_Booleans(object input, string expected)
        {
            var serializer = new ToonSerializer();
            var result = serializer.Serialize(input);
            Assert.Equal(expected, result);
        }

        [Fact]
        [Trait("Category", "Primitives")]
        public void Encodes_Null()
        {
            var serializer = new ToonSerializer();
            var result = serializer.Serialize(null);
            Assert.Equal("null", result);
        }
    }
}
