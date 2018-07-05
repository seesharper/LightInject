using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle("LightInject")]
#if NET452
    [assembly: AssemblyProduct("LightInject (NET45)")]
#endif
#if NET46
[assembly: AssemblyProduct("LightInject (NET46)")]
#endif
#if NETSTANDARD1_1
    [assembly: AssemblyProduct("LightInject (NETSTANDARD11)")]
#endif
#if NETSTANDARD1_3
    [assembly: AssemblyProduct("LightInject (NETSTANDARD13)")]
#endif
#if NETSTANDARD1_6
    [assembly: AssemblyProduct("LightInject (NETSTANDARD16)")]
#endif
#if NETCOREAPP2_0
    [assembly: AssemblyProduct("LightInject (NETCOREAPP20)")]
#endif
[assembly: AssemblyCopyright("Copyright © Bernhard Richter 2017")]


// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("4.0.0")]
[assembly: AssemblyFileVersion("4.0.0")]
[assembly: AssemblyInformationalVersion("4.0.0")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1633:FileMustHaveHeader", Justification = "Custom header.")]
[assembly: InternalsVisibleTo("LightInject.Benchmarks")]
