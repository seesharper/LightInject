using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("LightInject")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Microsoft")]
[assembly: AssemblyProduct("LightInject")]
[assembly: AssemblyCopyright("Copyright © Bernhard Richter")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("29c44396-1968-43b7-b95f-32afe2a6da08")]

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
[assembly: AssemblyVersion("2.0.0.0")]
[assembly: AssemblyFileVersion("2.0.0.0")]
[assembly: InternalsVisibleTo("LightInject.Tests, PublicKey=00240000048000009400000006020000002400005253413100040000010001009b14e2cb0ad516f90766c1a4d793a02d1f8fcbbaef57250028f98c241e6344d4b2a104ee20da7d6ef6a2fff9987b9def6b1e88a8744340c90a2880bf0194897b255a1fd20ebc70b833f770942000bcefee3122aadf8947cb37c6951a292c5146692ecc60df6c7b8ce629aed50b8f3c1c597132ae505bbea0cfea4c5b7a2f45b8")]
[assembly: InternalsVisibleTo("LightInject.SampleLibrary, PublicKey=00240000048000009400000006020000002400005253413100040000010001009b14e2cb0ad516f90766c1a4d793a02d1f8fcbbaef57250028f98c241e6344d4b2a104ee20da7d6ef6a2fff9987b9def6b1e88a8744340c90a2880bf0194897b255a1fd20ebc70b833f770942000bcefee3122aadf8947cb37c6951a292c5146692ecc60df6c7b8ce629aed50b8f3c1c597132ae505bbea0cfea4c5b7a2f45b8")]
[assembly: InternalsVisibleTo("LightInject.Web, PublicKey=00240000048000009400000006020000002400005253413100040000010001009b14e2cb0ad516f90766c1a4d793a02d1f8fcbbaef57250028f98c241e6344d4b2a104ee20da7d6ef6a2fff9987b9def6b1e88a8744340c90a2880bf0194897b255a1fd20ebc70b833f770942000bcefee3122aadf8947cb37c6951a292c5146692ecc60df6c7b8ce629aed50b8f3c1c597132ae505bbea0cfea4c5b7a2f45b8")]
[assembly: InternalsVisibleTo("LightInject.SampleLibraryWithCompositionRoot, PublicKey=00240000048000009400000006020000002400005253413100040000010001009b14e2cb0ad516f90766c1a4d793a02d1f8fcbbaef57250028f98c241e6344d4b2a104ee20da7d6ef6a2fff9987b9def6b1e88a8744340c90a2880bf0194897b255a1fd20ebc70b833f770942000bcefee3122aadf8947cb37c6951a292c5146692ecc60df6c7b8ce629aed50b8f3c1c597132ae505bbea0cfea4c5b7a2f45b8")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2, PublicKey=0024000004800000940000000602000000240000525341310004000001000100c547cac37abd99c8db225ef2f6c8a3602f3b3606cc9891605d02baa56104f4cfc0734aa39b93bf7852f7d9266654753cc297e7d2edfe0bac1cdcf9f717241550e0a7b191195b7667bb4f64bcb8e2121380fd1d9d46ad2d92d2d15605093924cceaf74c4861eff62abf69b9291ed0a340e113be11e6a7d3113e92484cf7045cc7")]