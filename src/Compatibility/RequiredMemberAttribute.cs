#if !NET8_0_OR_GREATER

namespace System.Runtime.CompilerServices
{
    // C# 11 RequiredMemberAttribute
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    internal sealed class RequiredMemberAttribute : Attribute { }
}

#endif