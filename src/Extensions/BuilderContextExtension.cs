using System;
using System.Linq;
using Unity.Builder;
using Unity.Injection;
using Unity.Interception.ContainerIntegration;
using Unity.Registration;

namespace Unity.Interception.Extensions
{
    internal static class BuilderContextExtension
    {

        public static TMember FindInjectedMember<TMember>(this Type type, ref BuilderContext context)
            where TMember : InterceptionMember
        {
            // Select Injected Members
            if (null == ((InternalRegistration)context.Registration).InjectionMembers)
                return null;

            return ((InternalRegistration)context.Registration)
                                                 .InjectionMembers
                                                 .OfType<TMember>()
                                                 .FirstOrDefault(m => MatchRank.NoMatch != m.MatchTo(type));
        }
    }
}
