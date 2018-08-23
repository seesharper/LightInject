using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace LightInject.Tests
{
    public class Issue396 
    {
        [Fact]
        public void ShouldHandleIssue396()
        {
            var container = new ServiceContainer();

            container.Register<Class1>();
            container.Register<Class2>();
            container.GetInstance<Class1>();

            var clonedContainer = container.Clone();
            clonedContainer.Register<Class3>();

            clonedContainer.GetInstance<Class2>();
            clonedContainer.GetInstance<Class3>();            
        }

        public class Class1
        {

        }

        public class Class2
        {

        }

        public class Class3
        {

        }
    }
}
