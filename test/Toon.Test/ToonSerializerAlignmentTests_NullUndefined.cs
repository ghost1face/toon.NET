using System;
using Xunit;

namespace Toon.Test
{
    public partial class ToonSerializerAlignmentTests
    {
        [Fact]
        [Trait("Category", "non-JSON-serializable values")]
        public void Converts_BigInt_To_String()
        {
            var serializer = new ToonSerializer();

            Assert.Equal("123", serializer.Serialize(123L));
        }

        [Fact]
        [Trait("Category", "non-JSON-serializable values")]
        public void Converts_Date_To_ISO_String()
        {
            var date = new DateTime(2025, 01, 01, 0, 0, 0, 0, 0);
            var serializer = new ToonSerializer();

            Assert.Equal("\"2025-01-01T00:00:00Z\"", serializer.Serialize(date));
            Assert.Equal("created: \"2025-01-01T00:00:00Z\"", serializer.Serialize(new { created = date }));
        }

        [Fact]
        [Trait("Category", "non-JSON-serializable values")]
        public void Converts_Undefined_To_Null()
        {
            var serializer = new ToonSerializer();
            object undefined = null; // C# has no undefined, so use null

            Assert.Equal("null", serializer.Serialize(undefined));
            Assert.Equal("value: null", serializer.Serialize(new { value = (object)null }));
        }

        [Fact]
        [Trait("Category", "non-JSON-serializable values")]
        public void Converts_NonFiniteNumbers_To_Null()
        {
            var serializer = new ToonSerializer();

            Assert.Equal("null", serializer.Serialize(double.PositiveInfinity));
            Assert.Equal("null", serializer.Serialize(double.NegativeInfinity));
            Assert.Equal("null", serializer.Serialize(double.NaN));
        }
    }
}
