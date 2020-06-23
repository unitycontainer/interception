using System;
using Unity.Injection;

namespace Unity.Interception.ContainerIntegration
{
    /// <summary>
    /// Stores information about a an intercepted object and configures a container accordingly.
    /// </summary>
    public abstract class InterceptionMember : InjectionMember, IMatchTo<Type>
    {
        public virtual MatchRank MatchTo(Type other) => MatchRank.NoMatch;
    }
}
