using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace LightInject.Tests
{
    public class ClosedGenericEnumerableTests
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ShouldNotDuplicateClosedGenericServicesWhenEnumerableIsResolvedMultipleTimes(bool enableMicrosoftCompatibility)
        {
            var container = CreateContainer(enableMicrosoftCompatibility);

            var handler = container.GetInstance<BehaviorHandler<int>>();
            var behaviors = container.GetInstance<IEnumerable<IBehavior<int>>>();

            Assert.Equal(new[] { typeof(FirstBehavior<int>), typeof(SecondBehavior<int>) }, handler.Behaviors.Select(b => b.GetType()));
            Assert.Equal(new[] { typeof(FirstBehavior<int>), typeof(SecondBehavior<int>) }, behaviors.Select(b => b.GetType()));
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ShouldNotDuplicateClosedGenericServicesAfterCanGetInstance(bool enableMicrosoftCompatibility)
        {
            var container = CreateContainer(enableMicrosoftCompatibility);

            Assert.True(container.CanGetInstance(typeof(IEnumerable<IBehavior<int>>), string.Empty));
            var behaviors = container.GetInstance<IEnumerable<IBehavior<int>>>();

            Assert.Equal(new[] { typeof(FirstBehavior<int>), typeof(SecondBehavior<int>) }, behaviors.Select(b => b.GetType()));
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ShouldNotDuplicateClosedGenericServiceResolvedAsSingleServiceBeforeEnumerable(bool enableMicrosoftCompatibility)
        {
            var container = CreateContainer(enableMicrosoftCompatibility);

            var behavior = container.GetInstance<IBehavior<int>>(enableMicrosoftCompatibility ? string.Empty : "second");
            var behaviors = container.GetInstance<IEnumerable<IBehavior<int>>>();

            Assert.IsType<SecondBehavior<int>>(behavior);
            Assert.Equal(new[] { typeof(FirstBehavior<int>), typeof(SecondBehavior<int>) }, behaviors.Select(b => b.GetType()));
        }

        private static ServiceContainer CreateContainer(bool enableMicrosoftCompatibility)
        {
            var container = new ServiceContainer(new ContainerOptions { EnableMicrosoftCompatibility = enableMicrosoftCompatibility, EnableVariance = false });
            container.Register(typeof(IBehavior<>), typeof(FirstBehavior<>));

            // Without Microsoft compatibility, registering another default service would replace the first one.
            container.Register(typeof(IBehavior<>), typeof(SecondBehavior<>), enableMicrosoftCompatibility ? string.Empty : "second");
            container.Register(typeof(BehaviorHandler<>), typeof(BehaviorHandler<>));
            return container;
        }

        public interface IBehavior<T>
        {
        }

        public class FirstBehavior<T> : IBehavior<T>
        {
        }

        public class SecondBehavior<T> : IBehavior<T>
        {
        }

        public class BehaviorHandler<T>
        {
            public BehaviorHandler(IEnumerable<IBehavior<T>> behaviors)
            {
                Behaviors = behaviors.ToArray();
            }

            public IBehavior<T>[] Behaviors { get; }
        }
    }
}
