#load "..\NuGet\Common.csx"

Publicizer.Write("NETFX_CORE_PCL", "LightInject.cs", @"..\Build\WindowsRuntime\LightInject\LightInject.cs");
Publicizer.Write("NET", "LightInject.cs", @"..\Build\Net\LightInject\LightInject.cs"); 
Publicizer.Write("WP_PCL", "LightInject.cs", @"..\Build\WindowsPhone\LightInject\LightInject.cs"); 
