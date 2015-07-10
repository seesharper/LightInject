
#load "common.csx"

string pathToSolutionFile = @"..\..\LightInject.sln";
string pathToSourceFile = @"..\..\LightInject\LightInject.cs";
string pathToNugetBuildDirectory = @"..\tmp\";
private string version = GetVersionNumberFromSourceFile(pathToSourceFile);

WriteLine("LightInject version {0}" , version);

Execute(() => RestoreNuGetPackages(), "NuGet");
Execute(() => InitializNugetBuildDirectory(), "Preparing Nuget build directory");
//Execute(() => PatchAssemblyInfo(), "Patching Nuget Assembly Info");
//Execute(() => BuildAllFrameworks(), "Building");

//Execute(() => MsBuild.Build(pathToSolutionFile), "Building LightInject");
//Execute(() => RunAllUnitTests(), "Running unit tests");
//Execute(() => BuildNugetProject("Net40", version), "Building Nuget project (Net40)");

private void BuildAllFrameworks()
{
	Build("Net40");
	Build("Net45");
	Build("Net46");	
	BuildDnx();
}

private void Build(string frameworkMoniker)
{
	var pathToSolutionFile = Path.Combine(pathToNugetBuildDirectory, frameworkMoniker + @"\Binary\LightInject.sln");
	WriteLine(pathToSolutionFile);
	MsBuild.Build(pathToSolutionFile);
}

private void BuildDnx()
{
	string pathToProjectFile = Path.Combine(pathToNugetBuildDirectory, @"Dnx/Binary/LightInject/project.json");
	DNU.Build(pathToProjectFile);
}

private void RestoreNuGetPackages()
{
	Execute(() => NuGet.Restore("../"), "Restoring packages related to build");
	Execute(() => NuGet.Restore("../../LightInject.Tests"), "Restoring packages for LightInject.Tests");		
}

private void RunAllUnitTests()
{	
	DirectoryUtils.Delete("TestResults");
	/*Execute(() => RunUnitTests("Net40"), "Running unit tests for Net40");
	Execute(() => RunUnitTests("Net45"), "Running unit tests for Net45");
	Execute(() => RunUnitTests("Net46"), "Running unit tests for Net46"); */
	Execute(() => AnalyzeTestCoverage("Net40"), "Analysing test coverage for Net40");			
}

private void RunUnitTests(string frameworkMoniker)
{
	string pathToTestAssembly = Path.Combine(pathToNugetBuildDirectory, frameworkMoniker + @"/Binary/LightInject.Tests/bin/Release/LightInject.Tests.dll");
	string pathToTestAdapter = ResolveDirectory("../../packages/", "xunit.runner.visualstudio.testadapter.dll");
	MsTest.Run(pathToTestAssembly, pathToTestAdapter);	
}


private void AnalyzeTestCoverage(string frameworkMoniker)
{	
	string pathToTestAdapter = ResolveDirectory("../../packages/", "xunit.runner.visualstudio.testadapter.dll");
	string pathToTestAssembly = Path.Combine(pathToNugetBuildDirectory, frameworkMoniker + @"/Binary/LightInject.Tests/bin/Release/LightInject.Tests.dll");
	MsTest.RunWithCodeCoverage(pathToTestAssembly, pathToTestAdapter, "lightinject.dll");
}

private void InitializNugetBuildDirectory()
{
	DirectoryUtils.Delete(pathToNugetBuildDirectory);	
	Execute(() => InitializeNugetBuildDirectoryForNet40(), "Preparing Net40");
	Execute(() => InitializeNugetBuildDirectoryForNet45(), "Preparing Net45");
	Execute(() => InitializeNugetBuildDirectoryForNet46(), "Preparing Net46");	
	Execute(() => InitializeNugetBuildDirectoryForDnx(), "Preparing Dnx");				
	//CreateDirectory(pathToNugetBuildDirectory);
	//RoboCopy(@"..\Templates", pathToNugetBuildDirectory, "/e");
	//File.Copy(pathToSourceFile, Path.Combine(pathToNugetBuildDirectory, "Net40/Binary/LightInject/LightInject.cs"), true);			
}

private void InitializeNugetBuildDirectoryForNet40()
{
	var pathToBinary = Path.Combine(pathToNugetBuildDirectory, "NET40/Binary");
	CreateDirectory(pathToBinary);
	RoboCopy("../../../LightInject", pathToBinary, "/e /XD bin obj .vs NuGet TestResults");	
	PatchProjectFile("NET40", "4.0");
	PatchTestProjectFile("NET40");	
	
	
	var patchToSource = Path.Combine(pathToNugetBuildDirectory, "NET40/Source");
	CreateDirectory(patchToSource);
	RoboCopy("../../../LightInject", patchToSource, "/e /XD bin obj .vs NuGet TestResults");
	Internalize("NET40");
		
	/*PatchProjectFile("NET40", "4.0");
	PatchTestProjectFile("NET40");*/  
}

private void Internalize(string frameworkMoniker)
{
	string pathToSourceFile = Path.Combine(pathToNugetBuildDirectory, frameworkMoniker + "/Source/LightInject/LightInject.cs");
	Internalizer.Internalize(pathToSourceFile, frameworkMoniker);
}


private void InitializeNugetBuildDirectoryForNet45()
{
	var pathToBinary = Path.Combine(pathToNugetBuildDirectory, "NET45/Binary");
	CreateDirectory(pathToBinary);
	RoboCopy("../../../LightInject", pathToBinary, "/e /XD bin obj .vs NuGet TestResults");	
	PatchProjectFile("NET45", "4.5");
	PatchTestProjectFile("NET45");				
}

private void InitializeNugetBuildDirectoryForNet46()
{
	var pathToBinary = Path.Combine(pathToNugetBuildDirectory, "NET46/Binary");
	CreateDirectory(pathToBinary);
	RoboCopy("../../../LightInject", pathToBinary, "/e /XD bin obj .vs NuGet TestResults");	
	PatchProjectFile("NET46", "4.6");
	PatchTestProjectFile("NET46");	
}

private void InitializeNugetBuildDirectoryForDnx()
{
	var pathToBinary = Path.Combine(pathToNugetBuildDirectory, "DNX/Binary");
	CreateDirectory(pathToBinary);
	RoboCopy("../../../LightInject", pathToBinary, "/e /XD bin obj .vs NuGet TestResults");	
}


private void BuildNugetProject(string framework, string version)
{				
	string pathToNugetSolutionFile = Path.Combine(pathToNugetBuildDirectory, framework + "/" + @"Binary/" + framework + ".sln");
	MsBuild.Build(pathToNugetSolutionFile);
}

private void PatchAssemblyInfo()
{
	Execute(() => PatchAssemblyInfo("Net40"), "Patching AssemblyInfo (Net40)");
	Execute(() => PatchAssemblyInfo("Net40"), "Patching AssemblyInfo (Net45)");
	Execute(() => PatchAssemblyInfo("Net40"), "Patching AssemblyInfo (Net46)");	
	Execute(() => PatchAssemblyInfo("Dnx"), "Patching AssemblyInfo (Dnx)");
}

private void PatchAssemblyInfo(string framework)
{	
	var pathToAssemblyInfo = Path.Combine(pathToNugetBuildDirectory, framework + @"\Binary\Lightinject\Properties\AssemblyInfo.cs");	
	PatchAssemblyVersionInfo(version, framework, pathToAssemblyInfo);
}

private void PatchProjectFile(string frameworkMoniker, string frameworkVersion)
{
	var pathToProjectFile = Path.Combine(pathToNugetBuildDirectory, frameworkMoniker + @"\Binary\Lightinject\LightInject.csproj");
	WriteLine("Patching {0} ", pathToProjectFile);	
	XDocument xmlDocument = XDocument.Load(pathToProjectFile);
	var frameworkVersionElement = xmlDocument.Descendants().SingleOrDefault(p => p.Name.LocalName == "TargetFrameworkVersion");
	frameworkVersionElement.Value = frameworkVersion;
	
	var defineConstantsElements = xmlDocument.Descendants().Where(p => p.Name.LocalName == "DefineConstants");
	foreach (var defineConstantsElement in defineConstantsElements)
	{
		defineConstantsElement.Value = defineConstantsElement.Value.Replace("NET46", frameworkMoniker); 
	}			
	xmlDocument.Save(pathToProjectFile);
}

private void PatchTestProjectFile(string frameworkMoniker)
{
	var pathToProjectFile = Path.Combine(pathToNugetBuildDirectory, frameworkMoniker + @"\Binary\Lightinject.Tests\LightInject.Tests.csproj");
	WriteLine("Patching {0} ", pathToProjectFile);	
	XDocument xmlDocument = XDocument.Load(pathToProjectFile);
	var defineConstantsElements = xmlDocument.Descendants().Where(p => p.Name.LocalName == "DefineConstants");
	foreach (var defineConstantsElement in defineConstantsElements)
	{
		defineConstantsElement.Value = defineConstantsElement.Value.Replace("NET46", frameworkMoniker); 
	}			
	xmlDocument.Save(pathToProjectFile);
}


