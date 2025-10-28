#if !NET8_0_OR_GREATER

namespace System.Runtime.CompilerServices
{
    // C# 11 CompilerFeatureRequiredAttribute
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
    internal sealed class CompilerFeatureRequiredAttribute : Attribute
    {
        public CompilerFeatureRequiredAttribute(string featureName) => FeatureName = featureName;
        public string FeatureName { get; }
    }
}

#endif
