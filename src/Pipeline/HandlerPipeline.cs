

using System.Collections.Generic;

namespace Unity.Interception.PolicyInjection.Pipeline
{
    /// <summary>
    /// The HandlerPipeline class encapsulates a list of <see cref="ICallHandler"/>s
    /// and manages calling them in the proper order with the right inputs.
    /// </summary>
    public class HandlerPipeline
    {
        private readonly List<ICallHandler> _handlers;

        /// <summary>
        /// Creates a new <see cref="HandlerPipeline"/> with an empty pipeline.
        /// </summary>
        public HandlerPipeline()
        {
            _handlers = new List<ICallHandler>();
        }

        /// <summary>
        /// Creates a new <see cref="HandlerPipeline"/> with the given collection
        /// of <see cref="ICallHandler"/>s.
        /// </summary>
        /// <param name="handlers">Collection of handlers to add to the pipeline.</param>
        public HandlerPipeline(IEnumerable<ICallHandler> handlers)
        {
            _handlers = new List<ICallHandler>(handlers);
        }

        /// <summary>
        /// GetOrDefault the number of handlers in this pipeline.
        /// </summary>
        public int Count => _handlers.Count;

        /// <summary>
        /// Execute the pipeline with the given input.
        /// </summary>
        /// <param name="input">Input to the method call.</param>
        /// <param name="target">The ultimate target of the call.</param>
        /// <returns>Return value from the pipeline.</returns>
        public IMethodReturn Invoke(IMethodInvocation input, InvokeHandlerDelegate target)
        {
            if (_handlers.Count == 0)
            {
                return target(input, null);
            }

            int handlerIndex = 0;

            IMethodReturn result = _handlers[0].Invoke(input, delegate
                                      {
                                          ++handlerIndex;
                                          if (handlerIndex < _handlers.Count)
                                          {
                                              return _handlers[handlerIndex].Invoke;
                                          }
                                          return target;
                                      });
            return result;
        }
    }
}
