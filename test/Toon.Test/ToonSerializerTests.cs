using System.Threading.Tasks;
using Toon.Test.Models;
using Toon.Test.Tools;
using Xunit;

namespace Toon.Test
{

    public class ToonSerializerTests : BaseVerifierTest
    {
        [Fact]
        public void Serialize_UserDataSet_ReturnsExpectedTokens()
        {
            var users = new[]
            {
                new User { Id = 1, Name = "Alice", Role = "admin" },
                new User { Id = 2, Name = "Bob", Role = "user" }
            };
            var dataSet = new UserDataSet { Users = users };

            var serializer = new ToonSerializer();
            var result = serializer.Serialize(dataSet);

            Assert.Contains("Alice", result);
            Assert.Contains("Bob", result);
            Assert.Contains("admin", result);
            Assert.Contains("user", result);
        }

        [Fact]
        public async Task SerializeSimple_Objects_WithPrimitive_Values()
        {
            var serializer = new ToonSerializer();

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
        public async Task SerializeNested_Objects_WithPrimitiveValues()
        {
            var serializer = new ToonSerializer();

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
        public async Task Serializes_WithArrayLength_InBrackets()
        {
            var serializer = new ToonSerializer();

            var data = new
            {
                Tags = new[] { "admin", "ops", "dev" }
            };

            var output = serializer.Serialize(data);

            var result = await BuildVerifier().Verify(output);

            Assert.Contains("[3]", result.Text);
        }

        [Fact]
        public async Task Serializes_ArrayOfObjects_Tabular()
        {
            var serializer = new ToonSerializer();

            var data = new
            {
                Items = new[]
                {
                    new { SKU= "A1", Qty = 2, Price = 9.99 },
                    new { SKU = "B2", Qty = 1, Price = 14.5 }
                }
            };

            var output = serializer.Serialize(data);

            var result = await BuildVerifier().Verify(output);

            Assert.Contains("Items[2]{SKU,Qty,Price}:", result.Text);
            Assert.Contains("A1,2,9.99", result.Text);
            Assert.Contains("B2,1,14.5", result.Text);
        }

        [Fact]
        public async Task Serializes_NestedArraysWithTabularFormatting()
        {
            var serializer = new ToonSerializer();

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

            Assert.Contains("Items[1]:", result.Text);
            Assert.Contains("- Users[2]{Id,Name}:", result.Text);
            Assert.Contains("1,Ada", result.Text);
            Assert.Contains("2,Bob", result.Text);
            Assert.Contains("Status: active", result.Text);
        }
    }
}