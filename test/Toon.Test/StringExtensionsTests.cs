using Toon.Extensions;
using Xunit;

namespace Toon.Test
{
    public class StringExtensionsTests
    {
        [Fact]
        public void ToCamelCase_ConvertsAppropriately()
        {
            Assert.Equal("camelCase", "CamelCase".ToCamelCase());
            Assert.Equal("myString", "MyString".ToCamelCase());
            Assert.Equal("my_String", "My_String".ToCamelCase());
            Assert.Equal("testString", "testString".ToCamelCase());
            Assert.Equal("multi-separator-string", "multi-separator-string".ToCamelCase());
            Assert.Equal("multi-Separator-String", "Multi-Separator-String".ToCamelCase());
            Assert.Equal("allcaps", "ALLCAPS".ToCamelCase());
        }
    }
}
