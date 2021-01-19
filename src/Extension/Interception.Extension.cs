using System;
using Unity.Extension;
using Unity.Interception.ContainerIntegration.ObjectBuilder;
using Unity.Interception.PolicyInjection.Policies;

namespace Unity.Interception
{
    /// <summary>
    /// A Unity container extension that allows you to configure
    /// whether an object should be intercepted and which mechanism should
    /// be used to do it, and also provides a convenient set of methods for
    /// configuring injection for <see cref="RuleDrivenPolicy"/> instances.
    /// </summary>
    public partial class Interception : UnityContainerExtension
    {
        /// <summary>
        /// Initial the container with this extension's functionality.
        /// </summary>
        protected override void Initialize()
        {
            if (Context is null) throw new ArgumentNullException(nameof(Context));

            // Type pipeline
            Context.TypePipelineChain.Add(new (UnityBuildStage, BuilderStrategy)[] 
            { 
                (UnityBuildStage.InterceptType,     new TypeInterceptionStrategy()),
                (UnityBuildStage.InterceptInstance, new InstanceInterceptionStrategy())
            });

            // Factory
            Context.FactoryPipelineChain.Add(new (UnityBuildStage, BuilderStrategy)[]
            {
                (UnityBuildStage.InterceptType,     new TypeInterceptionStrategy()),
                (UnityBuildStage.InterceptInstance, new InstanceInterceptionStrategy())
            });

            // Instance pipeline
            Context.InstancePipelineChain.Add(UnityBuildStage.InterceptInstance, new InstanceInterceptionStrategy());

            Context.MappingPipelineChain.Add(new (UnityBuildStage, BuilderStrategy)[]
            {
                (UnityBuildStage.InterceptType,     new TypeInterceptionStrategy()),
                (UnityBuildStage.InterceptInstance, new InstanceInterceptionStrategy())
            });

            // Try to register default policy
            Context.Policies.CompareExchange<InjectionPolicy>(new AttributeDrivenPolicy(), null);
        }
    }
}
