#load "Common.csx"

DirectoryUtils.DeleteAllPackages("..");

CreateSourcePackages();
CreateBinaryPackages();


private void CreateSourcePackages()
{
	CreateLightInjectSourcePackage();
	CreateLightInjectAnnotationSourcePackage();
	CreateLightInjectInterceptionSourcePackage();
	CreateLightInjectMockingSourcePackage();
	CreateLightInjectWebSourcePackage();
	CreateLightInjectMvcSourcePackage();
	CreateLightInjectServiceLocationSourcePackage();	
}

private void UpdateBinaryProjects()
{
	//LightInject
	Publicizer.Write("NETFX_CORE_PCL", @"..\..\LightInject\LightInject.cs", @"..\Build\WindowsRuntime\LightInject\LightInject.cs");
	Publicizer.Write("NET", @"..\..\LightInject\LightInject.cs", @"..\Build\Net\LightInject\LightInject.cs"); 
	Publicizer.Write("WP_PCL", @"..\..\LightInject\LightInject.cs", @"..\Build\WindowsPhone\LightInject\LightInject.cs"); 

	//LightInject.Annotation
	Publicizer.Write("NETFX_CORE_PCL",@"..\..\LightInject.Annotation\LightInject.Annotation.cs", @"..\Build\WindowsRuntime\LightInject.Annotation\LightInject.Annotation.cs");
	Publicizer.Write("NET", @"..\..\LightInject.Annotation\LightInject.Annotation.cs", @"..\Build\Net\LightInject.Annotation\LightInject.Annotation.cs");
	Publicizer.Write("WP_PCL", @"..\..\LightInject.Annotation\LightInject.Annotation.cs", @"..\Build\WindowsPhone\LightInject.Annotation\LightInject.Annotation.cs");

	//LightInject.Interception
	Publicizer.Write("NET", @"..\..\LightInject.Interception\LightInject.Interception.cs", @"..\Build\Net\LightInject.Interception\LightInject.Interception.cs");

	//LightInject.Mocking
	Publicizer.Write("NETFX_CORE_PCL", @"..\..\LightInject.Mocking\LightInject.Mocking.cs", @"..\Build\WindowsRuntime\LightInject.Mocking\LightInject.Mocking.cs");
	Publicizer.Write("NET", @"..\..\LightInject.Mocking\LightInject.Mocking.cs", @"..\Build\Net\LightInject.Mocking\LightInject.Mocking.cs");
	Publicizer.Write("WP_PCL", @"..\..\LightInject.Mocking\LightInject.Mocking.cs", @"..\Build\WindowsPhone\LightInject.Mocking\LightInject.Mocking.cs");

	Publicizer.Write("NET", @"..\..\LightInject.Mvc\LightInject.Mvc.cs", @"..\Build\Net\LightInject.Mvc\LightInject.Mvc.cs");

	//LightInject.ServiceLocation
	Publicizer.Write("NETFX_CORE_PCL", @"..\..\LightInject.ServiceLocation\LightInject.ServiceLocation.cs", @"..\Build\WindowsRuntime\LightInject.ServiceLocation\LightInject.ServiceLocation.cs");
	Publicizer.Write("NET", @"..\..\LightInject.ServiceLocation\LightInject.ServiceLocation.cs", @"..\Build\Net\LightInject.ServiceLocation\LightInject.ServiceLocation.cs");
	Publicizer.Write("WP_PCL", @"..\..\LightInject.ServiceLocation\LightInject.ServiceLocation.cs", @"..\Build\WindowsPhone\LightInject.ServiceLocation\LightInject.ServiceLocation.cs");

	//LightInject.Web
	Publicizer.Write("NET", @"..\..\LightInject.Web\LightInject.Web.cs", @"..\Build\Net\LightInject.Web\LightInject.Web.cs");
}

private void BuildBinaryProjects()
{
	MsBuild.Build(@"..\Build\Net\Net.sln"); 
	MsBuild.Build(@"..\Build\WindowsRuntime\WindowsRuntime.sln"); 
	MsBuild.Build(@"..\Build\WindowsPhone\WindowsPhone.sln"); 
}


private void CreateBinaryPackages()
{
	UpdateBinaryProjects();
	BuildBinaryProjects();
	CreateLightInjectBinaryPackage();
	CreateLightInjectAnnotationBinaryPackage();
	CreateLightInjectInterceptionBinaryPackage();
	CreateLightInjectMockingBinaryPackage();
	CreateLightInjectMvcBinaryPackage();
	CreateLightInjectWebBinaryPackage();
	CreateLightInjectServiceLocationBinaryPackage();
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

	File.Copy(@"..\Build\Net\LightInject\bin\release\LightInject.dll", @"..\LightInject\package\lib\net45\LightInject.dll", true);
	File.Copy(@"..\Build\Net\LightInject\bin\release\LightInject.pdb", @"..\LightInject\package\lib\net45\LightInject.pdb", true);
	File.Copy(@"..\Build\Net\LightInject\bin\release\LightInject.xml", @"..\LightInject\package\lib\net45\LightInject.xml", true);

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

	File.Copy(@"..\Build\Net\LightInject.Annotation\bin\release\LightInject.Annotation.dll", @"..\LightInject.Annotation\package\lib\net45\LightInject.Annotation.dll", true);
	File.Copy(@"..\Build\Net\LightInject.Annotation\bin\release\LightInject.Annotation.pdb", @"..\LightInject.Annotation\package\lib\net45\LightInject.Annotation.pdb", true);
	File.Copy(@"..\Build\Net\LightInject.Annotation\bin\release\LightInject.Annotation.xml", @"..\LightInject.Annotation\package\lib\net45\LightInject.Annotation.xml", true);

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

	File.Copy(@"..\Build\Net\LightInject.Interception\bin\release\LightInject.Interception.dll", @"..\LightInject.Interception\package\lib\net45\LightInject.Interception.dll", true);
	File.Copy(@"..\Build\Net\LightInject.Interception\bin\release\LightInject.Interception.pdb", @"..\LightInject.Interception\package\lib\net45\LightInject.Interception.pdb", true);
	File.Copy(@"..\Build\Net\LightInject.Interception\bin\release\LightInject.Interception.xml", @"..\LightInject.Interception\package\lib\net45\LightInject.Interception.xml", true);

	

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

	File.Copy(@"..\Build\Net\LightInject.Mocking\bin\release\LightInject.Mocking.dll", @"..\LightInject.Mocking\package\lib\net45\LightInject.Mocking.dll", true);
	File.Copy(@"..\Build\Net\LightInject.Mocking\bin\release\LightInject.Mocking.pdb", @"..\LightInject.Mocking\package\lib\net45\LightInject.Mocking.pdb", true);
	File.Copy(@"..\Build\Net\LightInject.Mocking\bin\release\LightInject.Mocking.xml", @"..\LightInject.Mocking\package\lib\net45\LightInject.Mocking.xml", true);

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

	File.Copy(@"..\Build\Net\LightInject.Mvc\bin\release\LightInject.Mvc.dll", @"..\LightInject.Mvc\package\lib\net45\LightInject.Mvc.dll", true);
	File.Copy(@"..\Build\Net\LightInject.Mvc\bin\release\LightInject.Mvc.pdb", @"..\LightInject.Mvc\package\lib\net45\LightInject.Mvc.pdb", true);
	File.Copy(@"..\Build\Net\LightInject.Mvc\bin\release\LightInject.Mvc.xml", @"..\LightInject.Mvc\package\lib\net45\LightInject.Mvc.xml", true);

	

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

	File.Copy(@"..\Build\Net\LightInject.Web\bin\release\LightInject.Web.dll", @"..\LightInject.Web\package\lib\net45\LightInject.Web.dll", true);
	File.Copy(@"..\Build\Net\LightInject.Web\bin\release\LightInject.Web.pdb", @"..\LightInject.Web\package\lib\net45\LightInject.Web.pdb", true);
	File.Copy(@"..\Build\Net\LightInject.Web\bin\release\LightInject.Web.xml", @"..\LightInject.Web\package\lib\net45\LightInject.Web.xml", true);

	

	NuGet.CreatePackage(@"..\NuGet.exe", @"..\LightInject.Web\package\LightInject.Web.nuspec", @"..");

	Console.WriteLine("Finished building the LightInject.Web(Binary) NuGet package");
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

	File.Copy(@"..\Build\Net\LightInject.ServiceLocation\bin\release\LightInject.ServiceLocation.dll", @"..\LightInject.ServiceLocation\package\lib\net45\LightInject.ServiceLocation.dll", true);
	File.Copy(@"..\Build\Net\LightInject.ServiceLocation\bin\release\LightInject.ServiceLocation.pdb", @"..\LightInject.ServiceLocation\package\lib\net45\LightInject.ServiceLocation.pdb", true);
	File.Copy(@"..\Build\Net\LightInject.ServiceLocation\bin\release\LightInject.ServiceLocation.xml", @"..\LightInject.ServiceLocation\package\lib\net45\LightInject.ServiceLocation.xml", true);

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
	SourceWriter.Write("NET", @"..\..\LightInject\LightInject.cs",  @"..\LightInject.Source\package\content\net45\LightInject\LightInject.cs.pp", true, true);
	SourceWriter.Write("NETFX_CORE", @"..\..\LightInject\LightInject.cs",  @"..\LightInject.Source\package\content\netcore45\LightInject\LightInject.cs.pp", true, true);
	SourceWriter.Write("WINDOWS_PHONE", @"..\..\LightInject\LightInject.cs",  @"..\LightInject.Source\package\content\windowsphone8\LightInject\LightInject.cs.pp", true, true);

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

	SourceWriter.Write("NET", @"..\..\LightInject.Annotation\LightInject.Annotation.cs",  @"..\LightInject.Annotation.Source\package\content\net40\LightInject\Annotation\LightInject.Annotation.cs.pp", true, true);
	SourceWriter.Write("NET", @"..\..\LightInject.Annotation\LightInject.Annotation.cs",  @"..\LightInject.Annotation.Source\package\content\net45\LightInject\Annotation\LightInject.Annotation.cs.pp", true, true);
	SourceWriter.Write("NETFX_CORE", @"..\..\LightInject.Annotation\LightInject.Annotation.cs",  @"..\LightInject.Annotation.Source\package\content\netcore45\LightInject\Annotation\LightInject.Annotation.cs.pp", true, true);
	
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
	SourceWriter.Write("NET", @"..\..\LightInject.Interception\LightInject.Interception.cs",  @"..\LightInject.Interception.Source\package\content\net45\LightInject\Interception\LightInject.Interception.cs.pp", true, true);
	
	
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

	SourceWriter.Write("NET", @"..\..\LightInject.Mocking\LightInject.Mocking.cs",  @"..\LightInject.Mocking.Source\package\content\net40\LightInject\Mocking\LightInject.Mocking.cs.pp", true, true);
	SourceWriter.Write("NET", @"..\..\LightInject.Mocking\LightInject.Mocking.cs",  @"..\LightInject.Mocking.Source\package\content\net45\LightInject\Mocking\LightInject.Mocking.cs.pp", true, true);
	SourceWriter.Write("NETFX_CORE", @"..\..\LightInject.Mocking\LightInject.Mocking.cs",  @"..\LightInject.Mocking.Source\package\content\netcore45\LightInject\Mocking\LightInject.Mocking.cs.pp", true, true);
	
	
	
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
	SourceWriter.Write("NET", @"..\..\LightInject.Web\LightInject.Web.cs",  @"..\LightInject.Web.Source\package\content\net45\LightInject\Web\LightInject.Web.cs.pp", true, true);
	
	
	NuGet.CreatePackage(@"..\NuGet.exe", @"..\LightInject.Web.Source\package\LightInject.Web.nuspec", @"..");

	Console.WriteLine("Finished building the LightInject.Web.Source NuGet package");
}


private void CreateLightInjectMvcSourcePackage()
{
	Console.WriteLine("Start building the LightInject.Mvc.Source NuGet package");
	
	DirectoryUtils.Delete(@"..\LightInject.Mvc.Source\package\content");
	Directory.CreateDirectory(@"..\LightInject.Mvc.Source\package\content\net40\LightInject\Mvc");
	Directory.CreateDirectory(@"..\LightInject.Mvc.Source\package\content\net45\LightInject\Mvc");
	

	SourceWriter.Write("NET", @"..\..\LightInject.Mvc\LightInject.Mvc.cs",  @"..\LightInject.Mvc.Source\package\content\net40\LightInject\Mvc\LightInject.Mvc.cs.pp", true, true);
	SourceWriter.Write("NET", @"..\..\LightInject.Mvc\LightInject.Mvc.cs",  @"..\LightInject.Mvc.Source\package\content\net45\LightInject\Mvc\LightInject.Mvc.cs.pp", true, true);
	
	
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

	SourceWriter.Write("NET", @"..\..\LightInject.ServiceLocation\LightInject.ServiceLocation.cs",  @"..\LightInject.ServiceLocation.Source\package\content\net40\LightInject\ServiceLocation\LightInject.ServiceLocation.cs.pp", true, true);
	SourceWriter.Write("NET", @"..\..\LightInject.ServiceLocation\LightInject.ServiceLocation.cs",  @"..\LightInject.ServiceLocation.Source\package\content\net45\LightInject\ServiceLocation\LightInject.ServiceLocation.cs.pp", true, true);
	SourceWriter.Write("NETFX_CORE", @"..\..\LightInject.ServiceLocation\LightInject.ServiceLocation.cs",  @"..\LightInject.ServiceLocation.Source\package\content\netcore45\LightInject\ServiceLocation\LightInject.ServiceLocation.cs.pp", true, true);
	
	
	
	NuGet.CreatePackage(@"..\NuGet.exe", @"..\LightInject.ServiceLocation.Source\package\LightInject.ServiceLocation.nuspec", @"..");

	Console.WriteLine("Finished building the LightInject.ServiceLocation.Source NuGet package");
}
