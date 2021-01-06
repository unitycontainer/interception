using System;
using Unity.Extension;
using Unity.Interception.ContainerIntegration;

namespace Unity.Interception.Extensions
{
    internal static class BuilderContextExtension
    {
        public static TMember FindInjectedMember<TContext, TMember>(this Type type, ref TContext context)
            where TMember  : InterceptionMember
            where TContext : IBuilderContext
        {
            // Select Injected Members
            if (null == context.Registration?.Other)
                return null;

            for (var member = context.Registration?.Other; null != member; member = member.Next)
            {
                if (member is TMember candidate && MatchRank.NoMatch != candidate.Match(type))
                    return candidate;
            }

            return null;
        }
    }
}
