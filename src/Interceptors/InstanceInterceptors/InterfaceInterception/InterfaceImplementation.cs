

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Unity.Interception.Interceptors.InstanceInterceptors.InterfaceInterception
{
    internal class InterfaceImplementation
    {
        private readonly TypeBuilder _typeBuilder;
        private readonly Type _interface;
        private readonly Type _targetInterface;
        private readonly GenericParameterMapper _genericParameterMapper;
        private readonly FieldBuilder _proxyInterceptionPipelineField;
        private readonly bool _explicitImplementation;
        private readonly FieldBuilder _targetField;

        public InterfaceImplementation(
            TypeBuilder typeBuilder,
            Type @interface,
            FieldBuilder proxyInterceptionPipelineField,
            bool explicitImplementation)
            : this(typeBuilder, @interface, proxyInterceptionPipelineField, explicitImplementation, null)
        { }

        public InterfaceImplementation(
            TypeBuilder typeBuilder,
            Type @interface,
            FieldBuilder proxyInterceptionPipelineField,
            bool explicitImplementation,
            FieldBuilder targetField)
            : this(typeBuilder, @interface, GenericParameterMapper.DefaultMapper, proxyInterceptionPipelineField, explicitImplementation, targetField)
        { }

        public InterfaceImplementation(
            TypeBuilder typeBuilder,
            Type @interface,
            GenericParameterMapper genericParameterMapper,
            FieldBuilder proxyInterceptionPipelineField,
            bool explicitImplementation,
            FieldBuilder targetField)
        {
            _typeBuilder = typeBuilder;
            _interface = @interface;
            _genericParameterMapper = genericParameterMapper;
            _proxyInterceptionPipelineField = proxyInterceptionPipelineField;
            _explicitImplementation = explicitImplementation;
            _targetField = targetField;

            if (@interface.IsGenericType)
            {
                // when the @interface is generic we need to get references to its methods though it
                // in this case, the targetInterface is a constructed version using the generic type parameters
                // from the generated type generate type
                var definition = @interface.GetGenericTypeDefinition();
                Debug.Assert(definition != null, nameof(definition) + " != null");
                var mappedParameters = definition.GetGenericArguments().Select(t => genericParameterMapper.Map(t)).ToArray();
                _targetInterface = definition.MakeGenericType(mappedParameters);
            }
            else
            {
                _targetInterface = @interface;
            }
        }

        public int Implement(HashSet<Type> implementedInterfaces, int memberCount)
        {
            if (implementedInterfaces.Contains(_interface))
            {
                return memberCount;
            }

            implementedInterfaces.Add(_interface);

            _typeBuilder.AddInterfaceImplementation(_interface);

            foreach (MethodInfo method in MethodsToIntercept())
            {
                OverrideMethod(method, memberCount++);
            }

            foreach (PropertyInfo property in PropertiesToIntercept())
            {
                OverrideProperty(property, memberCount++);
            }

            foreach (EventInfo @event in EventsToIntercept())
            {
                OverrideEvent(@event, memberCount++);
            }

            foreach (var extendedInterface in _interface.GetInterfaces())
            {
                memberCount =
                    new InterfaceImplementation(
                        _typeBuilder,
                        extendedInterface,
                        new GenericParameterMapper(extendedInterface, _genericParameterMapper),
                        _proxyInterceptionPipelineField,
                        _explicitImplementation,
                        _targetField)
                        .Implement(implementedInterfaces, memberCount);
            }

            return memberCount;
        }

        private IEnumerable<MethodInfo> MethodsToIntercept()
        {
            foreach (MethodInfo method in
                _interface.GetMethods(
                    BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (!method.IsSpecialName)
                {
                    yield return method;
                }
            }
        }

        private void OverrideMethod(MethodInfo method, int methodNum)
        {
            new InterfaceMethodOverride(
                _typeBuilder,
                _proxyInterceptionPipelineField,
                _targetField,
                method,
                _targetInterface,
                _genericParameterMapper,
                _explicitImplementation,
                methodNum)
                .AddMethod();
        }

        private IEnumerable<PropertyInfo> PropertiesToIntercept()
        {
            return _interface.GetProperties(
                BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        }

        private void OverrideProperty(PropertyInfo property, int count)
        {
            MethodBuilder getMethod = OverridePropertyMethod(property.GetGetMethod(), count);
            MethodBuilder setMethod = OverridePropertyMethod(property.GetSetMethod(), count);
            AddPropertyDefinition(property, getMethod, setMethod);
        }

        private void AddPropertyDefinition(PropertyInfo property, MethodBuilder getMethod, MethodBuilder setMethod)
        {
            PropertyBuilder newProperty =
                _typeBuilder.DefineProperty(
                    property.Name,
                    property.Attributes,
                    property.PropertyType,
                    property.GetIndexParameters().Select(param => param.ParameterType).ToArray());

            if (getMethod != null)
            {
                newProperty.SetGetMethod(getMethod);
            }

            if (setMethod != null)
            {
                newProperty.SetSetMethod(setMethod);
            }
        }

        private MethodBuilder OverridePropertyMethod(MethodInfo method, int count)
        {
            return method == null
                ? null
                : new InterfaceMethodOverride(
                    _typeBuilder,
                    _proxyInterceptionPipelineField,
                    _targetField,
                    method,
                    _targetInterface,
                    _genericParameterMapper,
                    _explicitImplementation,
                    count)
                    .AddMethod();
        }

        private IEnumerable<EventInfo> EventsToIntercept()
        {
            return _interface.GetEvents(
                BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        }

        private void OverrideEvent(EventInfo @event, int count)
        {
            MethodBuilder addMethod = OverrideEventMethod(@event.GetAddMethod(), count);
            MethodBuilder removeMethod = OverrideEventMethod(@event.GetRemoveMethod(), count);
            AddEventDefinition(@event, addMethod, removeMethod);
        }

        private void AddEventDefinition(EventInfo @event, MethodBuilder addMethod, MethodBuilder removeMethod)
        {
            EventBuilder newEvent = _typeBuilder.DefineEvent(@event.Name, @event.Attributes, @event.EventHandlerType);

            if (addMethod != null)
            {
                newEvent.SetAddOnMethod(addMethod);
            }

            if (removeMethod != null)
            {
                newEvent.SetRemoveOnMethod(removeMethod);
            }
        }

        private MethodBuilder OverrideEventMethod(MethodInfo method, int count)
        {
            return method == null
                ? null
                : new InterfaceMethodOverride(
                    _typeBuilder,
                    _proxyInterceptionPipelineField,
                    _targetField,
                    method,
                    _targetInterface,
                    _genericParameterMapper,
                    _explicitImplementation,
                    count)
                    .AddMethod();
        }
    }
}
