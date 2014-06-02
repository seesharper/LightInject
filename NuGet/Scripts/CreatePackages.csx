#load "Common.csx"
DirectoryUtils.DeleteAllPackages("..");

EnsureBuildToolsAreUpToDate();

BuildMainSolution();
//RunUnitTestsForMainSolution();
//AssertTestCoverageIs100PercentForMainSolution();

//RunUnitTestsForInterception();
//AssertTestCoverageIs100PercentForInterception();

//RunUnitTestsForSignalR();
//AssertTestCoverageIs100PercentForSignalR(); 

BuildNugetBinaries();

//RunUnitTestsForNet4NuGetPackage();
//AssertTestCoverageIs100PercentForNet4NuGetPackage();

RunUnitTestsForNet45NuGetPackage();
//AssertTestCoverageIs100PercentForNet45NuGetPackage();



/*Console.WriteLine("***Start creating NuGet packages***");

DirectoryUtils.DeleteAllPackages("..");


CreateBinaryPackages();
CreateSourcePackages();

Console.WriteLine("***Finished creating NuGet packages***"); */

private void EnsureBuildToolsAreUpToDate()
{
	Console.WriteLine("Building CoverageToXml");
	MsBuild.Build(@"..\CoverageToXml\CoverageToXml.sln");
}

private void BuildMainSolution()
{
	Console.WriteLine("Building LightInject");
	MsBuild.Build(@"..\..\..\LightInject\LightInject.sln"); 
}

private void RunUnitTestsForMainSolution()
{
	Console.WriteLine("Running tests (LightInject)");
	MsTest.Run(@"..\..\..\LightInject\LightInject.Tests\bin\release\LightInject.Tests.dll");
}

private void AssertTestCoverageIs100PercentForMainSolution()
{
	Console.WriteLine("Running tests with code coverage (LightInject)");
	MsTest.RunWithCodeCoverage(@"..\..\..\LightInject\LightInject.Tests\bin\release\LightInject.Tests.dll", "lightinject.dll");
}

private void RunUnitTestsForInterception()
{
	Console.WriteLine("Running tests (LightInject.Interception)");
	MsTest.Run(@"..\..\..\LightInject\LightInject.Interception.Tests\bin\release\LightInject.Interception.Tests.dll");	
}

private void AssertTestCoverageIs100PercentForInterception()
{
	Console.WriteLine("Running tests with coverage (LightInject.Interception)");
	MsTest.RunWithCodeCoverage(@"..\..\..\LightInject\LightInject.Interception.Tests\bin\release\LightInject.Interception.Tests.dll", "lightinject.interception.dll");	
}

private void RunUnitTestsForSignalR()
{
	Console.WriteLine("Running tests (LightInject.SignalR)");
	MsTest.Run(@"..\..\..\LightInject\LightInject.SignalR.Tests\bin\release\LightInject.SignalR.Tests.dll");	
}

private void AssertTestCoverageIs100PercentForSignalR()
{
	Console.WriteLine("Running tests with coverage (LightInject.SignalR)");
	MsTest.RunWithCodeCoverage(@"..\..\..\LightInject\LightInject.SignalR.Tests\bin\release\LightInject.SignalR.Tests.dll", "lightinject.signalr.dll");	
}

private void RunUnitTestsForNet4NuGetPackage()
{
	Console.WriteLine("Running tests (LightInject NuGet NET)");
	MsTest.Run(@"..\Build\Net\LightInject.Tests\bin\release\LightInject.Tests.dll");
}

private void AssertTestCoverageIs100PercentForNet4NuGetPackage()
{
	Console.WriteLine("Running tests with coverage (LightInject NuGet NET)");
	MsTest.RunWithCodeCoverage(@"..\Build\Net\LightInject.Tests\bin\release\LightInject.Tests.dll","lightinject.dll");
}

private void RunUnitTestsForNet45NuGetPackage()
{
	Console.WriteLine("Running tests (LightInject NuGet NET45)");
	MsTest.Run(@"..\Build\Net45\LightInject.Tests\bin\release\LightInject.Tests.dll");

	Console.WriteLine("Running tests (LightInject.Interception NuGet NET45)");
	MsTest.Run(@"..\Build\Net45\LightInject.Interception.Tests\bin\release\LightInject.Interception.Tests.dll");

	Console.WriteLine("Running tests (LightInject.SignalR NuGet NET45)");
	MsTest.Run(@"..\Build\Net45\LightInject.SignalR.Tests\bin\release\LightInject.SignalR.Tests.dll");
}

private void AssertTestCoverageIs100PercentForNet45NuGetPackage()
{
	Console.WriteLine("Running tests with coverage (LightInject NuGet NET45)");
	MsTest.RunWithCodeCoverage(@"..\Build\Net45\LightInject.Tests\bin\release\LightInject.Tests.dll","lightinject.dll");
}

private void CreateSourcePackages()
{
	CreateLightInjectSourcePackage();
	CreateLightInjectAnnotationSourcePackage();
	CreateLightInjectInterceptionSourcePackage();
	CreateLightInjectMockingSourcePackage();
	CreateLightInjectWebSourcePackage();
	CreateLightInjectMvcSourcePackage();
	CreateLightInjectServiceLocationSourcePackage();	
	CreateLightInjectWcfSourcePackage();
	CreateLightInjectWebApiSourcePackage();
	CreateLightInjectSignalRSourcePackage();

}

private void BuildNugetBinaries()
{
	UpdateNuGetBuildProjects();
	BuildBinaryProjects();	
}


private void UpdateNuGetBuildProjects()
{
	//LightInject
	string version = VersionUtils.GetVersionString(@"..\..\LightInject\LightInject.cs");		
	VersionUtils.UpdateAssemblyInfo(@"..\Build\Net\LightInject\Properties\AssemblyInfo.cs", version);
	VersionUtils.UpdateAssemblyInfo(@"..\Build\Net45\LightInject\Properties\AssemblyInfo.cs", version);
	VersionUtils.UpdateAssemblyInfo(@"..\Build\WindowsRuntime\LightInject\Properties\AssemblyInfo.cs", version);
	VersionUtils.UpdateAssemblyInfo(@"..\Build\WindowsPhone\LightInject\Properties\AssemblyInfo.cs", version);
	VersionUtils.UpdateNuGetPackageSpecification(@"..\LightInject\package\LightInject.nuspec", version);
	VersionUtils.UpdateNuGetPackageSpecification(@"..\LightInject.Source\package\LightInject.nuspec", version);

	Publicizer.Write("NETFX_CORE", @"..\..\LightInject\LightInject.cs", @"..\Build\WindowsRuntime\LightInject\LightInject.cs");
	Publicizer.Write("NET", @"..\..\LightInject\LightInject.cs", @"..\Build\Net\LightInject\LightInject.cs"); 
	Publicizer.Write("NET45", @"..\..\LightInject\LightInject.cs", @"..\Build\Net45\LightInject\LightInject.cs"); 
	Publicizer.Write("WINDOWS_PHONE", @"..\..\LightInject\LightInject.cs", @"..\Build\WindowsPhone\LightInject\LightInject.cs"); 

	//LightInject.Annotation
	version = VersionUtils.GetVersionString(@"..\..\LightInject.Annotation\LightInject.Annotation.cs");		
	VersionUtils.UpdateAssemblyInfo(@"..\Build\Net\LightInject.Annotation\Properties\AssemblyInfo.cs", version);
	VersionUtils.UpdateAssemblyInfo(@"..\Build\Net45\LightInject.Annotation\Properties\AssemblyInfo.cs", version);
	VersionUtils.UpdateAssemblyInfo(@"..\Build\WindowsRuntime\LightInject.Annotation\Properties\AssemblyInfo.cs", version);
	VersionUtils.UpdateAssemblyInfo(@"..\Build\WindowsPhone\LightInject.Annotation\Properties\AssemblyInfo.cs", version);

	VersionUtils.UpdateNuGetPackageSpecification(@"..\LightInject.Annotation\package\LightInject.Annotation.nuspec", version);
	VersionUtils.UpdateNuGetPackageSpecification(@"..\LightInject.Annotation.Source\package\LightInject.Annotation.nuspec", version);

	Publicizer.Write("NETFX_CORE",@"..\..\LightInject.Annotation\LightInject.Annotation.cs", @"..\Build\WindowsRuntime\LightInject.Annotation\LightInject.Annotation.cs");
	Publicizer.Write("NET", @"..\..\LightInject.Annotation\LightInject.Annotation.cs", @"..\Build\Net\LightInject.Annotation\LightInject.Annotation.cs");
	Publicizer.Write("NET45", @"..\..\LightInject.Annotation\LightInject.Annotation.cs", @"..\Build\Net45\LightInject.Annotation\LightInject.Annotation.cs");
	Publicizer.Write("WINDOWS_PHONE", @"..\..\LightInject.Annotation\LightInject.Annotation.cs", @"..\Build\WindowsPhone\LightInject.Annotation\LightInject.Annotation.cs");

	//LightInject.Interception
	version = VersionUtils.GetVersionString(@"..\..\LightInject.Interception\LightInject.Interception.cs");		
	VersionUtils.UpdateAssemblyInfo(@"..\Build\Net\LightInject.Interception\Properties\AssemblyInfo.cs", version);
	VersionUtils.UpdateAssemblyInfo(@"..\Build\Net45\LightInject.Interception\Properties\AssemblyInfo.cs", version);
	VersionUtils.UpdateNuGetPackageSpecification(@"..\LightInject.Interception\package\LightInject.Interception.nuspec", version);
	VersionUtils.UpdateNuGetPackageSpecification(@"..\LightInject.Interception.Source\package\LightInject.Interception.nuspec", version);


	Publicizer.Write("NET", @"..\..\LightInject.Interception\LightInject.Interception.cs", @"..\Build\Net\LightInject.Interception\LightInject.Interception.cs");
	Publicizer.Write("NET45", @"..\..\LightInject.Interception\LightInject.Interception.cs", @"..\Build\Net45\LightInject.Interception\LightInject.Interception.cs");

	//LightInject.Mocking
	
	version = VersionUtils.GetVersionString(@"..\..\LightInject.Mocking\LightInject.Mocking.cs");		
	VersionUtils.UpdateAssemblyInfo(@"..\Build\Net\LightInject.Mocking\Properties\AssemblyInfo.cs", version);
	VersionUtils.UpdateAssemblyInfo(@"..\Build\Net45\LightInject.Mocking\Properties\AssemblyInfo.cs", version);
	VersionUtils.UpdateAssemblyInfo(@"..\Build\WindowsRuntime\LightInject.Mocking\Properties\AssemblyInfo.cs", version);
	VersionUtils.UpdateAssemblyInfo(@"..\Build\WindowsPhone\LightInject.Mocking\Properties\AssemblyInfo.cs", version);

	VersionUtils.UpdateNuGetPackageSpecification(@"..\LightInject.Mocking\package\LightInject.Mocking.nuspec", version);
	VersionUtils.UpdateNuGetPackageSpecification(@"..\LightInject.Mocking.Source\package\LightInject.Mocking.nuspec", version);

	Publicizer.Write("NETFX_CORE", @"..\..\LightInject.Mocking\LightInject.Mocking.cs", @"..\Build\WindowsRuntime\LightInject.Mocking\LightInject.Mocking.cs");
	Publicizer.Write("NET", @"..\..\LightInject.Mocking\LightInject.Mocking.cs", @"..\Build\Net\LightInject.Mocking\LightInject.Mocking.cs");
	Publicizer.Write("NET45", @"..\..\LightInject.Mocking\LightInject.Mocking.cs", @"..\Build\Net45\LightInject.Mocking\LightInject.Mocking.cs");
	Publicizer.Write("WINDOWS_PHONE", @"..\..\LightInject.Mocking\LightInject.Mocking.cs", @"..\Build\WindowsPhone\LightInject.Mocking\LightInject.Mocking.cs");
	
	//LightInject.Mvc
	version = VersionUtils.GetVersionString(@"..\..\LightInject.Mvc\LightInject.Mvc.cs");		
	VersionUtils.UpdateAssemblyInfo(@"..\Build\Net\LightInject.Mvc\Properties\AssemblyInfo.cs", version);
	VersionUtils.UpdateAssemblyInfo(@"..\Build\Net45\LightInject.Mvc\Properties\AssemblyInfo.cs", version);
	
	Publicizer.Write("NET", @"..\..\LightInject.Mvc\LightInject.Mvc.cs", @"..\Build\Net\LightInject.Mvc\LightInject.Mvc.cs");
	Publicizer.Write("NET45", @"..\..\LightInject.Mvc\LightInject.Mvc.cs", @"..\Build\Net45\LightInject.Mvc\LightInject.Mvc.cs");

	VersionUtils.UpdateNuGetPackageSpecification(@"..\LightInject.Mvc\package\LightInject.Mvc.nuspec", version);
	VersionUtils.UpdateNuGetPackageSpecification(@"..\LightInject.Mvc.Source\package\LightInject.Mvc.nuspec", version);

	//LightInject.ServiceLocation
	version = VersionUtils.GetVersionString(@"..\..\LightInject.ServiceLocation\LightInject.ServiceLocation.cs");		
	VersionUtils.UpdateAssemblyInfo(@"..\Build\Net\LightInject.ServiceLocation\Properties\AssemblyInfo.cs", version);
	VersionUtils.UpdateAssemblyInfo(@"..\Build\Net45\LightInject.ServiceLocation\Properties\AssemblyInfo.cs", version);
	VersionUtils.UpdateAssemblyInfo(@"..\Build\WindowsRuntime\LightInject.ServiceLocation\Properties\AssemblyInfo.cs", version);
	VersionUtils.UpdateAssemblyInfo(@"..\Build\WindowsPhone\LightInject.ServiceLocation\Properties\AssemblyInfo.cs", version);

	VersionUtils.UpdateNuGetPackageSpecification(@"..\LightInject.ServiceLocation\package\LightInject.ServiceLocation.nuspec", version);
	VersionUtils.UpdateNuGetPackageSpecification(@"..\LightInject.ServiceLocation.Source\package\LightInject.ServiceLocation.nuspec", version);

	Publicizer.Write("NETFX_CORE", @"..\..\LightInject.ServiceLocation\LightInject.ServiceLocation.cs", @"..\Build\WindowsRuntime\LightInject.ServiceLocation\LightInject.ServiceLocation.cs");
	Publicizer.Write("NET", @"..\..\LightInject.ServiceLocation\LightInject.ServiceLocation.cs", @"..\Build\Net\LightInject.ServiceLocation\LightInject.ServiceLocation.cs");
	Publicizer.Write("NET45", @"..\..\LightInject.ServiceLocation\LightInject.ServiceLocation.cs", @"..\Build\Net45\LightInject.ServiceLocation\LightInject.ServiceLocation.cs");
	Publicizer.Write("WINDOWS_PHONE", @"..\..\LightInject.ServiceLocation\LightInject.ServiceLocation.cs", @"..\Build\WindowsPhone\LightInject.ServiceLocation\LightInject.ServiceLocation.cs");

	//LightInject.Web	
	version = VersionUtils.GetVersionString(@"..\..\LightInject.Web\LightInject.Web.cs");		
	VersionUtils.UpdateAssemblyInfo(@"..\Build\Net\LightInject.Web\Properties\AssemblyInfo.cs", version);
	VersionUtils.UpdateAssemblyInfo(@"..\Build\Net45\LightInject.Web\Properties\AssemblyInfo.cs", version);

	Publicizer.Write("NET", @"..\..\LightInject.Web\LightInject.Web.cs", @"..\Build\Net\LightInject.Web\LightInject.Web.cs");
	Publicizer.Write("NET45", @"..\..\LightInject.Web\LightInject.Web.cs", @"..\Build\Net45\LightInject.Web\LightInject.Web.cs");

	VersionUtils.UpdateNuGetPackageSpecification(@"..\LightInject.Web\package\LightInject.Web.nuspec", version);
	VersionUtils.UpdateNuGetPackageSpecification(@"..\LightInject.Web.Source\package\LightInject.Web.nuspec", version);

	//LightInject.Wcf
	version = VersionUtils.GetVersionString(@"..\..\LightInject.Wcf\LightInject.Wcf.cs");		
	VersionUtils.UpdateAssemblyInfo(@"..\Build\Net\LightInject.Wcf\Properties\AssemblyInfo.cs", version);
	VersionUtils.UpdateAssemblyInfo(@"..\Build\Net45\LightInject.Wcf\Properties\AssemblyInfo.cs", version);
	
	Publicizer.Write("NET", @"..\..\LightInject.Wcf\LightInject.Wcf.cs", @"..\Build\Net\LightInject.Wcf\LightInject.Wcf.cs");
	Publicizer.Write("NET", @"..\..\LightInject.Wcf\LightInject.Wcf.cs", @"..\Build\Net45\LightInject.Wcf\LightInject.Wcf.cs");

	VersionUtils.UpdateNuGetPackageSpecification(@"..\LightInject.Wcf\package\LightInject.Wcf.nuspec", version);
	VersionUtils.UpdateNuGetPackageSpecification(@"..\LightInject.Wcf.Source\package\LightInject.Wcf.nuspec", version);

	//LightInject.Wcf.Client
	/*version = VersionUtils.GetVersionString(@"..\..\LightInject.Wcf.Client\LightInject.Wcf.Client.cs");		
	VersionUtils.UpdateAssemblyInfo(@"..\Build\Net\LightInject.Wcf.Client\Properties\AssemblyInfo.cs", version);

	Publicizer.Write("NETFX_CORE_PCL",@"..\..\LightInject.Wcf.Client\LightInject.Wcf.Client.cs", @"..\Build\WindowsRuntime\LightInject.Wcf.Client\LightInject.Wcf.Client.cs");
	Publicizer.Write("NET", @"..\..\LightInject.Wcf.Client\LightInject.Wcf.Client.cs", @"..\Build\Net\LightInject.Wcf.Client\LightInject.Wcf.Client.cs");
	Publicizer.Write("WP_PCL", @"..\..\LightInject.Wcf.Client\LightInject.Wcf.Client.cs", @"..\Build\WindowsPhone\LightInject.Wcf.Client\LightInject.Wcf.Client.cs");

	VersionUtils.UpdateNuGetPackageSpecification(@"..\LightInject.Wcf.Client\package\LightInject.Wcf.Client.nuspec", version);
	VersionUtils.UpdateNuGetPackageSpecification(@"..\LightInject.Wcf..ClientSource\package\LightInject.Wcf.Client.nuspec", version); */

	//LightInject.WebApi
	version = VersionUtils.GetVersionString(@"..\..\LightInject.WebApi\LightInject.WebApi.cs");		
	VersionUtils.UpdateAssemblyInfo(@"..\Build\Net\LightInject.WebApi\Properties\AssemblyInfo.cs", version);
	VersionUtils.UpdateAssemblyInfo(@"..\Build\Net45\LightInject.WebApi\Properties\AssemblyInfo.cs", version);
	
	Publicizer.Write("NET", @"..\..\LightInject.WebApi\LightInject.WebApi.cs", @"..\Build\Net\LightInject.WebApi\LightInject.WebApi.cs");
	Publicizer.Write("NET45", @"..\..\LightInject.WebApi\LightInject.WebApi.cs", @"..\Build\Net45\LightInject.WebApi\LightInject.WebApi.cs");

	VersionUtils.UpdateNuGetPackageSpecification(@"..\LightInject.WebApi\package\LightInject.WebApi.nuspec", version);
	VersionUtils.UpdateNuGetPackageSpecification(@"..\LightInject.WebApi.Source\package\LightInject.WebApi.nuspec", version);

	//LightInject.SignalR
	version = VersionUtils.GetVersionString(@"..\..\LightInject.SignalR\LightInject.SignalR.cs");			
	VersionUtils.UpdateAssemblyInfo(@"..\Build\Net45\LightInject.SignalR\Properties\AssemblyInfo.cs", version);
		
	Publicizer.Write("NET45", @"..\..\LightInject.SignalR\LightInject.SignalR.cs", @"..\Build\Net45\LightInject.SignalR\LightInject.SignalR.cs");

	VersionUtils.UpdateNuGetPackageSpecification(@"..\LightInject.SignalR\package\LightInject.SignalR.nuspec", version);
	VersionUtils.UpdateNuGetPackageSpecification(@"..\LightInject.SignalR.Source\package\LightInject.SignalR.nuspec", version);
}


private void BuildBinaryProjects()
{
	MsBuild.Build(@"..\Build\Net\Net.sln"); 
	MsBuild.Build(@"..\Build\Net45\Net45.sln"); 
	MsBuild.Build(@"..\Build\WindowsRuntime\WindowsRuntime.sln"); 
	MsBuild.Build(@"..\Build\WindowsPhone\WindowsPhone.sln"); 
}


private void CreateBinaryPackages()
{	
	BuildBinaryProjects();
	CreateLightInjectBinaryPackage();
	CreateLightInjectAnnotationBinaryPackage();
	CreateLightInjectInterceptionBinaryPackage();
	CreateLightInjectMockingBinaryPackage();
	CreateLightInjectMvcBinaryPackage();
	CreateLightInjectWebBinaryPackage();
	CreateLightInjectServiceLocationBinaryPackage(); 
	CreateLightInjectWcfBinaryPackage();
	CreateLightInjectWebApiBinaryPackage();
	CreateLightInjectSignalRBinaryPackage();
}

private void CreateLightInjectBinaryPackage()
{
	Console.WriteLine("Start building the LightInject(Binary) NuGet package");

	DirectoryUtils.Delete(@"..\LightInject\package\lib");
	Directory.CreateDirectory(@"..\LightInject\package\lib\net40");
	Directory.CreateDirectory(@"..\LightInject\package\lib\net45");
	Directory.CreateDirectory(@"..\LightInject\package\lib\netcore45");
	Directory.CreateDirectory(@"..\LightInject\package\lib\windowsphone8");


	File.Copy(@"..\Build\Net\LightInject\bin\release\LightInject.dll", @"..\LightInject\package\lib\net40\LightInject.dll", true);
	File.Copy(@"..\Build\Net\LightInject\bin\release\LightInject.pdb", @"..\LightInject\package\lib\net40\LightInject.pdb", true);
	File.Copy(@"..\Build\Net\LightInject\bin\release\LightInject.xml", @"..\LightInject\package\lib\net40\LightInject.xml", true);

	File.Copy(@"..\Build\Net45\LightInject\bin\release\LightInject.dll", @"..\LightInject\package\lib\net45\LightInject.dll", true);
	File.Copy(@"..\Build\Net45\LightInject\bin\release\LightInject.pdb", @"..\LightInject\package\lib\net45\LightInject.pdb", true);
	File.Copy(@"..\Build\Net45\LightInject\bin\release\LightInject.xml", @"..\LightInject\package\lib\net45\LightInject.xml", true);

	File.Copy(@"..\Build\WindowsRuntime\LightInject\bin\release\LightInject.dll", @"..\LightInject\package\lib\netcore45\LightInject.dll", true);
	File.Copy(@"..\Build\WindowsRuntime\LightInject\bin\release\LightInject.pdb", @"..\LightInject\package\lib\netcore45\LightInject.pdb", true);
	File.Copy(@"..\Build\WindowsRuntime\LightInject\bin\release\LightInject.xml", @"..\LightInject\package\lib\netcore45\LightInject.xml", true);

	File.Copy(@"..\Build\WindowsPhone\LightInject\bin\release\LightInject.dll", @"..\LightInject\package\lib\windowsphone8\LightInject.dll", true);
	File.Copy(@"..\Build\WindowsPhone\LightInject\bin\release\LightInject.pdb", @"..\LightInject\package\lib\windowsphone8\LightInject.pdb", true);
	File.Copy(@"..\Build\WindowsPhone\LightInject\bin\release\LightInject.xml", @"..\LightInject\package\lib\windowsphone8\LightInject.xml", true);

	NuGet.CreatePackage(@"..\NuGet.exe", @"..\LightInject\package\LightInject.nuspec", @"..");

	Console.WriteLine("Finished building the LightInject(Binary) NuGet package");

}

private void CreateLightInjectAnnotationBinaryPackage()
{
	Console.WriteLine("Start building the LightInject.Annotation(Binary) NuGet package");

	DirectoryUtils.Delete(@"..\LightInject.Annotation\package\lib");
	Directory.CreateDirectory(@"..\LightInject.Annotation\package\lib\net40");
	Directory.CreateDirectory(@"..\LightInject.Annotation\package\lib\net45");
	Directory.CreateDirectory(@"..\LightInject.Annotation\package\lib\netcore45");
	Directory.CreateDirectory(@"..\LightInject.Annotation\package\lib\windowsphone8");

	File.Copy(@"..\Build\Net\LightInject.Annotation\bin\release\LightInject.Annotation.dll", @"..\LightInject.Annotation\package\lib\net40\LightInject.Annotation.dll", true);
	File.Copy(@"..\Build\Net\LightInject.Annotation\bin\release\LightInject.Annotation.pdb", @"..\LightInject.Annotation\package\lib\net40\LightInject.Annotation.pdb", true);
	File.Copy(@"..\Build\Net\LightInject.Annotation\bin\release\LightInject.Annotation.xml", @"..\LightInject.Annotation\package\lib\net40\LightInject.Annotation.xml", true);

	File.Copy(@"..\Build\Net45\LightInject.Annotation\bin\release\LightInject.Annotation.dll", @"..\LightInject.Annotation\package\lib\net45\LightInject.Annotation.dll", true);
	File.Copy(@"..\Build\Net45\LightInject.Annotation\bin\release\LightInject.Annotation.pdb", @"..\LightInject.Annotation\package\lib\net45\LightInject.Annotation.pdb", true);
	File.Copy(@"..\Build\Net45\LightInject.Annotation\bin\release\LightInject.Annotation.xml", @"..\LightInject.Annotation\package\lib\net45\LightInject.Annotation.xml", true);

	File.Copy(@"..\Build\WindowsRuntime\LightInject.Annotation\bin\release\LightInject.Annotation.dll", @"..\LightInject.Annotation\package\lib\netcore45\LightInject.Annotation.dll", true);
	File.Copy(@"..\Build\WindowsRuntime\LightInject.Annotation\bin\release\LightInject.Annotation.pdb", @"..\LightInject.Annotation\package\lib\netcore45\LightInject.Annotation.pdb", true);
	File.Copy(@"..\Build\WindowsRuntime\LightInject.Annotation\bin\release\LightInject.Annotation.xml", @"..\LightInject.Annotation\package\lib\netcore45\LightInject.Annotation.xml", true);

	File.Copy(@"..\Build\WindowsPhone\LightInject.Annotation\bin\release\LightInject.Annotation.dll", @"..\LightInject.Annotation\package\lib\windowsphone8\LightInject.Annotation.dll", true);
	File.Copy(@"..\Build\WindowsPhone\LightInject.Annotation\bin\release\LightInject.Annotation.pdb", @"..\LightInject.Annotation\package\lib\windowsphone8\LightInject.Annotation.pdb", true);
	File.Copy(@"..\Build\WindowsPhone\LightInject.Annotation\bin\release\LightInject.Annotation.xml", @"..\LightInject.Annotation\package\lib\windowsphone8\LightInject.Annotation.xml", true);


	NuGet.CreatePackage(@"..\NuGet.exe", @"..\LightInject.Annotation\package\LightInject.Annotation.nuspec", @"..");

	Console.WriteLine("Finished building the LightInject.Annotation(Binary) NuGet package");
}

private void CreateLightInjectInterceptionBinaryPackage()
{
	Console.WriteLine("Start building the LightInject.Interception(Binary) NuGet package");

	DirectoryUtils.Delete(@"..\LightInject.Interception\package\lib");
	Directory.CreateDirectory(@"..\LightInject.Interception\package\lib\net40");
	Directory.CreateDirectory(@"..\LightInject.Interception\package\lib\net45");

	File.Copy(@"..\Build\Net\LightInject.Interception\bin\release\LightInject.Interception.dll", @"..\LightInject.Interception\package\lib\net40\LightInject.Interception.dll", true);
	File.Copy(@"..\Build\Net\LightInject.Interception\bin\release\LightInject.Interception.pdb", @"..\LightInject.Interception\package\lib\net40\LightInject.Interception.pdb", true);
	File.Copy(@"..\Build\Net\LightInject.Interception\bin\release\LightInject.Interception.xml", @"..\LightInject.Interception\package\lib\net40\LightInject.Interception.xml", true);

	File.Copy(@"..\Build\Net45\LightInject.Interception\bin\release\LightInject.Interception.dll", @"..\LightInject.Interception\package\lib\net45\LightInject.Interception.dll", true);
	File.Copy(@"..\Build\Net45\LightInject.Interception\bin\release\LightInject.Interception.pdb", @"..\LightInject.Interception\package\lib\net45\LightInject.Interception.pdb", true);
	File.Copy(@"..\Build\Net45\LightInject.Interception\bin\release\LightInject.Interception.xml", @"..\LightInject.Interception\package\lib\net45\LightInject.Interception.xml", true);

	

	NuGet.CreatePackage(@"..\NuGet.exe", @"..\LightInject.Interception\package\LightInject.Interception.nuspec", @"..");

	Console.WriteLine("Finished building the LightInject.Interception(Binary) NuGet package");
}


private void CreateLightInjectMockingBinaryPackage()
{
	Console.WriteLine("Start building the LightInject.Mocking(Binary) NuGet package");

	DirectoryUtils.Delete(@"..\LightInject.Mocking\package\lib");
	Directory.CreateDirectory(@"..\LightInject.Mocking\package\lib\net40");
	Directory.CreateDirectory(@"..\LightInject.Mocking\package\lib\net45");
	Directory.CreateDirectory(@"..\LightInject.Mocking\package\lib\netcore45");
	Directory.CreateDirectory(@"..\LightInject.Mocking\package\lib\windowsphone8");


	File.Copy(@"..\Build\Net\LightInject.Mocking\bin\release\LightInject.Mocking.dll", @"..\LightInject.Mocking\package\lib\net40\LightInject.Mocking.dll", true);
	File.Copy(@"..\Build\Net\LightInject.Mocking\bin\release\LightInject.Mocking.pdb", @"..\LightInject.Mocking\package\lib\net40\LightInject.Mocking.pdb", true);
	File.Copy(@"..\Build\Net\LightInject.Mocking\bin\release\LightInject.Mocking.xml", @"..\LightInject.Mocking\package\lib\net40\LightInject.Mocking.xml", true);

	File.Copy(@"..\Build\Net45\LightInject.Mocking\bin\release\LightInject.Mocking.dll", @"..\LightInject.Mocking\package\lib\net45\LightInject.Mocking.dll", true);
	File.Copy(@"..\Build\Net45\LightInject.Mocking\bin\release\LightInject.Mocking.pdb", @"..\LightInject.Mocking\package\lib\net45\LightInject.Mocking.pdb", true);
	File.Copy(@"..\Build\Net45\LightInject.Mocking\bin\release\LightInject.Mocking.xml", @"..\LightInject.Mocking\package\lib\net45\LightInject.Mocking.xml", true);

	File.Copy(@"..\Build\WindowsRuntime\LightInject.Mocking\bin\release\LightInject.Mocking.dll", @"..\LightInject.Mocking\package\lib\netcore45\LightInject.Mocking.dll", true);
	File.Copy(@"..\Build\WindowsRuntime\LightInject.Mocking\bin\release\LightInject.Mocking.pdb", @"..\LightInject.Mocking\package\lib\netcore45\LightInject.Mocking.pdb", true);
	File.Copy(@"..\Build\WindowsRuntime\LightInject.Mocking\bin\release\LightInject.Mocking.xml", @"..\LightInject.Mocking\package\lib\netcore45\LightInject.Mocking.xml", true);

	File.Copy(@"..\Build\WindowsPhone\LightInject.Mocking\bin\release\LightInject.Mocking.dll", @"..\LightInject.Mocking\package\lib\windowsphone8\LightInject.Mocking.dll", true);
	File.Copy(@"..\Build\WindowsPhone\LightInject.Mocking\bin\release\LightInject.Mocking.pdb", @"..\LightInject.Mocking\package\lib\windowsphone8\LightInject.Mocking.pdb", true);
	File.Copy(@"..\Build\WindowsPhone\LightInject.Mocking\bin\release\LightInject.Mocking.xml", @"..\LightInject.Mocking\package\lib\windowsphone8\LightInject.Mocking.xml", true);

	NuGet.CreatePackage(@"..\NuGet.exe", @"..\LightInject.Mocking\package\LightInject.Mocking.nuspec", @"..");

	Console.WriteLine("Finished building the LightInject.Mocking(Binary) NuGet package");

}

private void CreateLightInjectMvcBinaryPackage()
{
	Console.WriteLine("Start building the LightInject.Mvc(Binary) NuGet package");

	DirectoryUtils.Delete(@"..\LightInject.Mvc\package\lib");
	Directory.CreateDirectory(@"..\LightInject.Mvc\package\lib\net40");
	Directory.CreateDirectory(@"..\LightInject.Mvc\package\lib\net45");

	File.Copy(@"..\Build\Net\LightInject.Mvc\bin\release\LightInject.Mvc.dll", @"..\LightInject.Mvc\package\lib\net40\LightInject.Mvc.dll", true);
	File.Copy(@"..\Build\Net\LightInject.Mvc\bin\release\LightInject.Mvc.pdb", @"..\LightInject.Mvc\package\lib\net40\LightInject.Mvc.pdb", true);
	File.Copy(@"..\Build\Net\LightInject.Mvc\bin\release\LightInject.Mvc.xml", @"..\LightInject.Mvc\package\lib\net40\LightInject.Mvc.xml", true);

	File.Copy(@"..\Build\Net45\LightInject.Mvc\bin\release\LightInject.Mvc.dll", @"..\LightInject.Mvc\package\lib\net45\LightInject.Mvc.dll", true);
	File.Copy(@"..\Build\Net45\LightInject.Mvc\bin\release\LightInject.Mvc.pdb", @"..\LightInject.Mvc\package\lib\net45\LightInject.Mvc.pdb", true);
	File.Copy(@"..\Build\Net45\LightInject.Mvc\bin\release\LightInject.Mvc.xml", @"..\LightInject.Mvc\package\lib\net45\LightInject.Mvc.xml", true);

	

	NuGet.CreatePackage(@"..\NuGet.exe", @"..\LightInject.Mvc\package\LightInject.Mvc.nuspec", @"..");

	Console.WriteLine("Finished building the LightInject.Mvc(Binary) NuGet package");
}

private void CreateLightInjectWebBinaryPackage()
{
	Console.WriteLine("Start building the LightInject.Web(Binary) NuGet package");

	DirectoryUtils.Delete(@"..\LightInject.Web\package\lib");
	Directory.CreateDirectory(@"..\LightInject.Web\package\lib\net40");
	Directory.CreateDirectory(@"..\LightInject.Web\package\lib\net45");

	File.Copy(@"..\Build\Net\LightInject.Web\bin\release\LightInject.Web.dll", @"..\LightInject.Web\package\lib\net40\LightInject.Web.dll", true);
	File.Copy(@"..\Build\Net\LightInject.Web\bin\release\LightInject.Web.pdb", @"..\LightInject.Web\package\lib\net40\LightInject.Web.pdb", true);
	File.Copy(@"..\Build\Net\LightInject.Web\bin\release\LightInject.Web.xml", @"..\LightInject.Web\package\lib\net40\LightInject.Web.xml", true);

	File.Copy(@"..\Build\Net45\LightInject.Web\bin\release\LightInject.Web.dll", @"..\LightInject.Web\package\lib\net45\LightInject.Web.dll", true);
	File.Copy(@"..\Build\Net45\LightInject.Web\bin\release\LightInject.Web.pdb", @"..\LightInject.Web\package\lib\net45\LightInject.Web.pdb", true);
	File.Copy(@"..\Build\Net45\LightInject.Web\bin\release\LightInject.Web.xml", @"..\LightInject.Web\package\lib\net45\LightInject.Web.xml", true);

	

	NuGet.CreatePackage(@"..\NuGet.exe", @"..\LightInject.Web\package\LightInject.Web.nuspec", @"..");

	Console.WriteLine("Finished building the LightInject.Web(Binary) NuGet package");
}

private void CreateLightInjectWcfBinaryPackage()
{
	Console.WriteLine("Start building the LightInject.Wcf(Binary) NuGet package");

	DirectoryUtils.Delete(@"..\LightInject.Wcf\package\lib");
	Directory.CreateDirectory(@"..\LightInject.Wcf\package\lib\net40");
	Directory.CreateDirectory(@"..\LightInject.Wcf\package\lib\net45");

	File.Copy(@"..\Build\Net\LightInject.Wcf\bin\release\LightInject.Wcf.dll", @"..\LightInject.Wcf\package\lib\net40\LightInject.Wcf.dll", true);
	File.Copy(@"..\Build\Net\LightInject.Wcf\bin\release\LightInject.Wcf.pdb", @"..\LightInject.Wcf\package\lib\net40\LightInject.Wcf.pdb", true);
	File.Copy(@"..\Build\Net\LightInject.Wcf\bin\release\LightInject.Wcf.xml", @"..\LightInject.Wcf\package\lib\net40\LightInject.Wcf.xml", true);

	File.Copy(@"..\Build\Net45\LightInject.Wcf\bin\release\LightInject.Wcf.dll", @"..\LightInject.Wcf\package\lib\net45\LightInject.Wcf.dll", true);
	File.Copy(@"..\Build\Net45\LightInject.Wcf\bin\release\LightInject.Wcf.pdb", @"..\LightInject.Wcf\package\lib\net45\LightInject.Wcf.pdb", true);
	File.Copy(@"..\Build\Net45\LightInject.Wcf\bin\release\LightInject.Wcf.xml", @"..\LightInject.Wcf\package\lib\net45\LightInject.Wcf.xml", true);
	
	NuGet.CreatePackage(@"..\NuGet.exe", @"..\LightInject.Wcf\package\LightInject.Wcf.nuspec", @"..");

	Console.WriteLine("Finished building the LightInject.Wcf(Binary) NuGet package");
}

private void CreateLightInjectWebApiBinaryPackage()
{
	Console.WriteLine("Start building the LightInject.WebApi(Binary) NuGet package");

	DirectoryUtils.Delete(@"..\LightInject.WebApi\package\lib");
	Directory.CreateDirectory(@"..\LightInject.WebApi\package\lib\net40");
	Directory.CreateDirectory(@"..\LightInject.WebApi\package\lib\net45");

	File.Copy(@"..\Build\Net\LightInject.WebApi\bin\release\LightInject.WebApi.dll", @"..\LightInject.WebApi\package\lib\net40\LightInject.WebApi.dll", true);
	File.Copy(@"..\Build\Net\LightInject.WebApi\bin\release\LightInject.WebApi.pdb", @"..\LightInject.WebApi\package\lib\net40\LightInject.WebApi.pdb", true);
	File.Copy(@"..\Build\Net\LightInject.WebApi\bin\release\LightInject.WebApi.xml", @"..\LightInject.WebApi\package\lib\net40\LightInject.WebApi.xml", true);

	File.Copy(@"..\Build\Net45\LightInject.WebApi\bin\release\LightInject.WebApi.dll", @"..\LightInject.WebApi\package\lib\net45\LightInject.WebApi.dll", true);
	File.Copy(@"..\Build\Net45\LightInject.WebApi\bin\release\LightInject.WebApi.pdb", @"..\LightInject.WebApi\package\lib\net45\LightInject.WebApi.pdb", true);
	File.Copy(@"..\Build\Net45\LightInject.WebApi\bin\release\LightInject.WebApi.xml", @"..\LightInject.WebApi\package\lib\net45\LightInject.WebApi.xml", true);
	
	NuGet.CreatePackage(@"..\NuGet.exe", @"..\LightInject.WebApi\package\LightInject.WebApi.nuspec", @"..");

	Console.WriteLine("Finished building the LightInject.WebApi(Binary) NuGet package");
}

private void CreateLightInjectSignalRBinaryPackage()
{
	Console.WriteLine("Start building the LightInject.SignalR(Binary) NuGet package");

	DirectoryUtils.Delete(@"..\LightInject.SignalR\package\lib");	
	Directory.CreateDirectory(@"..\LightInject.SignalR\package\lib\net45");

	File.Copy(@"..\Build\Net45\LightInject.SignalR\bin\release\LightInject.SignalR.dll", @"..\LightInject.SignalR\package\lib\net45\LightInject.SignalR.dll", true);
	File.Copy(@"..\Build\Net45\LightInject.SignalR\bin\release\LightInject.SignalR.pdb", @"..\LightInject.SignalR\package\lib\net45\LightInject.SignalR.pdb", true);
	File.Copy(@"..\Build\Net45\LightInject.SignalR\bin\release\LightInject.SignalR.xml", @"..\LightInject.SignalR\package\lib\net45\LightInject.SignalR.xml", true);
	
	NuGet.CreatePackage(@"..\NuGet.exe", @"..\LightInject.SignalR\package\LightInject.SignalR.nuspec", @"..");

	Console.WriteLine("Finished building the LightInject.SignalR(Binary) NuGet package");
}


private void CreateLightInjectServiceLocationBinaryPackage()
{
	Console.WriteLine("Start building the LightInject.ServiceLocation(Binary) NuGet package");

	DirectoryUtils.Delete(@"..\LightInject.ServiceLocation\package\lib");
	Directory.CreateDirectory(@"..\LightInject.ServiceLocation\package\lib\net40");
	Directory.CreateDirectory(@"..\LightInject.ServiceLocation\package\lib\net45");
	Directory.CreateDirectory(@"..\LightInject.ServiceLocation\package\lib\netcore45");
	Directory.CreateDirectory(@"..\LightInject.ServiceLocation\package\lib\windowsphone8");


	File.Copy(@"..\Build\Net\LightInject.ServiceLocation\bin\release\LightInject.ServiceLocation.dll", @"..\LightInject.ServiceLocation\package\lib\net40\LightInject.ServiceLocation.dll", true);
	File.Copy(@"..\Build\Net\LightInject.ServiceLocation\bin\release\LightInject.ServiceLocation.pdb", @"..\LightInject.ServiceLocation\package\lib\net40\LightInject.ServiceLocation.pdb", true);
	File.Copy(@"..\Build\Net\LightInject.ServiceLocation\bin\release\LightInject.ServiceLocation.xml", @"..\LightInject.ServiceLocation\package\lib\net40\LightInject.ServiceLocation.xml", true);

	File.Copy(@"..\Build\Net45\LightInject.ServiceLocation\bin\release\LightInject.ServiceLocation.dll", @"..\LightInject.ServiceLocation\package\lib\net45\LightInject.ServiceLocation.dll", true);
	File.Copy(@"..\Build\Net45\LightInject.ServiceLocation\bin\release\LightInject.ServiceLocation.pdb", @"..\LightInject.ServiceLocation\package\lib\net45\LightInject.ServiceLocation.pdb", true);
	File.Copy(@"..\Build\Net45\LightInject.ServiceLocation\bin\release\LightInject.ServiceLocation.xml", @"..\LightInject.ServiceLocation\package\lib\net45\LightInject.ServiceLocation.xml", true);

	File.Copy(@"..\Build\WindowsRuntime\LightInject.ServiceLocation\bin\release\LightInject.ServiceLocation.dll", @"..\LightInject.ServiceLocation\package\lib\netcore45\LightInject.ServiceLocation.dll", true);
	File.Copy(@"..\Build\WindowsRuntime\LightInject.ServiceLocation\bin\release\LightInject.ServiceLocation.pdb", @"..\LightInject.ServiceLocation\package\lib\netcore45\LightInject.ServiceLocation.pdb", true);
	File.Copy(@"..\Build\WindowsRuntime\LightInject.ServiceLocation\bin\release\LightInject.ServiceLocation.xml", @"..\LightInject.ServiceLocation\package\lib\netcore45\LightInject.ServiceLocation.xml", true);

	File.Copy(@"..\Build\WindowsPhone\LightInject.ServiceLocation\bin\release\LightInject.ServiceLocation.dll", @"..\LightInject.ServiceLocation\package\lib\windowsphone8\LightInject.ServiceLocation.dll", true);
	File.Copy(@"..\Build\WindowsPhone\LightInject.ServiceLocation\bin\release\LightInject.ServiceLocation.pdb", @"..\LightInject.ServiceLocation\package\lib\windowsphone8\LightInject.ServiceLocation.pdb", true);
	File.Copy(@"..\Build\WindowsPhone\LightInject.ServiceLocation\bin\release\LightInject.ServiceLocation.xml", @"..\LightInject.ServiceLocation\package\lib\windowsphone8\LightInject.ServiceLocation.xml", true);

	NuGet.CreatePackage(@"..\NuGet.exe", @"..\LightInject.ServiceLocation\package\LightInject.ServiceLocation.nuspec", @"..");

	Console.WriteLine("Finished building the LightInject.ServiceLocation(Binary) NuGet package");

}

private void CopyLightInjectToContentFolder()
{
	File.Copy(@"..\Build\Net\LightInject.Annotation\bin\release\LightInject.Annotation.dll", @"..\LightInject.Binary\package\lib\net45\LightInject.Annotation.dll", true);
	File.Copy(@"..\Build\Net\LightInject.Annotation\bin\release\LightInject.Annotation.pdb", @"..\LightInject.Binary\package\lib\net45\LightInject.Annotation.pdb", true);
	File.Copy(@"..\Build\Net\LightInject.Annotation\bin\release\LightInject.Annotation.xml", @"..\LightInject.Binary\package\lib\net45\LightInject.Annotation.xml", true);
}



private void CopyLightInjectToLibFolder()
{
	
}


private void CreateLightInjectSourcePackage()
{
	Console.WriteLine("Start building the LightInject.Source NuGet package");
	
	DirectoryUtils.Delete(@"..\LightInject.Source\package\content");
	Directory.CreateDirectory(@"..\LightInject.Source\package\content\net40\LightInject");
	Directory.CreateDirectory(@"..\LightInject.Source\package\content\net45\LightInject");
	Directory.CreateDirectory(@"..\LightInject.Source\package\content\netcore45\LightInject");
	Directory.CreateDirectory(@"..\LightInject.Source\package\content\windowsphone8\LightInject");

	SourceWriter.Write("NET", @"..\..\LightInject\LightInject.cs",  @"..\LightInject.Source\package\content\net40\LightInject\LightInject.cs.pp", true, true);
	SourceWriter.Write("NET45", @"..\..\LightInject\LightInject.cs",  @"..\LightInject.Source\package\content\net45\LightInject\LightInject.cs.pp", true, true);
	SourceWriter.Write("NETFX_CORE", @"..\..\LightInject\LightInject.cs",  @"..\LightInject.Source\package\content\netcore45\LightInject\LightInject.cs.pp", true, false);
	SourceWriter.Write("WINDOWS_PHONE", @"..\..\LightInject\LightInject.cs",  @"..\LightInject.Source\package\content\windowsphone8\LightInject\LightInject.cs.pp", true, false);

	NuGet.CreatePackage(@"..\NuGet.exe", @"..\LightInject.Source\package\LightInject.nuspec", @"..");

	Console.WriteLine("Finished building the LightInject.Source NuGet package");
}

private void CreateLightInjectAnnotationSourcePackage()
{
	Console.WriteLine("Start building the LightInject.Annotation.Source NuGet package");
	
	DirectoryUtils.Delete(@"..\LightInject.Annotation.Source\package\content");
	Directory.CreateDirectory(@"..\LightInject.Annotation.Source\package\content\net40\LightInject\Annotation");
	Directory.CreateDirectory(@"..\LightInject.Annotation.Source\package\content\net45\LightInject\Annotation");
	Directory.CreateDirectory(@"..\LightInject.Annotation.Source\package\content\netcore45\LightInject\Annotation");
	Directory.CreateDirectory(@"..\LightInject.Annotation.Source\package\content\windowsphone8\LightInject\Annotation");

	SourceWriter.Write("NET", @"..\..\LightInject.Annotation\LightInject.Annotation.cs",  @"..\LightInject.Annotation.Source\package\content\net40\LightInject\Annotation\LightInject.Annotation.cs.pp", true, true);
	SourceWriter.Write("NET45", @"..\..\LightInject.Annotation\LightInject.Annotation.cs",  @"..\LightInject.Annotation.Source\package\content\net45\LightInject\Annotation\LightInject.Annotation.cs.pp", true, true);
	SourceWriter.Write("NETFX_CORE", @"..\..\LightInject.Annotation\LightInject.Annotation.cs",  @"..\LightInject.Annotation.Source\package\content\netcore45\LightInject\Annotation\LightInject.Annotation.cs.pp", true, false);
	SourceWriter.Write("WINDOWS_PHONE", @"..\..\LightInject.Annotation\LightInject.Annotation.cs",  @"..\LightInject.Annotation.Source\package\content\windowsphone8\LightInject\Annotation\LightInject.Annotation.cs.pp", true, false);
	
	NuGet.CreatePackage(@"..\NuGet.exe", @"..\LightInject.Annotation.Source\package\LightInject.Annotation.nuspec", @"..");

	Console.WriteLine("Finished building the LightInject.Annotation.Source NuGet package");
}

private void CreateLightInjectInterceptionSourcePackage()
{
	Console.WriteLine("Start building the LightInject.Interception.Source NuGet package");
	
	DirectoryUtils.Delete(@"..\LightInject.Interception.Source\package\content");
	Directory.CreateDirectory(@"..\LightInject.Interception.Source\package\content\net40\LightInject\Interception");
	Directory.CreateDirectory(@"..\LightInject.Interception.Source\package\content\net45\LightInject\Interception");
	

	SourceWriter.Write("NET", @"..\..\LightInject.Interception\LightInject.Interception.cs",  @"..\LightInject.Interception.Source\package\content\net40\LightInject\Interception\LightInject.Interception.cs.pp", true, true);
	SourceWriter.Write("NET45", @"..\..\LightInject.Interception\LightInject.Interception.cs",  @"..\LightInject.Interception.Source\package\content\net45\LightInject\Interception\LightInject.Interception.cs.pp", true, true);
	
	
	NuGet.CreatePackage(@"..\NuGet.exe", @"..\LightInject.Interception.Source\package\LightInject.Interception.nuspec", @"..");

	Console.WriteLine("Finished building the LightInject.Interception.Source NuGet package");
}

private void CreateLightInjectMockingSourcePackage()
{
	Console.WriteLine("Start building the LightInject.Mocking.Source NuGet package");
	
	DirectoryUtils.Delete(@"..\LightInject.Mocking.Source\package\content");
	Directory.CreateDirectory(@"..\LightInject.Mocking.Source\package\content\net40\LightInject\Mocking");
	Directory.CreateDirectory(@"..\LightInject.Mocking.Source\package\content\net45\LightInject\Mocking");
	Directory.CreateDirectory(@"..\LightInject.Mocking.Source\package\content\netcore45\LightInject\Mocking");
	Directory.CreateDirectory(@"..\LightInject.Mocking.Source\package\content\windowsphone8\LightInject\Mocking");

	SourceWriter.Write("NET", @"..\..\LightInject.Mocking\LightInject.Mocking.cs",  @"..\LightInject.Mocking.Source\package\content\net40\LightInject\Mocking\LightInject.Mocking.cs.pp", true, true);
	SourceWriter.Write("NET45", @"..\..\LightInject.Mocking\LightInject.Mocking.cs",  @"..\LightInject.Mocking.Source\package\content\net45\LightInject\Mocking\LightInject.Mocking.cs.pp", true, true);
	SourceWriter.Write("NETFX_CORE", @"..\..\LightInject.Mocking\LightInject.Mocking.cs",  @"..\LightInject.Mocking.Source\package\content\netcore45\LightInject\Mocking\LightInject.Mocking.cs.pp", true, false);
	SourceWriter.Write("WINDOWS_PHONE", @"..\..\LightInject.Mocking\LightInject.Mocking.cs",  @"..\LightInject.Mocking.Source\package\content\windowsphone8\LightInject\Mocking\LightInject.Mocking.cs.pp", true, false);
	
	
	NuGet.CreatePackage(@"..\NuGet.exe", @"..\LightInject.Mocking.Source\package\LightInject.Mocking.nuspec", @"..");

	Console.WriteLine("Finished building the LightInject.Mocking.Source NuGet package");
}

private void CreateLightInjectWebSourcePackage()
{
	Console.WriteLine("Start building the LightInject.Web.Source NuGet package");
	
	DirectoryUtils.Delete(@"..\LightInject.Web.Source\package\content");
	Directory.CreateDirectory(@"..\LightInject.Web.Source\package\content\net40\LightInject\Web");
	Directory.CreateDirectory(@"..\LightInject.Web.Source\package\content\net45\LightInject\Web");
	

	SourceWriter.Write("NET", @"..\..\LightInject.Web\LightInject.Web.cs",  @"..\LightInject.Web.Source\package\content\net40\LightInject\Web\LightInject.Web.cs.pp", true, true);
	SourceWriter.Write("NET45", @"..\..\LightInject.Web\LightInject.Web.cs",  @"..\LightInject.Web.Source\package\content\net45\LightInject\Web\LightInject.Web.cs.pp", true, true);
	
	
	NuGet.CreatePackage(@"..\NuGet.exe", @"..\LightInject.Web.Source\package\LightInject.Web.nuspec", @"..");

	Console.WriteLine("Finished building the LightInject.Web.Source NuGet package");
}

private void CreateLightInjectWcfSourcePackage()
{
	Console.WriteLine("Start building the LightInject.Wcf.Source NuGet package");
	
	DirectoryUtils.Delete(@"..\LightInject.Wcf.Source\package\content");
	Directory.CreateDirectory(@"..\LightInject.Wcf.Source\package\content\net40\LightInject\Wcf");
	Directory.CreateDirectory(@"..\LightInject.Wcf.Source\package\content\net45\LightInject\Wcf");
	

	SourceWriter.Write("NET", @"..\..\LightInject.Wcf\LightInject.Wcf.cs",  @"..\LightInject.Wcf.Source\package\content\net40\LightInject\Wcf\LightInject.Wcf.cs.pp", true, true);
	SourceWriter.Write("NET45", @"..\..\LightInject.Wcf\LightInject.Wcf.cs",  @"..\LightInject.Wcf.Source\package\content\net45\LightInject\Wcf\LightInject.Wcf.cs.pp", true, true);
	
	
	NuGet.CreatePackage(@"..\NuGet.exe", @"..\LightInject.Wcf.Source\package\LightInject.Wcf.nuspec", @"..");

	Console.WriteLine("Finished building the LightInject.Wcf.Source NuGet package");
}

private void CreateLightInjectWebApiSourcePackage()
{
	Console.WriteLine("Start building the LightInject.WebApi.Source NuGet package");
	
	DirectoryUtils.Delete(@"..\LightInject.WebApi.Source\package\content");
	Directory.CreateDirectory(@"..\LightInject.WebApi.Source\package\content\net40\LightInject\WebApi");
	Directory.CreateDirectory(@"..\LightInject.WebApi.Source\package\content\net45\LightInject\WebApi");
	

	SourceWriter.Write("NET", @"..\..\LightInject.WebApi\LightInject.WebApi.cs",  @"..\LightInject.WebApi.Source\package\content\net40\LightInject\WebApi\LightInject.WebApi.cs.pp", true, true);
	SourceWriter.Write("NET45", @"..\..\LightInject.WebApi\LightInject.WebApi.cs",  @"..\LightInject.WebApi.Source\package\content\net45\LightInject\WebApi\LightInject.WebApi.cs.pp", true, true);
	
	
	NuGet.CreatePackage(@"..\NuGet.exe", @"..\LightInject.WebApi.Source\package\LightInject.WebApi.nuspec", @"..");

	Console.WriteLine("Finished building the LightInject.WebApi.Source NuGet package");
}

private void CreateLightInjectSignalRSourcePackage()
{
	Console.WriteLine("Start building the LightInject.SignalR.Source NuGet package");
	
	DirectoryUtils.Delete(@"..\LightInject.SignalR.Source\package\content");
	Directory.CreateDirectory(@"..\LightInject.SignalR.Source\package\content\net40\LightInject\SignalR");
	Directory.CreateDirectory(@"..\LightInject.SignalR.Source\package\content\net45\LightInject\SignalR");
	
	SourceWriter.Write("NET45", @"..\..\LightInject.SignalR\LightInject.SignalR.cs",  @"..\LightInject.SignalR.Source\package\content\net45\LightInject\SignalR\LightInject.SignalR.cs.pp", true, true);
	
	
	NuGet.CreatePackage(@"..\NuGet.exe", @"..\LightInject.SignalR.Source\package\LightInject.SignalR.nuspec", @"..");

	Console.WriteLine("Finished building the LightInject.SignalR.Source NuGet package");
}

private void CreateLightInjectMvcSourcePackage()
{
	Console.WriteLine("Start building the LightInject.Mvc.Source NuGet package");
	
	DirectoryUtils.Delete(@"..\LightInject.Mvc.Source\package\content");
	Directory.CreateDirectory(@"..\LightInject.Mvc.Source\package\content\net40\LightInject\Mvc");
	Directory.CreateDirectory(@"..\LightInject.Mvc.Source\package\content\net45\LightInject\Mvc");
	

	SourceWriter.Write("NET", @"..\..\LightInject.Mvc\LightInject.Mvc.cs",  @"..\LightInject.Mvc.Source\package\content\net40\LightInject\Mvc\LightInject.Mvc.cs.pp", true, true);
	SourceWriter.Write("NET45", @"..\..\LightInject.Mvc\LightInject.Mvc.cs",  @"..\LightInject.Mvc.Source\package\content\net45\LightInject\Mvc\LightInject.Mvc.cs.pp", true, true);
	
	
	NuGet.CreatePackage(@"..\NuGet.exe", @"..\LightInject.Mvc.Source\package\LightInject.Mvc.nuspec", @"..");

	Console.WriteLine("Finished building the LightInject.Mvc.Source NuGet package");
}

private void CreateLightInjectServiceLocationSourcePackage()
{
	Console.WriteLine("Start building the LightInject.ServiceLocation.Source NuGet package");
	
	DirectoryUtils.Delete(@"..\LightInject.ServiceLocation.Source\package\content");
	Directory.CreateDirectory(@"..\LightInject.ServiceLocation.Source\package\content\net40\LightInject\ServiceLocation");
	Directory.CreateDirectory(@"..\LightInject.ServiceLocation.Source\package\content\net45\LightInject\ServiceLocation");
	Directory.CreateDirectory(@"..\LightInject.ServiceLocation.Source\package\content\netcore45\LightInject\ServiceLocation");
	Directory.CreateDirectory(@"..\LightInject.ServiceLocation.Source\package\content\windowsphone8\LightInject\ServiceLocation");

	SourceWriter.Write("NET", @"..\..\LightInject.ServiceLocation\LightInject.ServiceLocation.cs",  @"..\LightInject.ServiceLocation.Source\package\content\net40\LightInject\ServiceLocation\LightInject.ServiceLocation.cs.pp", true, true);
	SourceWriter.Write("NET45", @"..\..\LightInject.ServiceLocation\LightInject.ServiceLocation.cs",  @"..\LightInject.ServiceLocation.Source\package\content\net45\LightInject\ServiceLocation\LightInject.ServiceLocation.cs.pp", true, true);
	SourceWriter.Write("NETFX_CORE", @"..\..\LightInject.ServiceLocation\LightInject.ServiceLocation.cs",  @"..\LightInject.ServiceLocation.Source\package\content\netcore45\LightInject\ServiceLocation\LightInject.ServiceLocation.cs.pp", true, false);
	SourceWriter.Write("WINDOWS_PHONE", @"..\..\LightInject.ServiceLocation\LightInject.ServiceLocation.cs",  @"..\LightInject.ServiceLocation.Source\package\content\windowsphone8\LightInject\ServiceLocation\LightInject.ServiceLocation.cs.pp", true, false);
	
	
	NuGet.CreatePackage(@"..\NuGet.exe", @"..\LightInject.ServiceLocation.Source\package\LightInject.ServiceLocation.nuspec", @"..");

	Console.WriteLine("Finished building the LightInject.ServiceLocation.Source NuGet package");
}
