namespace LightInject.Tests
{
	using System;
	using System.Reflection;

	using LightMock;

	internal class AssemblyScannerMock : MockContext<IAssemblyScanner>, IAssemblyScanner
	{				
		public void Scan(Assembly assembly, IServiceRegistry serviceRegistry, Func<ILifetime> lifetime, Func<Type, Type, bool> shouldRegister, Func<Type, Type, string> serviceNameProvider)
		{
            ((IInvocationContext<IAssemblyScanner>)this).Invoke(m => m.Scan(assembly, serviceRegistry, lifetime, shouldRegister, serviceNameProvider));            
		}

		public void Scan(Assembly assembly, IServiceRegistry serviceRegistry)
		{
            ((IInvocationContext<IAssemblyScanner>)this).Invoke(m => m.Scan(assembly, serviceRegistry));
		}
	}
}