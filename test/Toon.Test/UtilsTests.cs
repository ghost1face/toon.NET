using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Toon.Test.Models;
using Xunit;

namespace Toon.Test
{
    public class UtilsTests
    {
        public static IEnumerable<object[]> ComplexTypes
        {
            get
            {
                foreach (var objectType in ObjectTypes)
                {
                    yield return objectType;
                }

                foreach (var arrayLike in ArrayLike)
                {
                    yield return arrayLike;
                }
            }
        }

        public static IEnumerable<object[]> ArrayLike => new List<object[]>
        {
            new object[] { new int[] { 1,2,3 } },
            new object[] { new List<int> { 1,2,3 } },
            new object[] { Enumerable.Empty<int>() },
        };

        public static IEnumerable<object[]> ObjectTypes
        {
            get
            {
                yield return new object[] { new object() };
                yield return new object[] { new User() };

                foreach (var dictionaryType in DictionaryTypes)
                {
                    yield return dictionaryType;
                }
            }
        }

        public static IEnumerable<object[]> DictionaryTypes = new List<object[]>
        {
            new object[] { new Dictionary<string, object>() },
            new object[] { new ConcurrentDictionary<string, object>() }
        };

        [Theory]
        [InlineData(1)]
        [InlineData("string")]
        [InlineData(true)]
        [InlineData(12.4f)]
        [InlineData(-100)]
        [InlineData(-0)]
        [InlineData(null)]
        public void IsJsonPrimitive_ReturnsTrue_ForPrimitives(object? value)
        {
            var isPrimitive = Utils.IsJsonPrimitive(value);

            Assert.True(isPrimitive);
        }

        [Theory]
        [MemberData(nameof(ComplexTypes))]
        public void IsJsonPrimitive_ReturnsFalse_OnComplexTypes(object? value)
        {
            var isPrimitive = Utils.IsJsonPrimitive(value);

            Assert.False(isPrimitive);
        }

        [Theory]
        [MemberData(nameof(ArrayLike))]
        public void IsArrayLike_ReturnsTrue_ForArraysAndLists(object? value)
        {
            var arrayLike = Utils.IsArrayLike(value);

            Assert.True(arrayLike);
        }

        [Theory]
        [InlineData(1)]
        [InlineData("string")]
        [InlineData(true)]
        [InlineData(12.4f)]
        [InlineData(-100)]
        [InlineData(-0)]
        public void IsArrayLike_ReturnsFalse_ForPrimitiveTypes(object? value)
        {
            var arrayLike = Utils.IsArrayLike(value);

            Assert.False(arrayLike);
        }

        [Theory]
        [MemberData(nameof(ObjectTypes))]
        public void IsArrayLike_ReturnsFalse_ForObjectTypes(object? value)
        {
            var arrayLike = Utils.IsArrayLike(value);

            Assert.False(arrayLike);
        }

        [Theory]
        [MemberData(nameof(DictionaryTypes))]
        public void IsDictionary_Identifies_Dictionary(object? value)
        {
            var isDictionary = Utils.IsDictionary(value);

            Assert.True(isDictionary);
        }

        [Fact]
        public void IsArrayOfPrimitives_Identifies_Properly()
        {
            var arr = new object?[] { 1, "a", true, (object?)null };

            Assert.True(Utils.IsArrayOfPrimitives(arr));
        }

        [Fact]
        public void IsArrayOfPrimitives_DoesNotIdentify_NonPrimitiveArray()
        {
            var arr2 = new object[] { 1, new object() };

            Assert.False(Utils.IsArrayOfPrimitives(arr2));
        }

        [Fact]
        public void IsArrayOfArrays_Works()
        {
            var arr = new object[] { new int[] { 1 }, new int[] { 2 } };
            Assert.True(Utils.IsArrayOfArrays(arr));
            var arr2 = new object[] { 1, new int[] { 2 } };
            Assert.False(Utils.IsArrayOfArrays(arr2));
        }

        [Fact]
        public void IsArrayOfObjects_Works()
        {
            var arr = new object[] { new User(), new User() };
            Assert.True(Utils.IsArrayOfObjects(arr));
            var arr2 = new object[] { 1, new User() };
            Assert.False(Utils.IsArrayOfObjects(arr2));
        }
    }
}
