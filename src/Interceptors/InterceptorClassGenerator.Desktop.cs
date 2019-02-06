using System;
using System.Collections.Concurrent;
using System.Reflection.Emit;

namespace Unity.Interception.Interceptors
{
    public static class InterceptorClassGenerator
    {
        private static readonly ConcurrentDictionary<AssemblyBuilder, ModuleBuilder> ModuleByAssembly =
            new ConcurrentDictionary<AssemblyBuilder, ModuleBuilder>();

        public static ModuleBuilder CreateModuleBuilder(AssemblyBuilder assemblyBuilder)
        {
            return ModuleByAssembly.GetOrAdd(assemblyBuilder, assembly =>
            {
                string moduleName = Guid.NewGuid().ToString("N");
#if DEBUG_SAVE_GENERATED_ASSEMBLY
            return AssemblyBuilder.DefineDynamicModule(moduleName, moduleName + ".dll", true);
#else
                return assembly.DefineDynamicModule(moduleName);
#endif
            });
        }
    }
}