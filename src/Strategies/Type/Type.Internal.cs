using Unity.Extension;

namespace Unity.Interception.Strategies
{
    public partial class TypeInterceptionStrategy
    {
        private ITypeInterceptor? GetInterceptor<TContext>(ref TContext context)
            where TContext : IBuilderContext
            => context.FirstOrDefault<Interceptor>(o => o.IsTypeInterceptor)?
                      .GetInterceptor(Interception) as ITypeInterceptor
            ?? (context.Contract.Type.IsGenericType
            ? GetInterceptorFor<ITypeInterceptor>(context.Contract.Type, context.Contract.Name, context.TypeDefinition)
            : GetInterceptorFor<ITypeInterceptor>(context.Contract.Type, context.Contract.Name));
    }
}
