using System.Reflection;

namespace Unity.Interception
{
    /// <summary>
    /// This interface is implemented by the matching rule classes.
    /// A Matching rule is used to see if a particular policy should
    /// be applied to a class member.
    /// </summary>
    public interface IMatchingRule : IMatch<MethodBase, bool>
    {
    }
}
