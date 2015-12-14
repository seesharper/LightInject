namespace LightInject.Interception.Tests
{
    using System;
    using System.Linq;

    using Xunit;

    [Collection("Interception")]
    public class MethodSelectorTests
    {
        [Fact]
        public void Execute_InterfaceWithMethod_ReturnsInterfaceMethodsAndMethodsInheritedFromObject()
        {
            var methodSelector = new MethodSelector();

            var methods = methodSelector.Execute(typeof(IMethodWithNoParameters), Type.EmptyTypes);

            Assert.Equal(4, methods.Length);
        }

        [Fact]
        public void Execute_InterfaceWithProperty_DoesNotReturnGetAndSetMethods()
        {
            var methodSelector = new MethodSelector();

            var methods = methodSelector.Execute(typeof(IClassWithProperty), Type.EmptyTypes);

            Assert.True(!methods.Any(m => m.IsDeclaredBy<IClassWithProperty>()));
        }

        [Fact]
        public void Execute_InterfaceWithEvent_DoesNotReturnAddAndRemoveMethods()
        {
            var methodSelector = new MethodSelector();

            var methods = methodSelector.Execute(typeof(IClassWithEvent), Type.EmptyTypes);

            Assert.True(!methods.Any(m => m.IsDeclaredBy<IClassWithEvent>()));
        }

        [Fact]
        public void Execute_ClassWithVirtualMethod_ReturnsOnlyVirtualMethods()
        {
            var methodSelector = new MethodSelector();

            var methods = methodSelector.Execute(typeof(ClassWithVirtualAndNonVirtualMethod), Type.EmptyTypes);

            Assert.True(methods.All(m => m.IsVirtual));
        }

        [Fact]
        public void Execute_ClassWithProtectedVirtualMethod_ReturnsProtectedVirtualMethod()
        {
            var methodSelector = new MethodSelector();

            var methods = methodSelector.Execute(typeof(ClassWithVirtualProtectedMethod), Type.EmptyTypes);

            Assert.True(methods.Any(m => m.Name == "Execute"));
        }

        [Fact]
        public void Execute_ClassWithNoMethods_ReturnsInterceptableMethodsInheritedFromObject()
        {
            var methodSelector = new MethodSelector();

            var methods = methodSelector.Execute(typeof(ClassWithNoMethods), Type.EmptyTypes);

            Assert.Equal(3, methods.Length);
            Assert.True(methods.Any(m => m.Name == "ToString"));
            Assert.True(methods.Any(m => m.Name == "GetHashCode"));
            Assert.True(methods.Any(m => m.Name == "Equals"));
        }

        [Fact]
        public void Execute_ClassWithPrivateMethod_DoesNotReturnPrivateMethod()
        {
            var methodSelector = new MethodSelector();

            var methods = methodSelector.Execute(typeof(ClassWithPrivateMethod), Type.EmptyTypes);

            Assert.True(!methods.Any(m => m.IsDeclaredBy<ClassWithPrivateMethod>()));
        }

        [Fact]
        public void ShouldNotSelectFinalMethodsFromInterface()
        {
            var methodSelector = new MethodSelector();
            var methods = methodSelector.Execute(typeof(ClassWithOneMethod), Type.EmptyTypes);
            Assert.False(methods.Any(m => m.IsFinal));
            
        }
    }
}