using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Toon.Utils
{
    internal static partial class NumericUtility
    {
#if NET8_0_OR_GREATER
        [GeneratedRegex("[Ee]\\D?(.*)")]
        private static partial Regex ParseDecimalPlacesRegex();

        private static readonly Regex parseDecimalPlacesRegex = ParseDecimalPlacesRegex();
#else
        private static readonly Regex parseDecimalPlacesRegex = new Regex("[Ee]\\D?(.*)");
#endif

        public static string ConvertToString(double value, string? originalParsedValue = null)
        {
            if (double.IsNegativeInfinity(value))
                return Constants.NullLiteral;

            if (double.IsPositiveInfinity(value))
                return Constants.NullLiteral;

            if (double.IsNaN(value))
                return Constants.NullLiteral;

            if (originalParsedValue != null)
            {
                var hasPower = originalParsedValue.IndexOf("E", StringComparison.OrdinalIgnoreCase) > -1;

                return hasPower ?
                    value.ToString($"N{int.Parse(parseDecimalPlacesRegex.Match(originalParsedValue).Groups[1].Value)}") : value.ToString("N");
            }

            return value.ToString("N");
        }

        public static string ConvertToString(float value)
        {
            if (float.IsNegativeInfinity(value))
                return Constants.NullLiteral;

            if (float.IsPositiveInfinity(value))
                return Constants.NullLiteral;

            if (float.IsNaN(value))
                return Constants.NullLiteral;

            return value.ToString();
        }

        public static string ConvertToString(decimal value)
        {
            return value.ToString("R");
        }
    }
}
