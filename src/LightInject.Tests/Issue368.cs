using System;
using System.Collections.Generic;
using System.Threading;
using LightInject;
using Xunit;

namespace LightInject.Tests
{
    public class Issue368 : TestBase
    {
        [Fact]
        public void ShouldCallDisposeOnlyOnceForSameInstance()
        {                        
            var servicecontainer = new ServiceContainer();

            using (servicecontainer.BeginScope())
            {
                servicecontainer.Register(sf => new TransientServiceWithInterface(), new PerRequestLifeTime());
                servicecontainer.Register<ISomeService>(sf => sf.GetInstance<TransientServiceWithInterface>(), new PerRequestLifeTime());

                using (servicecontainer.BeginScope())
                {
                    servicecontainer.GetInstance<ISomeService>();
                }

                Assert.Equal(1, TransientServiceWithInterface.instanceCounter); // 1
                Assert.Equal(1, TransientServiceWithInterface.disposalCounters); // 2 !
            }
        }

        interface ISomeService
        {

        }

        class TransientServiceWithInterface : IDisposable, ISomeService
        {
            public TransientServiceWithInterface()
            {
                Interlocked.Increment(ref instanceCounter);
            }

            public static int instanceCounter;
            public static int disposalCounters;
            public void Dispose()
            {
                Interlocked.Increment(ref disposalCounters);

            }
            
            public override bool Equals(object obj)
            {
                // We do this to ensure that we use reference equals for tracking instances
                return false;
            }
        }
    }

}