using System;
using System.Linq;
using System.Reflection;

namespace Toon.Extensions
{
    internal static class ObjectExtensions
    {
        public static bool IsNumber(this object value)
        {
            return value is sbyte
                    || value is byte
                    || value is short
                    || value is ushort
                    || value is int
                    || value is uint
                    || value is long
                    || value is ulong
                    || value is float
                    || value is double
                    || value is decimal;
        }

        public static PropertyData[] GetSerializableProperties(this object obj)
        {
            return obj.GetPublicProperties().Select(p => new PropertyData
            {
                SerializedName = p.Name,
                PropertyInfo = p,
            }).ToArray();
        }

        public static PropertyInfo[] GetPublicProperties(this object obj)
        {
            var type = obj.GetType();
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty);

            if (properties == null)
                return Array.Empty<PropertyInfo>();

            return properties;
        }

        public static string[] GetPropertyNames(this object obj)
        {
            var properties = obj.GetPublicProperties();

            return properties.Select(p => p.Name).ToArray();
        }

        public static bool HasProperty(this object obj, string name)
        {
            var type = obj.GetType();

            var property = type.GetProperty(name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty);

            return property != null;
        }

        public static object? GetValue(this object obj, string name)
        {
            var type = obj.GetType();

            var property = type.GetProperty(name);
            if (property == null)
                return null;

            return property.GetValue(obj, null);
        }
    }
}
