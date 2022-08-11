#if !USE_EXPRESSIONS
using System;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using ILVerifier;
namespace LightInject.Tests
{
    public class MethodBuilderMethodSkeleton : IMethodSkeleton
    {
        private readonly string outputPath;
        private readonly string fileName;
        private AssemblyBuilder assemblyBuilder;
        private TypeBuilder typeBuilder;
        private MethodBuilder methodBuilder;
        private IEmitter emitter;

        public MethodBuilderMethodSkeleton(Type returnType, Type[] parameterTypes, string outputPath)
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

        IEmitter IMethodSkeleton.GetEmitter()
        {
            return emitter;
        }

        public Delegate CreateDelegate(Type delegateType)
        {
            var dynamicType = typeBuilder.CreateType();
            new Verifier()
                .WithAssemblyReferenceFromType<ServiceContainer>()
                .WithAssemblyReferenceFromType<MethodBuilderMethodSkeleton>()
                .Verify(dynamicType.Assembly);
            MethodInfo methodInfo = dynamicType.GetMethod("DynamicMethod", BindingFlags.Static | BindingFlags.Public);
            return Delegate.CreateDelegate(delegateType, methodInfo);
        }

        public Delegate CreateDelegate(Type delegateType, object target)
        {
            var dynamicType = typeBuilder.CreateType();
            new Verifier()
                .WithAssemblyReferenceFromType<ServiceContainer>()
                .WithAssemblyReferenceFromType<MethodBuilderMethodSkeleton>()
                .Verify(dynamicType.Assembly);
            MethodInfo methodInfo = dynamicType.GetMethod("DynamicMethod", BindingFlags.Static | BindingFlags.Public);
            return Delegate.CreateDelegate(delegateType, target, methodInfo);
        }

        private void CreateAssemblyBuilder()
        {
            assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("DynamicMethodAssembly"), AssemblyBuilderAccess.Run);
        }

        private AssemblyName CreateAssemblyName()
        {
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(outputPath);
            return new AssemblyName(fileNameWithoutExtension);
        }

        private void CreateTypeBuilder()
        {
            var moduleName = Path.GetFileNameWithoutExtension(outputPath);
            ModuleBuilder module = assemblyBuilder.DefineDynamicModule(moduleName);
            typeBuilder = module.DefineType("DynamicType", TypeAttributes.Public);
        }

        private void CreateMethodBuilder(Type returnType, Type[] parameterTypes)
        {
            methodBuilder = typeBuilder.DefineMethod(
                "DynamicMethod", MethodAttributes.Public | MethodAttributes.Static, returnType, parameterTypes);
            methodBuilder.InitLocals = true;
            emitter = new Emitter(methodBuilder.GetILGenerator(), parameterTypes);
        }
    }
}
#endif