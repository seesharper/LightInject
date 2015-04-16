#load "Common.csx"

static string pathToSourceFile = @"..\LightInject.Wcf\LightInject.Wcf.cs";
private string version = GetVersionNumberFromSourceFile(pathToSourceFile);
private string nugetBuildDirectory = Execute(() => CopyToNuGetBuildDirectory(@"..\LightInject.Wcf"), "Copying project to NuGet build directory");
Console.WriteLine ("Building version " + version);

Execute(() => Build(@"..\LightInject.Wcf.sln"), "Building LightInject.Wcf");
Execute(() => RunUnitTests(), "Running unit tests");

Execute(() => PatchNugetVersionInfo(), "Patching NuGet version information");
Execute(() => PatchAssemblyInfo(), "Patching assembly version info");
Execute(() => MakeInternalTypesPublic(), "Making internal types public");
Execute(() => Build(Path.Combine(nugetBuildDirectory, "LightInject.Wcf.csproj")), "Building NuGet binary project");
Execute(() => CopyNugetBinariesToPackageLibDirectory(), "Copy NuGet binaries to lib directory");
Execute(() => RemoveDirectory(nugetBuildDirectory), string.Format("Removing NuGet build directory ({0})", nugetBuildDirectory));
Execute(() => CreateNugetBinaryPackage(), "Creating the binary NuGet package"); 

private void PatchNugetVersionInfo()
{	
	PatchNugetVersionInfo(@"LightInject.Wcf\package\LightInject.Wcf.nuspec", version);
	PatchNugetVersionInfo(@"LightInject.Wcf.Source\package\LightInject.Wcf.nuspec", version);
}

private void PatchAssemblyInfo()
{
	string pathToAssemblyInfo = Path.Combine(nugetBuildDirectory, @"properties/assemblyinfo.cs");
	string assemblyInfo = ReadFile(pathToAssemblyInfo);
	assemblyInfo = PatchAssemblyVersionInfo(version, assemblyInfo);
	assemblyInfo = RemoveInternalsVisibleToAttribute(assemblyInfo);
	WriteFile(pathToAssemblyInfo, assemblyInfo);
}

private void MakeInternalTypesPublic()
{
	string pathToNugetSourceFile = Path.Combine(nugetBuildDirectory, "LightInject.Wcf.cs");
	string source = ReadFile(pathToNugetSourceFile);
	string publicSource = Regex.Replace(source, @"(internal)(\sstatic\sclass|\sclass)", @"public$2");		
	WriteFile(pathToNugetSourceFile, publicSource);
}

private void CopyNugetBinariesToPackageLibDirectory()
{
	string pathToReleaseBuild = Path.Combine(nugetBuildDirectory, @"bin\release\");
	string pathToLibDirectory = @"LightInject.Wcf\package\lib\net45";
	RoboCopy(Path.Combine(nugetBuildDirectory, @"bin\release\"), pathToLibDirectory + " file LightInject.Wcf.*");
}

private void CreateNugetBinaryPackage()
{
	string pathToSpecification = @"LightInject.Wcf\package\LightInject.Wcf.nuspec";
	string outputDirectory = @"..\..\..\NuGet";
	CreatePackage(pathToSpecification, outputDirectory);
}

private void RunUnitTests()
{
	string pathToTestAdapter = @"C:\GitHub\LightInject\Integrations\Wcf\packages\NUnitTestAdapter.1.2\lib\";

	string pathToTestAssembly = @"..\LightInject.Wcf.Tests\bin\release\LightInject.Wcf.Tests.dll";

	MsTest.Run(pathToTestAssembly);
}