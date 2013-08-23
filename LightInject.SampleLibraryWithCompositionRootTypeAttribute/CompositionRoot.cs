using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightInject.SampleLibraryWithCompositionRootTypeAttribute
{
    internal class CompositionRoot : ICompositionRoot
    {
        [ThreadStatic]
        private static int callCount;

        public static int CallCount
        {
            get
            {
                return callCount;
            }
            set
            {
                callCount = value;
            }
        }

        public void Compose(IServiceRegistry serviceRegistry)
        {
            CallCount++;
            serviceRegistry.Register<IFoo,Foo>();
        }
    }
}
