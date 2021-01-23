using System;
using Unity.Extension;
using Unity.Interception.Strategies;

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
        #region Fields

        private readonly Schemes _interceptors;

        #endregion


        #region Constructors

        public Interception() => _interceptors = new Schemes();

        #endregion


        /// <summary>
        /// Initial the container with this extension's functionality.
        /// </summary>
        protected override void Initialize()
        {
            if (Context is null) throw new ArgumentNullException(nameof(Context));

            Context.Policies.Set(typeof(Schemes), _interceptors);

            var type     = new TypeInterceptionStrategy(_interceptors);
            var instance = new InstanceInterceptionStrategy(_interceptors);

            // Type pipeline
            Context.TypePipelineChain.Add(new (UnityBuildStage, BuilderStrategy)[] 
            { 
                (UnityBuildStage.InterceptType,     type),
                (UnityBuildStage.InterceptInstance, instance)
            });

            // Factory
            Context.FactoryPipelineChain.Add(new (UnityBuildStage, BuilderStrategy)[]
            {
                (UnityBuildStage.InterceptType,     type),
                (UnityBuildStage.InterceptInstance, instance)
            });

            // Instance pipeline
            Context.InstancePipelineChain.Add(UnityBuildStage.InterceptInstance, instance);

            // Mapping pipeline
            Context.MappingPipelineChain.Add(new (UnityBuildStage, BuilderStrategy)[]
            {
                (UnityBuildStage.InterceptType,     type),
                (UnityBuildStage.InterceptInstance, instance)
            });

            Context.Container.RegisterInstance<InjectionPolicy>(typeof(AttributeDrivenPolicy).AssemblyQualifiedName,
                                                                new AttributeDrivenPolicy());
        }
    }
}
