// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using Unity.Interception.Interceptors.TypeInterceptors.VirtualMethodInterception.InterceptingClassGeneration;
using Unity.Interception.Properties;
using Unity.Interception.Utilities;

namespace Unity.Interception.Interceptors.InstanceInterceptors.InterfaceInterception
{
    /// <summary>
    /// A class used to generate proxy classes for doing interception on
    /// interfaces.
    /// </summary>
    public partial class InterfaceInterceptorClassGenerator
    {
        private static readonly AssemblyBuilder AssemblyBuilder;
        private readonly Type _typeToIntercept;
        private readonly IEnumerable<Type> _additionalInterfaces;
        private GenericParameterMapper _mainInterfaceMapper;

        private FieldBuilder _proxyInterceptionPipelineField;
        private FieldBuilder _targetField;
        private FieldBuilder _typeToProxyField;
        private TypeBuilder _typeBuilder;

        static InterfaceInterceptorClassGenerator()
        {
            byte[] pair;

            using (MemoryStream ms = new MemoryStream())
            {
                typeof(InterfaceInterceptorClassGenerator)
                    .Assembly
                    .GetManifestResourceStream("Unity.Interception.package.snk")
                    .CopyTo(ms);

                pair = ms.ToArray();
            }

            AssemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(
                new AssemblyName("Unity_ILEmit_InterfaceProxies") { KeyPair = new StrongNameKeyPair(pair) },
#if DEBUG_SAVE_GENERATED_ASSEMBLY
                AssemblyBuilderAccess.RunAndSave);
#else
                AssemblyBuilderAccess.Run);
#endif
        }

        /// <summary>
        /// Create an instance of <see cref="InterfaceInterceptorClassGenerator"/> that
        /// can construct an intercepting proxy for the given interface.
        /// </summary>
        /// <param name="typeToIntercept">Type of the interface to intercept.</param>
        /// <param name="additionalInterfaces">Additional interfaces the proxy must implement.</param>
        public InterfaceInterceptorClassGenerator(Type typeToIntercept, IEnumerable<Type> additionalInterfaces)
        {
            var interfaces = additionalInterfaces as Type[] ?? additionalInterfaces.ToArray();
            CheckAdditionalInterfaces(interfaces);

            _typeToIntercept = typeToIntercept;
            _additionalInterfaces = interfaces;
            CreateTypeBuilder();
        }

        private static void CheckAdditionalInterfaces(IEnumerable<Type> additionalInterfaces)
        {
            if (additionalInterfaces == null)
            {
                throw new ArgumentNullException(nameof(additionalInterfaces));
            }

            foreach (var type in additionalInterfaces)
            {
                if (type == null)
                {
                    throw new ArgumentException(
                        Resources.ExceptionContainsNullElement,
                        nameof(additionalInterfaces));
                }
                if (!type.IsInterface)
                {
                    throw new ArgumentException(
                        string.Format(CultureInfo.CurrentCulture, Resources.ExceptionTypeIsNotInterface, type.Name),
                        nameof(additionalInterfaces));
                }
                if (type.IsGenericTypeDefinition)
                {
                    throw new ArgumentException(
                        string.Format(CultureInfo.CurrentCulture, Resources.ExceptionTypeIsOpenGeneric, type.Name),
                        nameof(additionalInterfaces));
                }
            }
        }

        /// <summary>
        /// Create the type to proxy the requested interface
        /// </summary>
        /// <returns></returns>
        public Type CreateProxyType()
        {
            HashSet<Type> implementedInterfaces = new HashSet<Type>();

            int memberCount =
                new InterfaceImplementation(
                    _typeBuilder,
                    _typeToIntercept,
                    _mainInterfaceMapper,
                    _proxyInterceptionPipelineField,
                    false,
                    _targetField)
                    .Implement(implementedInterfaces, 0);

            foreach (var @interface in _additionalInterfaces)
            {
                memberCount =
                    new InterfaceImplementation(
                        _typeBuilder,
                        @interface,
                        _proxyInterceptionPipelineField,
                        true)
                        .Implement(implementedInterfaces, memberCount);
            }

            AddConstructor();

            Type result = _typeBuilder.CreateTypeInfo().AsType();
#if DEBUG_SAVE_GENERATED_ASSEMBLY
            assemblyBuilder.Save("Unity_ILEmit_InterfaceProxies.dll");
#endif
            return result;
        }

        private void AddConstructor()
        {
            Type[] paramTypes = Sequence.Collect(_typeToIntercept, typeof(Type)).ToArray();

            ConstructorBuilder ctorBuilder = _typeBuilder.DefineConstructor(
                MethodAttributes.Public,
                CallingConventions.HasThis,
                paramTypes);

            ctorBuilder.DefineParameter(1, ParameterAttributes.None, "target");
            ctorBuilder.DefineParameter(2, ParameterAttributes.None, "typeToProxy");
            ILGenerator il = ctorBuilder.GetILGenerator();

            // Call base class constructor

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Call, ObjectMethods.Constructor);

            // Initialize pipeline field
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Newobj, InterceptionBehaviorPipelineMethods.Constructor);
            il.Emit(OpCodes.Stfld, _proxyInterceptionPipelineField);

            // Initialize the target field
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Stfld, _targetField);

            // Initialize the typeToProxy field
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Stfld, _typeToProxyField);

            il.Emit(OpCodes.Ret);
        }

        private void CreateTypeBuilder()
        {
            TypeAttributes newAttributes = TypeAttributes.Public | TypeAttributes.Class;

            ModuleBuilder moduleBuilder = GetModuleBuilder();
            _typeBuilder = moduleBuilder.DefineType(CreateTypeName(), newAttributes);

            _mainInterfaceMapper = DefineGenericArguments();

            _proxyInterceptionPipelineField = InterceptingProxyImplementor.ImplementIInterceptingProxy(_typeBuilder);
            _targetField = _typeBuilder.DefineField("target", _typeToIntercept, FieldAttributes.Private);
            _typeToProxyField = _typeBuilder.DefineField("typeToProxy", typeof(Type), FieldAttributes.Private);
        }

        private string CreateTypeName()
        {
            return "DynamicModule.ns.Wrapped_" + _typeToIntercept.Name + "_" + Guid.NewGuid().ToString("N");
        }

        private GenericParameterMapper DefineGenericArguments()
        {
            if (!_typeToIntercept.IsGenericType)
            {
                return GenericParameterMapper.DefaultMapper;
            }

            Type[] genericArguments = _typeToIntercept.GetGenericArguments();

            GenericTypeParameterBuilder[] genericTypes = _typeBuilder.DefineGenericParameters(
                genericArguments.Select(t => t.Name).ToArray());

            for (int i = 0; i < genericArguments.Length; ++i)
            {
                genericTypes[i].SetGenericParameterAttributes(
                    genericArguments[i].GenericParameterAttributes & ~GenericParameterAttributes.VarianceMask);
                var interfaceConstraints = new List<Type>();
                foreach (Type constraint in genericArguments[i].GetGenericParameterConstraints())
                {
                    if (constraint.IsClass)
                    {
                        genericTypes[i].SetBaseTypeConstraint(constraint);
                    }
                    else
                    {
                        interfaceConstraints.Add(constraint);
                    }
                }
                if (interfaceConstraints.Count > 0)
                {
                    genericTypes[i].SetInterfaceConstraints(interfaceConstraints.ToArray());
                }
            }

            return new GenericParameterMapper(genericArguments, genericTypes.Cast<Type>().ToArray());
        }
    }
}
