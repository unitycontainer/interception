

using System;
using System.Reflection.Emit;

namespace Unity.Interception.Interceptors.InstanceInterceptors.InterfaceInterception
{
    public partial class InterfaceInterceptorClassGenerator
    {
        private static ModuleBuilder GetModuleBuilder()
        {
            string moduleName = Guid.NewGuid().ToString("N");
#if DEBUG_SAVE_GENERATED_ASSEMBLY
            return AssemblyBuilder.DefineDynamicModule(moduleName, moduleName + ".dll", true);
#else
            return AssemblyBuilder.DefineDynamicModule(moduleName);
#endif
        }
    }
}