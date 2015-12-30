namespace LightInject.AutoFactory.Tests
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Threading;

    using AssemblyName = System.Reflection.AssemblyName;

    public class VerifiableTypeBuilderFactory : ITypeBuilderFactory
    {
        public TypeBuilder CreateTypeBuilder(Type targetType, Type[] additionalInterfaces)
        {
            ModuleBuilder moduleBuilder = GetModuleBuilder();
            const TypeAttributes typeAttributes = TypeAttributes.Public | TypeAttributes.Class;
            var typeName = targetType.Name + "AutoFactory";
            if (targetType.IsInterface)
            {
                Type[] interfaceTypes = new[] { targetType }.Concat(additionalInterfaces).ToArray();
                return moduleBuilder.DefineType(typeName, typeAttributes, null, interfaceTypes);
            }

            return moduleBuilder.DefineType(typeName, typeAttributes, targetType, additionalInterfaces);
        }

        public Type CreateType(TypeBuilder typeBuilder)
        {
            Type proxyType = typeBuilder.CreateType();
            ((AssemblyBuilder)typeBuilder.Assembly).Save("AutoFactoryAssembly.dll");
            AssemblyAssert.IsValidAssembly("AutoFactoryAssembly.dll");
            return proxyType;
        }

        private static ModuleBuilder GetModuleBuilder()
        {
            AssemblyBuilder assemblyBuilder = GetAssemblyBuilder();
            return assemblyBuilder.DefineDynamicModule("AutoFactoryAssembly", "AutoFactoryAssembly.dll");
        }

        private static AssemblyBuilder GetAssemblyBuilder()
        {
            var assemblyPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AutoFactoryAssembly.dll");
            var assemblybuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(
                new AssemblyName("AutoFactoryAssembly"), AssemblyBuilderAccess.RunAndSave, Path.GetDirectoryName(assemblyPath));
            return assemblybuilder;
        }
    }
}