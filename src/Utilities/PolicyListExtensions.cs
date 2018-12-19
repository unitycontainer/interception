using System;
using System.Globalization;
using System.Reflection;
using Unity.Builder;

// ReSharper disable once CheckNamespace

namespace Unity.Policy
{
    public static class LegacyPolicyListUtilityExtensions
    {
        #region Get

        /// <summary>
        /// Default resolution/search algorithm for retrieving requested policy
        /// </summary>
        /// <param name="policies"><see cref="IPolicyList"/> to search.</param>
        /// <param name="policyInterface">The interface the policy is registered under.</param>
        /// <param name="buildKey">The key the policy applies.</param>
        /// <returns>The policy in the list, if present; returns null otherwise.</returns>
        public static object GetOrDefault(this IPolicyList policies, Type policyInterface, object buildKey)
        {
            var tuple = ParseBuildKey(buildKey);

            return (buildKey != null ? policies.Get(tuple.Item1, tuple.Item2, policyInterface) : null) ??
                   (tuple.Item1 != null && tuple.Item1.GetTypeInfo().IsGenericType
                       ? Get(policies, policyInterface, ReplaceType(tuple.Item1.GetGenericTypeDefinition())) : null) ??
                   (tuple.Item1 != null ? policies.Get(tuple.Item1, string.Empty, policyInterface) : null) ??
                   (tuple.Item1 != null && tuple.Item1.GetTypeInfo().IsGenericType
                       ? policies.Get(tuple.Item1.GetGenericTypeDefinition(), string.Empty, policyInterface) : null) ??
                   policies.Get(null, null, policyInterface);

            object ReplaceType(Type newType)
            {
                switch (buildKey)
                {
                    case Type _:
                        return newType;

                    case INamedType originalKey:
                        return new NamedTypeBuildKey(newType, originalKey.Name);

                    default:
                        throw new ArgumentException(string.Format(CultureInfo.CurrentCulture,
                            "Cannot extract type from build key {0}.", buildKey), nameof(buildKey));
                }
            }

            object Get(IPolicyList list, Type policy, object key)
            {
                var tupleKey = ParseBuildKey(key);
                return list.Get(tupleKey.Item1, tupleKey.Item2, policy);
            }

            Tuple<Type, string> ParseBuildKey(object key)
            {
                switch (key)
                {
                    case INamedType namedBuildKey:
                        return new Tuple<Type, string>(namedBuildKey.Type, namedBuildKey.Name);

                    case Type typeBuildKey:
                        return new Tuple<Type, string>(typeBuildKey, string.Empty);

                    case string name:
                        return new Tuple<Type, string>(null, name);

                    case null:
                        return new Tuple<Type, string>(null, null);

                    default:
                        return new Tuple<Type, string>(key.GetType(), null);
                }
            }
        }

        public static object GetOrDefault(this IPolicyList policies, Type policyInterface, Type type, string name)
        {
            return policies.Get(type, name, policyInterface) ??
                   (type != null && type.GetTypeInfo().IsGenericType
                       ? policies.Get(type.GetGenericTypeDefinition(), name, policyInterface) : null) ??
                   (type != null ? policies.Get(type, string.Empty, policyInterface) : null) ??
                   (type != null && type.GetTypeInfo().IsGenericType
                       ? policies.Get(type.GetGenericTypeDefinition(), string.Empty, policyInterface) : null) ??
                   policies.Get(null, null, policyInterface);
        }

        #endregion
    }
}
