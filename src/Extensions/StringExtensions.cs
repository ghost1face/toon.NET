namespace Toon.Extensions
{
    internal static class StringExtensions
    {
        public static string ToCamelCase(this string value)
        {
            // ref: 
            // https://github.com/dotnet/runtime/blob/main/src/libraries/System.Text.Json/Common/JsonCamelCaseNamingPolicy.cs#L6
            if (string.IsNullOrWhiteSpace(value) || !char.IsUpper(value[0]))
                return value;

            var chars = value.ToCharArray();

            for (int i = 0; i < chars.Length; i++)
            {
                if (i == 1 && !char.IsUpper(chars[i]))
                {
                    break;
                }

                bool hasNext = (i + 1 < chars.Length);

                // Stop when next char is already lowercase.
                if (i > 0 && hasNext && !char.IsUpper(chars[i + 1]))
                {
                    // If the next char is a space, lowercase current char before exiting.
                    if (chars[i + 1] == ' ')
                    {
                        chars[i] = char.ToLowerInvariant(chars[i]);
                    }

                    break;
                }

                chars[i] = char.ToLowerInvariant(chars[i]);
            }

            return new string(chars);
        }
    }
}
