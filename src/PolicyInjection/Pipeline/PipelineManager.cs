﻿

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using Unity.Interception.Interceptors;
using Unity.Interception.Utilities;

namespace Unity.Interception.PolicyInjection.Pipeline
{
    /// <summary>
    /// A collection of <see cref="HandlerPipeline"/> objects, indexed
    /// by <see cref="MethodBase"/>. Returns an empty pipeline if a
    /// MethodBase is requested that isn't in the dictionary.
    /// </summary>
    public class PipelineManager
    {
        private readonly Dictionary<HandlerPipelineKey, HandlerPipeline> _pipelines =
            new Dictionary<HandlerPipelineKey, HandlerPipeline>();

        private static readonly HandlerPipeline EmptyPipeline = new HandlerPipeline();

        private static readonly ConcurrentDictionary<HandlerPipelineKey, MethodInfo> BaseMethodDefinitions =
            new ConcurrentDictionary<HandlerPipelineKey, MethodInfo>();

        /// <summary>
        /// Retrieve the pipeline associated with the requested <paramref name="method"/>.
        /// </summary>
        /// <param name="method">The method for which the pipeline is being requested.</param>
        /// <returns>The handler pipeline for the given method. If no pipeline has
        /// been set, returns a new empty pipeline.</returns>
        public HandlerPipeline GetPipeline(MethodBase method)
        {
            HandlerPipelineKey key = HandlerPipelineKey.ForMethod(method);
            HandlerPipeline pipeline = EmptyPipeline;
            if (_pipelines.ContainsKey(key))
            {
                pipeline = _pipelines[key];
            }
            return pipeline;
        }

        /// <summary>
        /// Set a new pipeline for a method.
        /// </summary>
        /// <param name="method">The method on which the pipeline should be set.</param>
        /// <param name="pipeline">The new pipeline.</param>
        public void SetPipeline(MethodBase method, HandlerPipeline pipeline)
        {
            HandlerPipelineKey key = HandlerPipelineKey.ForMethod(method);
            _pipelines[key] = pipeline;
        }

        /// <summary>
        /// GetOrDefault the pipeline for the given method, creating it if necessary.
        /// </summary>
        /// <param name="method">Method to retrieve the pipeline for.</param>
        /// <param name="handlers">Handlers to initialize the pipeline with</param>
        /// <returns>True if the pipeline has any handlers in it, false if not.</returns>
        public bool InitializePipeline(MethodImplementationInfo method, IEnumerable<ICallHandler> handlers)
        {
            Guard.ArgumentNotNull(method, "method");

            var pipeline = CreatePipeline(method.ImplementationMethodInfo, handlers);
            if (method.InterfaceMethodInfo != null)
            {
                _pipelines[HandlerPipelineKey.ForMethod(method.InterfaceMethodInfo)] = pipeline;
            }

            return pipeline.Count > 0;
        }

        private HandlerPipeline CreatePipeline(MethodInfo method, IEnumerable<ICallHandler> handlers)
        {
            HandlerPipelineKey key = HandlerPipelineKey.ForMethod(method);
            if (_pipelines.ContainsKey(key))
            {
                return _pipelines[key];
            }

            var baseMethodDefinition = BaseMethodDefinitions.GetOrAdd(key, k => method.GetBaseDefinition());

            if (baseMethodDefinition == method)
            {
                _pipelines[key] = new HandlerPipeline(handlers);
                return _pipelines[key];
            }

            var basePipeline = CreatePipeline(baseMethodDefinition, handlers);
            _pipelines[key] = basePipeline;
            return basePipeline;
        }
    }
}
