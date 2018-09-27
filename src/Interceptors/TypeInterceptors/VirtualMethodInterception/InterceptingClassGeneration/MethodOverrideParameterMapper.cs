

using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Unity.Interception.Interceptors.TypeInterceptors.VirtualMethodInterception.InterceptingClassGeneration
{
    /// <summary>
    /// This class handles parameter type mapping. When we generate
    /// a generic method, we need to make sure our parameter type
    /// objects line up with the generic parameters on the generated
    /// method, not on the one we're overriding. 
    /// </summary>
    internal class MethodOverrideParameterMapper
    {
        private readonly MethodInfo _methodToOverride;
        private GenericParameterMapper _genericParameterMapper;

        public MethodOverrideParameterMapper(MethodInfo methodToOverride)
        {
            _methodToOverride = methodToOverride;
        }

        public void SetupParameters(MethodBuilder methodBuilder, GenericParameterMapper parentMapper)
        {
            if (_methodToOverride.IsGenericMethod)
            {
                var genericArguments = _methodToOverride.GetGenericArguments();
                var names = genericArguments.Select(t => t.Name).ToArray();
                var builders = methodBuilder.DefineGenericParameters(names);
                for (int i = 0; i < genericArguments.Length; ++i)
                {
                    builders[i].SetGenericParameterAttributes(genericArguments[i].GenericParameterAttributes);

                    var constraintTypes =
                        genericArguments[i]
                            .GetGenericParameterConstraints()
                            .Select(ct => parentMapper.Map(ct))
                            .ToArray();

                    var interfaceConstraints = constraintTypes.Where(t => t.IsInterface).ToArray();
                    Type baseConstraint = constraintTypes.Where(t => !t.IsInterface).FirstOrDefault();
                    if (baseConstraint != null)
                    {
                        builders[i].SetBaseTypeConstraint(baseConstraint);
                    }
                    if (interfaceConstraints.Length > 0)
                    {
                        builders[i].SetInterfaceConstraints(interfaceConstraints);
                    }
                }

                _genericParameterMapper =
                    new GenericParameterMapper(genericArguments, builders.Cast<Type>().ToArray(), parentMapper);
            }
            else
            {
                _genericParameterMapper = parentMapper;
            }
        }

        public Type GetParameterType(Type originalParameterType)
        {
            return _genericParameterMapper.Map(originalParameterType);
        }

        public Type GetElementType(Type originalParameterType)
        {
            return GetParameterType(originalParameterType).GetElementType();
        }

        public Type GetReturnType()
        {
            return GetParameterType(_methodToOverride.ReturnType);
        }

        public Type[] GenericMethodParameters => _genericParameterMapper.GetGeneratedParameters();
    }
}
