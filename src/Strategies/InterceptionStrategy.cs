using System;
using System.Collections.Generic;
using Unity.Extension;

namespace Unity.Interception.Strategies
{
    public partial class InterceptionStrategy : BuilderStrategy
    {
        #region Fields

        protected readonly Interception Interception;

        #endregion


        #region Constructors

        public InterceptionStrategy(Interception extension) 
            => Interception = extension;

        #endregion


        #region Implementation

        protected TInterceptor? GetInterceptorFor<TInterceptor>(Type type, string? name, Type definition)
            where TInterceptor : IInterceptor
            => Interception.Get<TInterceptor>(type, name)       ?? // Exact match (type and name)
               Interception.Get<TInterceptor>(definition, name) ?? // Named generic 
               Interception.Get<TInterceptor>(type)             ?? // Default type
               Interception.Get<TInterceptor>(definition);         // Default generic type

        protected TInterceptor? GetInterceptorFor<TInterceptor>(Type type, string? name)
            where TInterceptor : IInterceptor
            => Interception.Get<TInterceptor>(type, name)       ??  // Exact match (type and name)
               Interception.Get<TInterceptor>(type);                // Default type

        protected IEnumerable<IInterceptionBehavior> GetInjectedBehaviors(InterceptionBehavior? behavior, ResolverOverride[] overrides)
        {
            while (behavior is not null)
            {
                yield return behavior.GetBehavior(Interception);
                
                behavior = behavior.Next<InterceptionBehavior>();
            }
        }


        protected IEnumerable<IInterceptionBehavior> GetPolicyBehaviors(IInterceptor interceptor)
        {
            //var instances = context.OfType<InterceptionBehavior>().ToArray();
            //var behaviors = new IInterceptionBehavior[instances.Length];

            //var interceptionRequest = new CurrentInterceptionRequest(interceptor, registration, implementation);
            //var @override = new DependencyOverride<CurrentInterceptionRequest>(interceptionRequest);

            //for (var i = 0; i < instances.Length; i++)
            //{
            //    var instance = instances[i];
            //    behaviors[i] = instance._explicitBehavior is not null
            //        ? instance._explicitBehavior
            //        : (IInterceptionBehavior)context.Container.Resolve(instance._type, instance._name, @override)!;
            //}

            //return behaviors;

            yield break;
        }

        protected IEnumerable<IInterceptionBehavior> GetBehaviors(IInterceptor interceptor)
        {
            yield break;
        }

        #endregion
    }
}
