using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Toon.Utils
{
    internal static class Serialization
    {
        public static bool IsJsonPrimitive(object? value)
        {
            return
                value == null
                || value is string
                || value is byte
                || value is ushort
                || value is short
                || value is uint
                || value is int
                || value is ulong
                || value is long
                || value is float
                || value is double
                || value is decimal
                || value is bool
                || value is DateTime
                || value is DateTimeOffset;
        }

        public static bool IsArrayLike(object? value)
        {
            if (value == null)
                return false;

            if (value is string)
                return false;

            if (value is IDictionary)
                return false;

            if (value is IEnumerable)
                return true;

            var type = value.GetType();

            return type.GetInterfaces()
                .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>));
        }

        public static bool IsDictionary(object? obj)
        {
            if (obj == null)
                return false;

            if (obj is IDictionary)
                return true;

            return false;
        }

        public static bool IsArrayOfPrimitives(IEnumerable<object?> objects)
        {
            return objects.All(IsJsonPrimitive);
        }

        public static bool IsArrayOfArrays(IEnumerable<object> objects)
        {
            return objects.All(IsArrayLike);
        }

        public static bool IsArrayOfObjects(IEnumerable<object> objects)
        {
            return objects.All(o => !IsJsonPrimitive(o) && !IsArrayLike(o));
        }
    }
}
