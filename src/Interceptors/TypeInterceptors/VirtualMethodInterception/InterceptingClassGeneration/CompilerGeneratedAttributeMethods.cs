// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.Interception.Utilities;

namespace Unity.Interception.Interceptors.TypeInterceptors.VirtualMethodInterception.InterceptingClassGeneration
{
    internal static class CompilerGeneratedAttributeMethods
    {
        public static ConstructorInfo CompilerGeneratedAttribute
        {
            get { return StaticReflection.GetConstructorInfo(() => new CompilerGeneratedAttribute()); }
        }
    }
}
