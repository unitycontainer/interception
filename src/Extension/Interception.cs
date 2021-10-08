using System;
using System.Diagnostics;
using Unity.Extension;
using Unity.Interception.Strategies;
using Unity.Storage;

namespace Unity.Interception
{
    /// <summary>
    /// A Unity container extension that allows you to configure
    /// whether an object should be intercepted and which mechanism should
    /// be used to do it, and also provides a convenient set of methods for
    /// configuring injection for <see cref="RuleDrivenPolicy"/> instances.
    /// </summary>
    public partial class Interception : UnityContainerExtension,  
                                        IUnityContainerExtensionConfigurator
    {
        #region Fields


        protected int Count;

        [CLSCompliant(false)]
        protected Entry[] Data;

        [DebuggerBrowsable(DebuggerBrowsableState.Never), CLSCompliant(false)]
        protected Metadata[] Meta;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected readonly object SyncRoot = new object();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected int Prime = 1;


        #endregion


        #region Constructors

        public Interception()
        {
            Data = new Entry[Storage.Prime.Numbers[Prime]];
            Meta = new Metadata[Storage.Prime.Numbers[++Prime]];
        }

        #endregion


        /// <summary>
        /// Initial the container with this extension's functionality.
        /// </summary>
        protected override void Initialize()
        {
            if (Context is null) throw new ArgumentNullException(nameof(Context));

            var type     = new TypeInterceptionStrategy(this);
            var instance = new InstanceInterceptionStrategy(this);

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
