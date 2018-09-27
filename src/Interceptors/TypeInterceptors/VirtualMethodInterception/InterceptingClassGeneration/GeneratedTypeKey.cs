

using System;
using System.Collections.Generic;

namespace Unity.Interception.Interceptors.TypeInterceptors.VirtualMethodInterception.InterceptingClassGeneration
{
    internal struct GeneratedTypeKey
    {
        private readonly Type _baseType;
        private readonly Type[] _additionalInterfaces;

        public GeneratedTypeKey(Type baseType, Type[] additionalInterfaces)
        {
            _baseType = baseType;
            _additionalInterfaces = additionalInterfaces;
        }

        internal class GeneratedTypeKeyComparer : IEqualityComparer<GeneratedTypeKey>
        {
            public bool Equals(GeneratedTypeKey x, GeneratedTypeKey y)
            {
                if (!(x._baseType.Equals(y._baseType) && x._additionalInterfaces.Length == y._additionalInterfaces.Length))
                {
                    return false;
                }
                for (int i = 0; i < x._additionalInterfaces.Length; i++)
                {
                    if (!x._additionalInterfaces[i].Equals(y._additionalInterfaces[i]))
                    {
                        return false;
                    }
                }

                return true;
            }

            public int GetHashCode(GeneratedTypeKey obj)
            {
                return obj._baseType.GetHashCode();
            }
        }
    }
}
