namespace LightInject.Tests
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Reflection.Emit;


    public class MethodBuilderMethodSkeleton : IMethodSkeleton
    {
        private readonly string outputPath;

        

        private readonly string fileName;
        private AssemblyBuilder assemblyBuilder;
        private TypeBuilder typeBuilder;
        private MethodBuilder methodBuilder;

        public MethodBuilderMethodSkeleton(string outputPath, Type returnType, Type[] parameterTypes)
        {
            this.outputPath = outputPath;
        
            fileName = Path.GetFileName(outputPath);
            CreateAssemblyBuilder();
            CreateTypeBuilder();
            CreateMethodBuilder(returnType, parameterTypes);
        }

        public ILGenerator GetILGenerator()
        {
            return methodBuilder.GetILGenerator();
        }

        public Delegate CreateDelegate()
        {            
            methodBuilder.GetILGenerator().Emit(OpCodes.Ret);
            var dynamicType = typeBuilder.CreateType();
            assemblyBuilder.Save(fileName);
            Console.WriteLine("Saving file " + fileName);
            AssemblyAssert.IsValidAssembly(outputPath);
            MethodInfo methodInfo = dynamicType.GetMethod("DynamicMethod", BindingFlags.Static | BindingFlags.Public);
            return Delegate.CreateDelegate(typeof(Func<object[], object>), methodInfo);
        }

        private void CreateAssemblyBuilder()
        {
            AppDomain myDomain = AppDomain.CurrentDomain;
            assemblyBuilder = myDomain.DefineDynamicAssembly(CreateAssemblyName(), AssemblyBuilderAccess.RunAndSave, Path.GetDirectoryName(outputPath));
        }

        private AssemblyName CreateAssemblyName()
        {
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(outputPath);
            return new AssemblyName(fileNameWithoutExtension);
        }

        private void CreateTypeBuilder()
        {
            var moduleName = Path.GetFileNameWithoutExtension(outputPath);
            ModuleBuilder module = assemblyBuilder.DefineDynamicModule(moduleName, fileName);
            typeBuilder = module.DefineType("DynamicType", TypeAttributes.Public);
        }

        private void CreateMethodBuilder(Type returnType, Type[] parameterTypes)
        {
            methodBuilder = typeBuilder.DefineMethod(
                "DynamicMethod", MethodAttributes.Public | MethodAttributes.Static, returnType, parameterTypes);
            methodBuilder.InitLocals = true;

        }
    }
}