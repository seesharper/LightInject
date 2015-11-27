using System;

namespace LightInject.Interception.Tests
{
    using Xunit;

    using Moq;

    [Collection("Interception")]
    public class InterceptorTests
    {
        [Fact]
        public void Execute_CompositeInterceptor_CallsProceedInExpectedSequence()
        {

            int callOrder = 0;
            var firstInterceptorMock = new Mock<IInterceptor>();
            firstInterceptorMock.Setup(i => i.Invoke(It.IsAny<IInvocationInfo>()))
                                .Callback(() => Assert.Equal(0, callOrder++))
                                .Returns<IInvocationInfo>(ii => ii.Proceed());


            var secondInterceptorMock = new Mock<IInterceptor>();
            secondInterceptorMock.Setup(i => i.Invoke(It.IsAny<IInvocationInfo>()))
                                 .Callback(() => Assert.Equal(1, callOrder++))
                                .Returns<IInvocationInfo>(ii => ii.Proceed());

            var thirdInterceptorMock = new Mock<IInterceptor>();
            thirdInterceptorMock.Setup(i => i.Invoke(It.IsAny<IInvocationInfo>()))
                                .Callback(() => Assert.Equal(2, callOrder++))
                                .Returns<IInvocationInfo>(ii => ii.Proceed());

            var invocationMock = new Mock<IInvocationInfo>();

            var compositeInterceptor = new CompositeInterceptor(new[]
                                                                    {
                                                                        new Lazy<IInterceptor>(() => firstInterceptorMock.Object), 
                                                                        new Lazy<IInterceptor>(() => secondInterceptorMock.Object),
                                                                        new Lazy<IInterceptor>(() => thirdInterceptorMock.Object)
                                                                    });

            compositeInterceptor.Invoke(invocationMock.Object);
        }

        [Fact]
        public void Execute_CompositeInterceptor_PassesMethodToInterceptor()
        {
            var firstInterceptorMock = new Mock<IInterceptor>();
            var secondInterceptorMock = new Mock<IInterceptor>();
            var invocationMock = new Mock<IInvocationInfo>();
            var method = typeof(IMethodWithNoParameters).GetMethods()[0];

            invocationMock.SetupGet(i => i.Method).Returns(method);
            var compositeInterceptor = new CompositeInterceptor(new[]
                                                                    {
                                                                        new Lazy<IInterceptor>(() => firstInterceptorMock.Object), 
                                                                        new Lazy<IInterceptor>(() => secondInterceptorMock.Object),
                                                                    });

            compositeInterceptor.Invoke(invocationMock.Object);

            firstInterceptorMock.Verify(i => i.Invoke(It.Is<IInvocationInfo>(ii => ii.Method == method)));
        }

        [Fact]
        public void Execute_CompositeInterceptor_PassesArgumentsToInterceptor()
        {
            var firstInterceptorMock = new Mock<IInterceptor>();
            var secondInterceptorMock = new Mock<IInterceptor>();
            var invocationMock = new Mock<IInvocationInfo>();
            var arguments = new object[]{42};

            invocationMock.SetupGet(i => i.Arguments).Returns(arguments);
            var compositeInterceptor = new CompositeInterceptor(new[]
                                                                    {
                                                                        new Lazy<IInterceptor>(() => firstInterceptorMock.Object), 
                                                                        new Lazy<IInterceptor>(() => secondInterceptorMock.Object),
                                                                    });

            compositeInterceptor.Invoke(invocationMock.Object);

            firstInterceptorMock.Verify(i => i.Invoke(It.Is<IInvocationInfo>(ii => ii.Arguments == arguments)));
        }

        [Fact]
        public void Execute_CompositeInterceptor_PassesProxyToInterceptor()
        {
            var firstInterceptorMock = new Mock<IInterceptor>();
            var secondInterceptorMock = new Mock<IInterceptor>();
            var invocationMock = new Mock<IInvocationInfo>();
            IProxy proxy = new Mock<IProxy>().Object;

            invocationMock.SetupGet(i => i.Proxy).Returns(proxy);
            var compositeInterceptor = new CompositeInterceptor(new[]
                                                                    {
                                                                        new Lazy<IInterceptor>(() => firstInterceptorMock.Object), 
                                                                        new Lazy<IInterceptor>(() => secondInterceptorMock.Object),
                                                                    });

            compositeInterceptor.Invoke(invocationMock.Object);

            firstInterceptorMock.Verify(i => i.Invoke(It.Is<IInvocationInfo>(ii => ii.Proxy == proxy)));
        }


        [Fact]
        public void CreateMethodInterceptor_SingleInterceptor_ReturnsInterceptor()
        {
            var sampleInterceptor = new Mock<IInterceptor>().Object;

            var interceptor = MethodInterceptorFactory.CreateMethodInterceptor(
                new[] { new Lazy<IInterceptor>(() => sampleInterceptor) });

            Assert.Equal(sampleInterceptor, interceptor.Value);
        }

        [Fact]
        public void CreateMethodInterceptor_MultipleInterceptors_ReturnsCompositeInterceptor()
        {
            var firstInterceptor = new Mock<IInterceptor>().Object;
            var secondInterceptor = new Mock<IInterceptor>().Object;

            var interceptor = MethodInterceptorFactory.CreateMethodInterceptor(
                new[] { new Lazy<IInterceptor>(() => firstInterceptor), new Lazy<IInterceptor>(() => secondInterceptor) });

            Assert.IsType(typeof(CompositeInterceptor), interceptor.Value);
        }
    }
}
