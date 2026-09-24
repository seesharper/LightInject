using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace LightInject.Tests
{
    public class Issue614
    {
        [Fact]
        public void ShouldResolveOpenGenericEnumerablesConcurrentlyWithCanGetInstance()
        {
            var closingTypes = typeof(string).Assembly.GetExportedTypes()
                .Where(t => t.IsClass && !t.IsGenericTypeDefinition && !t.ContainsGenericParameters)
                .Take(200)
                .ToArray();

            for (int iteration = 0; iteration < 20; iteration++)
            {
                var container = new ServiceContainer(new ContainerOptions { EnableMicrosoftCompatibility = true, EnableVariance = false });
                container.Register(typeof(IBehavior<>), typeof(FirstBehavior<>));
                container.Register(typeof(IBehavior<>), typeof(SecondBehavior<>));
                container.Register(typeof(Handler<>), typeof(Handler<>));

                using var barrier = new Barrier(Environment.ProcessorCount * 2);
                var tasks = Enumerable.Range(0, Environment.ProcessorCount * 2).Select(taskIndex => Task.Run(() =>
                {
                    barrier.SignalAndWait();
                    for (int i = taskIndex; i < closingTypes.Length; i += 2)
                    {
                        var closingType = closingTypes[i];
                        if (taskIndex % 2 == 0)
                        {
                            container.CanGetInstance(typeof(IEnumerable<>).MakeGenericType(typeof(IBehavior<>).MakeGenericType(closingType)), string.Empty);
                        }
                        else
                        {
                            var handler = (IHandler)container.GetInstance(typeof(Handler<>).MakeGenericType(closingType));
                            Assert.NotNull(handler);
                        }
                    }
                })).ToArray();

                Task.WaitAll(tasks);
            }
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

        public interface IHandler
        {
            int BehaviorCount { get; }
        }

        public class Handler<T> : IHandler
        {
            public Handler(IEnumerable<IBehavior<T>> behaviors)
            {
                BehaviorCount = behaviors.Count();
            }

            public int BehaviorCount { get; }
        }
    }
}
