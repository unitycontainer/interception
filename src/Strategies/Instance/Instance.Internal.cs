using Unity.Extension;

namespace Unity.Interception.Strategies
{
    public partial class InstanceInterceptionStrategy
    {
        private IInstanceInterceptor? GetInterceptor<TContext>(ref TContext context) 
            where TContext : IBuilderContext 
            => context.FirstOrDefault<Interceptor>(o => o.IsInstanceInterceptor)?
                      .GetInterceptor(Interception) as IInstanceInterceptor
            ?? (context.Contract.Type.IsGenericType
            ? GetInterceptorFor<IInstanceInterceptor>(context.Contract.Type, context.Contract.Name, context.TypeDefinition)
            : GetInterceptorFor<IInstanceInterceptor>(context.Contract.Type, context.Contract.Name));
    }
}
