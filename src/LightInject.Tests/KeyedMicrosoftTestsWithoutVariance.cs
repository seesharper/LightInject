namespace LightInject.Tests;

public class KeyedMicrosoftTestsWithoutVariance : KeyedMicrosoftTests
{
    internal override IServiceContainer CreateContainer()
    {
        var container = new ServiceContainer(options =>
         {
             options.AllowMultipleRegistrations = true;
             options.EnableCurrentScope = false;
             options.OptimizeForLargeObjectGraphs = false;
             options.EnableOptionalArguments = true;
             options.EnableVariance = false;
         })
        {
            AssemblyScanner = new NoOpAssemblyScanner()
        };
        container.ConstructorDependencySelector = new AnnotatedConstructorDependencySelector();
        container.ConstructorSelector = new AnnotatedConstructorSelector(container.CanGetInstance);
        return container;
    }
}