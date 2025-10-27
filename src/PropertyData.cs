using System.Reflection;

namespace Toon
{
    internal class PropertyData
    {
        public required string SerializedName { get; set; }
        public required PropertyInfo PropertyInfo { get; set; }
    }
}
