namespace LightInject.Tests
{
	using System;
	using System.Reflection;

	using LightMock;

	internal class AssemblyScannerMock : IAssemblyScanner
	{
		private readonly IInvocationContext<IAssemblyScanner> context;

		public AssemblyScannerMock(IInvocationContext<IAssemblyScanner> context)
		{
			this.context = context;
		}

		public void Scan(Assembly assembly, IServiceRegistry serviceRegistry, Func<ILifetime> lifetime, Func<Type, Type, bool> shouldRegister)
		{
			context.Invoke(m => m.Scan(assembly, serviceRegistry, lifetime, shouldRegister));
		}

		public void Scan(Assembly assembly, IServiceRegistry serviceRegistry)
		{
			context.Invoke(m => m.Scan(assembly, serviceRegistry));
		}
	}
}