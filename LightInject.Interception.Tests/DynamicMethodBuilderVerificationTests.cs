namespace LightInject.Interception.Tests
{
    using System;
    using System.IO;
    using System.Reflection;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class DynamicMethodBuilderVerificationTests : DynamicMethodBuilderTests
    {
        protected override IMethodBuilder GetMethodBuilder()
        {
            var dynamicMethodBuilder = new DynamicMethodBuilder();
            var field = typeof(DynamicMethodBuilder).GetField(
                "methodSkeletonFactory", BindingFlags.Instance | BindingFlags.NonPublic);
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DynamicMethodAssembly.dll");
            Func<IDynamicMethodSkeleton> methodSkeletonFactory = () => new InterceptionMethodBuilderMethodSkeleton(path);
            field.SetValue(dynamicMethodBuilder, methodSkeletonFactory);
            return dynamicMethodBuilder;
        }
    }
}