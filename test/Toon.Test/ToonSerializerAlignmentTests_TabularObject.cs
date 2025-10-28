using System;
using System.Collections.Generic;
using Xunit;
using Toon;

namespace Toon.Test
{
    public partial class ToonSerializerAlignmentTests
    {
        [Fact]
        public void Handles_Tabular_Array_With_Null_Values()
        {
            var serializer = new ToonSerializer();
            var obj = new { items = new[] { new { id = 1, value = (string)null }, new { id = 2, value = "test" } } };
            var result = serializer.Serialize(obj);
            Assert.Contains("items[2]{id,value}:", result.Replace("\r\n", "\n"));
            Assert.Contains("1,null", result.Replace("\r\n", "\n"));
            Assert.Contains("2,test", result.Replace("\r\n", "\n"));
        }

        [Fact]
        public void Quotes_Strings_With_Delimiters_In_Tabular_Rows()
        {
            var serializer = new ToonSerializer();
            var obj = new { items = new[] { new { sku = "A,1", desc = "cool", qty = 2 }, new { sku = "B2", desc = "wip: test", qty = 1 } } };
            var result = serializer.Serialize(obj);
            Assert.Contains("\"A,1\",cool,2", result.Replace("\r\n", "\n"));
            Assert.Contains("B2,\"wip: test\",1", result.Replace("\r\n", "\n"));
        }

        [Fact]
        public void Quotes_Ambiguous_Strings_In_Tabular_Rows()
        {
            var serializer = new ToonSerializer();
            var obj = new { items = new[] { new { id = 1, status = "true" }, new { id = 2, status = "false" } } };
            var result = serializer.Serialize(obj);
            Assert.Contains("1,\"true\"", result.Replace("\r\n", "\n"));
            Assert.Contains("2,\"false\"", result.Replace("\r\n", "\n"));
        }

        //// TODO: Add support for dictionary types
        //[Fact]
        //public void Handles_Tabular_Arrays_With_Keys_Needing_Quotes()
        //{
        //    var serializer = new ToonSerializer();
        //    var obj = new { items = new[] { new Dictionary<string, object> { ["order:id"] = 1, ["full name"] = "Ada" }, new Dictionary<string, object> { ["order:id"] = 2, ["full name"] = "Bob" } } };
            
        //    var result = serializer.Serialize(obj);

        //    Assert.Contains("items[2]{\"order:id\",\"full name\"}:", result.Replace("\r\n", "\n"));
        //    Assert.Contains("1,Ada", result.Replace("\r\n", "\n"));
        //    Assert.Contains("2,Bob", result.Replace("\r\n", "\n"));
        //}
    }
}
