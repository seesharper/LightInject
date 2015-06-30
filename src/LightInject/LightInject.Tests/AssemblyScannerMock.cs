namespace LightInject.Tests
{
	using System;
	using System.Reflection;

	using LightMock;

	internal class AssemblyScannerMock : MockContext<IAssemblyScanner>, IAssemblyScanner
	{
		private readonly IInvocationContext<IAssemblyScanner> context;
		
		public void Scan(Assembly assembly, IServiceRegistry serviceRegistry, Func<ILifetime> lifetime, Func<Type, Type, bool> shouldRegister)
		{
            ((IInvocationContext<IAssemblyScanner>)this).Invoke(m => m.Scan(assembly, serviceRegistry, lifetime, shouldRegister));            
		}

		public void Scan(Assembly assembly, IServiceRegistry serviceRegistry)
		{
            ((IInvocationContext<IAssemblyScanner>)this).Invoke(m => m.Scan(assembly, serviceRegistry));
		}
	}
}