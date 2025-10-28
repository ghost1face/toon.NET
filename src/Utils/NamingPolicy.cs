using Toon.Extensions;

namespace Toon.Utils
{
    internal static class NamingPolicy
    {
        public static string NameProperty(string originalName, ToonNamingPolicy? toonNamingPolicy)
        {
            if (toonNamingPolicy == null) return originalName;

            if (toonNamingPolicy == ToonNamingPolicy.CamelCase)
                return originalName.ToCamelCase();

            return originalName;
        }
    }
}
