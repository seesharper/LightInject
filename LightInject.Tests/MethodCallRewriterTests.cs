using System;
using System.Linq.Expressions;
using System.Collections.Generic;
using LightInject;
using LightInject.SampleLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DependencyInjector.Tests
{
    [TestClass]
    public class MethodCallRewriterTests
    {
        [TestMethod]
        public void Visit_GetInstance_ReturnExpressionWithActualBodyExpression()
        {
            var container = new Mock<IServiceContainer>().Object;            
            var rewriter = new ServiceContainer.MethodCallRewriter((t,s) => Expression.New(typeof(Foo)));
            Expression<Func<IFoo>> expression = () => container.GetInstance<IFoo>();
            Expression result = rewriter.Visit(expression);
            Assert.IsFalse(result.Contains<MethodCallExpression>(me => true));
        }

        [TestMethod]
        public void Visit_GetNamedInstance_ReturnExpressionWithActualBodyExpression()
        {
            var container = new Mock<IServiceContainer>().Object;
            var rewriter = new ServiceContainer.MethodCallRewriter((t, s) => Expression.New(typeof(Foo)));
            Expression<Func<IFoo>> expression = () => container.GetInstance<IFoo>("SomeFoo");
            Expression result = rewriter.Visit(expression);
            Assert.IsFalse(result.Contains<MethodCallExpression>(me => true));
        }

        [TestMethod]
        public void Visit_GetAllInstances_ReturnExpressionWithActualBodyExpression()
        {
            var container = new Mock<IServiceContainer>().Object;
            var rewriter = new ServiceContainer.MethodCallRewriter((t, s) => Expression.NewArrayInit(typeof(IFoo),Expression.New(typeof(Foo))));
            Expression<Func<IEnumerable<IFoo>>> expression = () => container.GetAllInstances<IFoo>();
            Expression result = rewriter.Visit(expression);
            Assert.IsFalse(result.Contains<MethodCallExpression>(me => true));
        }

        [TestMethod]
        public void Visit_UnknownMethod_ReturnsOriginalExpression()
        {            
            var rewriter = new ServiceContainer.MethodCallRewriter((t, s) => Expression.New(typeof(Foo)));
            Expression<Func<IFoo>> expression = () => GetFoo();
            Expression result = rewriter.Visit(expression);
            Assert.IsTrue(result.Contains<MethodCallExpression>(me => true));
        }



        private IFoo GetFoo()
        {
            return null;
        }

    }
}
