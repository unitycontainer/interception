// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Reflection;
using Unity.Interception.InterceptionBehaviors;
using Unity.Interception.Utilities;

namespace Unity.Interception.Interceptors.TypeInterceptors.VirtualMethodInterception.InterceptingClassGeneration
{
    internal static class InvokeInterceptionBehaviorDelegateMethods
    {
        internal static ConstructorInfo InvokeInterceptionBehaviorDelegate => typeof(InvokeInterceptionBehaviorDelegate)
            .GetConstructor(Sequence.Collect(typeof(object), typeof(IntPtr)));
    }
}
