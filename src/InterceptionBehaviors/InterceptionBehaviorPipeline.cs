

using System;
using System.Collections.Generic;
using Unity.Interception.PolicyInjection.Pipeline;

namespace Unity.Interception.InterceptionBehaviors
{
    /// <summary>
    /// The InterceptionBehaviorPipeline class encapsulates a list of <see cref="IInterceptionBehavior"/>s
    /// and manages calling them in the proper order with the right inputs.
    /// </summary>
    public class InterceptionBehaviorPipeline
    {
        private readonly List<IInterceptionBehavior> _interceptionBehaviors;

        /// <summary>
        /// Creates a new <see cref="HandlerPipeline"/> with an empty pipeline.
        /// </summary>
        public InterceptionBehaviorPipeline()
        {
            _interceptionBehaviors = new List<IInterceptionBehavior>();
        }

        /// <summary>
        /// Creates a new <see cref="HandlerPipeline"/> with the given collection
        /// of <see cref="ICallHandler"/>s.
        /// </summary>
        /// <param name="interceptionBehaviors">Collection of interception behaviors to add to the pipeline.</param>
        public InterceptionBehaviorPipeline(IEnumerable<IInterceptionBehavior> interceptionBehaviors)
        {
            _interceptionBehaviors = new List<IInterceptionBehavior>(interceptionBehaviors ?? throw new ArgumentNullException(nameof(interceptionBehaviors)));
        }

        /// <summary>
        /// GetOrDefault the number of interceptors in this pipeline.
        /// </summary>
        public int Count => _interceptionBehaviors.Count;

        /// <summary>
        /// Execute the pipeline with the given input.
        /// </summary>
        /// <param name="input">Input to the method call.</param>
        /// <param name="target">The ultimate target of the call.</param>
        /// <returns>Return value from the pipeline.</returns>
        public IMethodReturn Invoke(IMethodInvocation input, InvokeInterceptionBehaviorDelegate target)
        {
            if (_interceptionBehaviors.Count == 0)
            {
                return target(input, null);
            }

            int interceptorIndex = 0;

            IMethodReturn result = _interceptionBehaviors[0].Invoke(input, delegate
                                      {
                                          ++interceptorIndex;
                                          if (interceptorIndex < _interceptionBehaviors.Count)
                                          {
                                              return _interceptionBehaviors[interceptorIndex].Invoke;
                                          }
                                          return target;
                                      });
            return result;
        }

        /// <summary>
        /// Adds a <see cref="IInterceptionBehavior"/> to the pipeline.
        /// </summary>
        /// <param name="interceptionBehavior">The interception behavior to add.</param>
        public void Add(IInterceptionBehavior interceptionBehavior)
        {
            _interceptionBehaviors.Add(interceptionBehavior ?? throw new ArgumentNullException(nameof(interceptionBehavior)));
        }
    }
}
