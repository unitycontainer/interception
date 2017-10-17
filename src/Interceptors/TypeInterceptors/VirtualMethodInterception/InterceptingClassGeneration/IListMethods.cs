// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.Collections;
using System.Reflection;

namespace Unity.Interception.Interceptors.TypeInterceptors.VirtualMethodInterception.InterceptingClassGeneration
{
    internal static class IListMethods
    {
        internal static MethodInfo GetItem => typeof(IList).GetProperty("Item")?.GetGetMethod();
    }
}
