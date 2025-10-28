using System.Threading.Tasks;
using Toon.Test.Models;
using Xunit;

namespace Toon.Test
{
    public partial class ToonSerializerTests
    {
        [Fact]
        public async Task Serialize_UserDataSet_ReturnsExpectedTokens_CamelCase()
        {
            var users = new[]
            {
                new User { Id = 1, Name = "Alice", Role = "admin" },
                new User { Id = 2, Name = "Bob", Role = "user" }
            };
            var dataSet = new UserDataSet { Users = users };

            var serializer = new ToonSerializer(new ToonSerializerSettings { PropertyNamingPolicy = ToonNamingPolicy.CamelCase });
            var result = serializer.Serialize(dataSet);

            var output = await BuildVerifier().Verify(result);

            Assert.Contains("Alice", output.Text);
            Assert.Contains("Bob", output.Text);
            Assert.Contains("admin", output.Text);
            Assert.Contains("user", output.Text);
        }

        [Fact]
        public async Task SerializeSimple_Objects_WithPrimitive_Values_CamelCase()
        {
            var serializer = new ToonSerializer(new ToonSerializerSettings { PropertyNamingPolicy = ToonNamingPolicy.CamelCase });

            var user = new User
            {
                Id = 123,
                Name = "Ada",
                Active = true
            };

            var output = serializer.Serialize(user);

            await BuildVerifier().Verify(output);
        }

        [Fact]
        public async Task SerializeNested_Objects_WithPrimitiveValues_CamelCase()
        {
            var serializer = new ToonSerializer(new ToonSerializerSettings { PropertyNamingPolicy = ToonNamingPolicy.CamelCase });

            var data = new
            {
                User = new User
                {
                    Id = 123,
                    Name = "Ada"
                }
            };

            var output = serializer.Serialize(data);

            await BuildVerifier().Verify(output);
        }

        [Fact]
        public async Task Serializes_ArrayOfObjects_Tabular_CamelCase()
        {
            var serializer = new ToonSerializer(new ToonSerializerSettings { PropertyNamingPolicy = ToonNamingPolicy.CamelCase });

            var data = new
            {
                Items = new[]
                {
                    new { SKU = "A1", Qty = 2, Price = 9.99 },
                    new { SKU = "B2", Qty = 1, Price = 14.5 }
                }
            };

            var output = serializer.Serialize(data);

            var result = await BuildVerifier().Verify(output);

            Assert.Contains("items[2]{sku,qty,price}:", result.Text);
            Assert.Contains("A1,2,9.99", result.Text);
            Assert.Contains("B2,1,14.5", result.Text);
        }

        [Fact]
        public async Task Serializes_NestedArraysWithTabularFormatting_CamelCase()
        {
            var serializer = new ToonSerializer(new ToonSerializerSettings { PropertyNamingPolicy = ToonNamingPolicy.CamelCase });

            var data = new
            {
                Items = new[]
                {
                    new
                    {
                        Users = new[]
                        {
                            new { Id = 1, Name = "Ada" },
                            new { Id = 2, Name = "Bob" }
                        },
                        Status = "active"
                    }
                }
            };

            var output = serializer.Serialize(data);

            var result = await BuildVerifier().Verify(output);

            Assert.Contains("items[1]:", result.Text);
            Assert.Contains("- users[2]{id,name}:", result.Text);
            Assert.Contains("1,Ada", result.Text);
            Assert.Contains("2,Bob", result.Text);
            Assert.Contains("status: active", result.Text);
        }
    }
}
