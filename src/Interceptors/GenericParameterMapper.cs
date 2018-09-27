

using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Interception.Properties;
using Unity.Interception.Utilities;

namespace Unity.Interception.Interceptors
{
    /// <summary>
    /// Maps types involving generic parameter types from reflected types into equivalent versions involving generated types.
    /// </summary>
    public class GenericParameterMapper
    {
        private static readonly KeyValuePair<Type, Type>[] EmptyMappings = new KeyValuePair<Type, Type>[0];

        private readonly IDictionary<Type, Type> _mappedTypesCache = new Dictionary<Type, Type>();
        private readonly ICollection<KeyValuePair<Type, Type>> _localMappings;
        private readonly GenericParameterMapper _parent;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericParameterMapper"/> class.
        /// </summary>
        /// <param name="type">A constructed generic type, open or closed.</param>
        /// <param name="parent">The parent mapper, or <see langword="null"/>.</param>
        public GenericParameterMapper(Type type, GenericParameterMapper parent)
        {
            Guard.ArgumentNotNull(type, "type");

            if (type.IsGenericType)
            {
                if (type.IsGenericTypeDefinition)
                {
                    throw new ArgumentException(Resources.ExceptionCannotMapGenericTypeDefinition, "type");
                }

                _parent = parent;
                _localMappings =
                    CreateMappings(
                        type.GetGenericTypeDefinition().GetGenericArguments(),
                        type.GetGenericArguments());
            }
            else
            {
                _localMappings = EmptyMappings;
                _parent = null;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericParameterMapper"/> class.
        /// </summary>
        /// <param name="reflectedParameters">The reflected generic parameters.</param>
        /// <param name="generatedParameters">The generated generic parameters.</param>
        public GenericParameterMapper(Type[] reflectedParameters, Type[] generatedParameters)
            : this(reflectedParameters, generatedParameters, null)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericParameterMapper"/> class.
        /// </summary>
        /// <param name="reflectedParameters">The reflected generic parameters.</param>
        /// <param name="generatedParameters">The generated generic parameters.</param>
        /// <param name="parent">The parent mapper, or <see langword="null"/>.</param>
        public GenericParameterMapper(Type[] reflectedParameters, Type[] generatedParameters, GenericParameterMapper parent)
        {
            _parent = parent;
            _localMappings = CreateMappings(reflectedParameters, generatedParameters);
        }

        private static ICollection<KeyValuePair<Type, Type>> CreateMappings(Type[] reflectedParameters, Type[] generatedParameters)
        {
            Guard.ArgumentNotNull(reflectedParameters, "reflectedParameters");
            Guard.ArgumentNotNull(generatedParameters, "generatedParameters");

            if (reflectedParameters.Length != generatedParameters.Length)
            {
                throw new ArgumentException(Resources.ExceptionMappedParametersDoNotMatch, "reflectedParameters");
            }

            var mappings = new List<KeyValuePair<Type, Type>>();
            for (int i = 0; i < reflectedParameters.Length; i++)
            {
                mappings.Add(new KeyValuePair<Type, Type>(reflectedParameters[i], generatedParameters[i]));
            }

            return mappings;
        }

        /// <summary>
        /// Maps a type to an equivalent type involving generated types.
        /// </summary>
        /// <param name="typeToMap">The type to map.</param>
        public Type Map(Type typeToMap)
        {
            if (!_mappedTypesCache.TryGetValue(typeToMap, out var mappedType))
            {
                mappedType = DoMap(typeToMap);
                _mappedTypesCache[typeToMap] = mappedType;
            }

            return mappedType;
        }

        private Type DoMap(Type typeToMap)
        {
            if (!typeToMap.IsGenericParameter)
            {
                // The type does not represent a generic parameter, but it might be a type constructed with 
                // generic parameters

                if (typeToMap.IsArray)
                {
                    // Array case: create a new array type for the mapped element type

                    var mappedElementType = Map(typeToMap.GetElementType());

                    // assume a rank of 1 means vector. there is no way to check for this (IsSzArray is not public)
                    var mappedArrayType =
                        typeToMap.GetArrayRank() == 1
                            ? mappedElementType.MakeArrayType()
                            : mappedElementType.MakeArrayType(typeToMap.GetArrayRank());

                    return mappedArrayType;
                }

                if (typeToMap.IsGenericType)
                {
                    // Generic type case: construct a new generic type with mapped types

                    var mappedGenericArguments = typeToMap.GetGenericArguments().Select(gp => Map(gp)).ToArray();
                    var mappedGenericType = typeToMap.GetGenericTypeDefinition().MakeGenericType(mappedGenericArguments);

                    return mappedGenericType;
                }

                return typeToMap;
            }
            // Map the generic parameter from the reflected type into the generated generic parameter

            var mappedType =
                _localMappings.Where(kvp => kvp.Key == typeToMap).Select(kvp => kvp.Value).FirstOrDefault()
                ?? typeToMap;

            if (_parent != null)
            {
                mappedType = _parent.Map(mappedType);
            }

            return mappedType;
        }

        /// <summary>
        /// Gets the default mapper.
        /// </summary>
        public static GenericParameterMapper DefaultMapper { get; } = new GenericParameterMapper(Type.EmptyTypes, 
                                                                                                 Type.EmptyTypes, null);

        /// <summary>
        /// Gets the formal parameters.
        /// </summary>
        /// <returns></returns>
        public Type[] GetReflectedParameters()
        {
            return _localMappings.Select(kvp => kvp.Key).ToArray();
        }

        /// <summary>
        /// Gets the actual parameters.
        /// </summary>
        /// <returns></returns>
        public Type[] GetGeneratedParameters()
        {
            return _localMappings.Select(kvp => kvp.Value).ToArray();
        }
    }
}
