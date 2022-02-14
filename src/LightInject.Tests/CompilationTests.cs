using LightInject.SampleLibrary;
using System.Collections.Generic;
using Xunit;

namespace LightInject.Tests
{
    public class CompilationTests
    {
        [Fact]
        public void ShouldCompileServices()
        {
            var options = new ContainerOptions();
            var log = new List<LogEntry>();
            options.LogFactory = (type) => (e) => log.Add(e);
            var container = new ServiceContainer(options);

            container.Register<IBar, Bar>();
            container.Register<IFoo, FooWithDependency>();
            container.Compile();

            Assert.Contains(log, e => e.Level == LogLevel.Info && e.Message.Matches("Compiling delegate.*IBar"));
            Assert.Contains(log, e => e.Level == LogLevel.Info && e.Message.Matches("Compiling delegate.*IFoo"));

            log.Clear();
            container.GetInstance<IFoo>();
            Assert.DoesNotContain(log, e => e.Level == LogLevel.Info && e.Message.Matches("Compiling delegate.*IBar"));
            Assert.DoesNotContain(log, e => e.Level == LogLevel.Info && e.Message.Matches("Compiling delegate.*IFoo"));
        }

        [Fact]
        public void ShouldLogWarningWhenCompileOpenGenericService()
        {
            var options = new ContainerOptions();
            var log = new List<LogEntry>();
            options.LogFactory = (type) => (e) => log.Add(e);
            var container = new ServiceContainer(options);

            container.Register(typeof(OpenGenericFoo<,>));
            container.Compile();

            Assert.Contains(log, e => e.Level == LogLevel.Warning && e.Message.Matches("Unable to precompile.*OpenGenericFoo"));
        }

        [Fact]
        public void ShouldCompileOpenGenericService()
        {
            var options = new ContainerOptions();
            var log = new List<LogEntry>();
            options.LogFactory = (type) => (e) => log.Add(e);
            var container = new ServiceContainer(options);

            container.Register(typeof(OpenGenericFoo<,>));

            container.Compile<OpenGenericFoo<string, int>>();

            log.Clear();

            container.GetInstance<OpenGenericFoo<string, int>>();

            Assert.DoesNotContain(log, e => e.Level == LogLevel.Info && e.Message.Matches("Compiling delegate.*OpenGenericFoo"));
        }

        [Fact]
        public void ShouldCompileNamedService()
        {
            var options = new ContainerOptions();
            var log = new List<LogEntry>();
            options.LogFactory = (type) => (e) => log.Add(e);
            var container = new ServiceContainer(options);

            container.Register<IFoo, Foo>("SomeFoo");
            container.Compile();

            Assert.Contains(log, e => e.Level == LogLevel.Info && e.Message.Matches("Compiling delegate.*IFoo.*SomeFoo"));

            log.Clear();
            container.GetInstance<IFoo>();
            Assert.DoesNotContain(log, e => e.Level == LogLevel.Info && e.Message.Matches("Compiling delegate.*IFoo.*SomeFoo"));
        }


        [Fact]
        public void ShouldCompileNamedOpenGenericService()
        {
            var options = new ContainerOptions();
            var log = new List<LogEntry>();
            options.LogFactory = (type) => (e) => log.Add(e);
            var container = new ServiceContainer(options);

            container.Register(typeof(OpenGenericFoo<,>), typeof(OpenGenericFoo<,>), "OpenGenericFoo");

            container.Compile<OpenGenericFoo<string, int>>("OpenGenericFoo");

            log.Clear();

            container.GetInstance<OpenGenericFoo<string, int>>("OpenGenericFoo");

            Assert.DoesNotContain(log, e => e.Level == LogLevel.Info && e.Message.Matches("Compiling delegate.*OpenGenericFoo"));
        }


        [Fact]
        public void ShouldCompileServicesAccordingToPredicate()
        {
            var options = new ContainerOptions();
            var log = new List<LogEntry>();
            options.LogFactory = (type) => (e) => log.Add(e);
            var container = new ServiceContainer(options);

            container.Register<IBar, Bar>();
            container.Register<IFoo, FooWithDependency>();
            container.Compile(sr => sr.ServiceType == typeof(IBar));

            Assert.Contains(log, e => e.Level == LogLevel.Info && e.Message.Matches("Compiling delegate.*IBar"));
            Assert.DoesNotContain(log, e => e.Level == LogLevel.Info && e.Message.Matches("Compiling delegate.*IFoo"));
        }
    }
}
