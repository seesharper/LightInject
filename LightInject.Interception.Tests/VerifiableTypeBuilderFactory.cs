namespace LightInject.Interception.Tests
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;

    using AssemblyName = System.Reflection.AssemblyName;

    public class VerifiableTypeBuilderFactory : ITypeBuilderFactory
    {
        public TypeBuilder CreateTypeBuilder(Type baseType, Type[] additionalInterfaces)
        {
            ModuleBuilder moduleBuilder = GetModuleBuilder();
            const TypeAttributes TypeAttributes = TypeAttributes.Public | TypeAttributes.Class;
            var typeName = baseType.Name + "Proxy";
            Type[] interfaceTypes = new[] { baseType }.Concat(additionalInterfaces).ToArray();
            return moduleBuilder.DefineType(typeName, TypeAttributes, null, interfaceTypes);
        }

        private static ModuleBuilder GetModuleBuilder()
        {
            AssemblyBuilder assemblyBuilder = GetAssemblyBuilder();
            return assemblyBuilder.DefineDynamicModule("ProxyAssembly", "ProxyAssembly.dll");
        }

        private static AssemblyBuilder GetAssemblyBuilder()
        {
            var assemblyPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ProxyAssembly.dll");

            var assemblybuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(
                new AssemblyName("ProxyAssembly"), AssemblyBuilderAccess.RunAndSave, Path.GetDirectoryName(assemblyPath));
            return assemblybuilder;
        }
    }
}