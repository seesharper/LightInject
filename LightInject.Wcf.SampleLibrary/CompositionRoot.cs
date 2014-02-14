using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightInject.Wcf.SampleLibrary
{
    public class CompositionRoot : ICompositionRoot
    {
        void ICompositionRoot.Compose(IServiceRegistry serviceRegistry)
        {
            serviceRegistry.RegisterAssembly("LightInject.Wcf.SampleLibrary.Implementation.dll");
        }
    }
}
