
#load "common.csx"

string pathToSolutionFile = @"..\..\LightInject.sln";

string pathToSourceFile = @"..\..\LightInject\LightInject.cs";

string pathToNugetBuildDirectory = @"..\tmp\";

//Execute(() => Execute(() => {WriteLine("TEST");}, "Child"), "Parent");



private string version = GetVersionNumberFromSourceFile(pathToSourceFile);

Console.WriteLine("LightInject version {0}" , version);

Execute(() => RestoreNuGetPackages(), "NuGet");

Execute(() => MsBuild.Build(pathToSolutionFile), "Building LightInject");

Execute(() => RunUnitTests(), "Running unit tests");

Execute(() => InitializNugetBuildDirectory(), "Preparing Nuget build directory");

Execute(() => PatchAssemblyInfo(), "Patching Nuget Assembly Info");

Execute(() => BuildNugetProject("Net40", version), "Building Nuget project (Net40)"); 

private void RestoreNuGetPackages()
{
	Execute(() => NuGet.Restore("../"), "Restoring packages related to build");
	Execute(() => NuGet.Restore("../../LightInject.Tests"), "Restoring packages for LightInject.Tests");		
}

private void RunUnitTests()
{	
	DirectoryUtils.Delete("TestResults");
	
	var pathToTestAssembly = "../../LightInject.Tests/bin/Release/LightInject.Tests.dll";
	MsTest.Run(pathToTestAssembly, ".");	
	Execute(() => AnalyzeTestCoverage(pathToTestAssembly), "Analysing test coverage");	
}

private void AnalyzeTestCoverage(string pathToTestAssembly)
{
	string pathToTestAdapter = "../../packages/xunit.runner.visualstudio.2.0.0/build/_common";
	MsTest.RunWithCodeCoverage(pathToTestAssembly, pathToTestAdapter, "lightinject.dll");
}



private void InitializNugetBuildDirectory()
{
	DirectoryUtils.Delete(pathToNugetBuildDirectory);
	
	CreateDirectory(pathToNugetBuildDirectory);

	RoboCopy(@"..\Templates", pathToNugetBuildDirectory, "/e");

	File.Copy(pathToSourceFile, Path.Combine(pathToNugetBuildDirectory, "Net40/Binary/LightInject/LightInject.cs"), true);
	
		
}

private void BuildNugetProject(string framework, string version)
{				
	string pathToNugetSolutionFile = Path.Combine(pathToNugetBuildDirectory, framework + "/" + @"Binary/" + framework + ".sln");
	MsBuild.Build(pathToNugetSolutionFile);
}

private void PatchAssemblyInfo()
{
	Execute(() => PatchAssemblyInfo("Net40"), "Patching AssemblyInfo (Net40)");	
}

private void PatchAssemblyInfo(string framework)
{	
	var pathToAssemblyInfo = Path.Combine(pathToNugetBuildDirectory, framework + @"\Binary\Lightinject\Properties\AssemblyInfo.cs");	
	PatchAssemblyVersionInfo(version, pathToAssemblyInfo);
}


