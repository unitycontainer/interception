using System;

namespace Unity.Interception
{
    /// <summary>
    /// Attribute used to indicate that no interception should be applied to
    /// the attribute target.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Class)]
    public sealed class ApplyNoPoliciesAttribute : Attribute
    {
    }
}
