namespace LightInject.Interception.Tests
{
    using System;
    using System.IO;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class DynamicMethodBuilderVerificationTests : DynamicMethodBuilderTests
    {
        protected override IMethodBuilder GetMethodBuilder()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DynamicMethodAssembly.dll");
            return new DynamicMethodBuilder(() => new MethodBuilderMethodSkeleton(path));
        }
    }
}