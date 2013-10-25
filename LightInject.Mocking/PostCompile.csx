#load "..\NuGet\Common.csx"

Publicizer.Write("NETFX_CORE_PCL", "LightInject.Mocking.cs", @"..\Build\WindowsRuntime\LightInject.Mocking\LightInject.Mocking.cs");
Publicizer.Write("NET", "LightInject.Mocking.cs", @"..\Build\Net\LightInject.Mocking\LightInject.Mocking.cs");
Publicizer.Write("WP_PCL", "LightInject.Mocking.cs", @"..\Build\WindowsPhone\LightInject.Mocking\LightInject.Mocking.cs");