#! "netcoreapp1.1"
#r "nuget:NetStandard.Library,1.6.1"
#r "nuget:Microsoft.Extensions.CommandLineUtils, 1.1.1"
#load "context.csx"
#load "tasks.csx"
#load "AssemblyVersion.csx"
#load "Internalizer.csx"
#load "DotNet.csx"
#load "NuGet.csx"

using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Linq;
using System;
using System.Text.RegularExpressions;
using Microsoft.Extensions.CommandLineUtils;


// Tasks.Add(() => Compile(), () => Test(), () => Pack())

Tasks.Add(() => Compile());
Tasks.Add(() => Test(), () => Compile());
Tasks.Add(() => Pack(), () => Test());
//Tasks.Add(() => )
//Tasks.Execute(() => Test());
Tasks.Execute(Args.ToArray());

// WriteLine("LightInject version {0}" , version);
// Execute(() => InitializBuildDirectories(), "Preparing build directories");
// Execute(() => RenameSolutionFiles(), "Renaming solution files");
// Execute(() => PatchAssemblyInfo(), "Patching assembly information");
// Execute(() => InternalizeSourceVersions(), "Internalizing source versions");
// Execute(() => BuildAllFrameworks(), "Building all frameworks");
// //Execute(() => RunAllUnitTests(), "Running unit tests");

// Execute(() => AnalyzeTestCoverage(), "Analyzing test coverage");
// Execute(() => CreateNugetPackages(), "Creating NuGet packages");
// return;


private void Compile()
{
    BuildContext.Initialize("LightInject");
    InitializeBuildDirectory();
    PatchAssemblyInfo();
    Internalize();
    BuildAllProjects();

    void InitializeBuildDirectory()
    {
        FileUtils.RemoveDirectory(BuildContext.BuildFolder);
        FileUtils.CopySolution(BuildContext.SolutionFolder, BuildContext.BinaryBuildFolder);
        FileUtils.CopySolution(BuildContext.SolutionFolder, BuildContext.SourceBuildFolder);
    }

    void PatchAssemblyInfo()
    {
        AssemblyInfo.SetVersionInfo(BuildContext.Version, BuildContext.FileVersion, BuildContext.BinaryProjectFolder);
        AssemblyInfo.SetVersionInfo(BuildContext.Version, BuildContext.FileVersion, BuildContext.SourceBuildFolder);
        AssemblyInfo.AddInternalsVisibleToAttribute(BuildContext.SourceBuildFolder, BuildContext.TestProjectName);
    }

    void Internalize()
    {
        Internalizer.Internalize(BuildContext.SourceProjectFolder);
    }

    void BuildAllProjects()
    {
        DotNet.Build(BuildContext.BinaryProjectFolder);
        DotNet.Build(BuildContext.BinaryTestProjectFolder);
        DotNet.Build(BuildContext.SourceProjectFolder);
        DotNet.Build(BuildContext.SourceTestProjectFolder);        
    }
}

private void Test()
{
    //DotNet.Test(BuildContext.BinaryTestProjectFolder);    
}

private void Pack()
{
    FileUtils.CreateDirectory(BuildContext.PackageOutputFolder);    
    DotNet.Pack(BuildContext.BinaryProjectFolder, BuildContext.PackageOutputFolder);    
    CreateSourcePackage();
    void CreateSourcePackage()
    {
        var pathToMetadata = Path.Combine(BuildContext.SourceProjectFolder, $"{BuildContext.ProjectName}.Source.nuspec");
        var pathToNugetSpecTemplate = $"{BuildContext.ProjectName}.Source.nuspec";
        File.Copy(pathToNugetSpecTemplate, pathToMetadata, true);
        var pathToSourceFile = Path.Combine(BuildContext.SourceProjectFolder, $"{BuildContext.ProjectName}.cs");
        var pathToSourceFileTemplate = Path.Combine(BuildContext.SourceProjectFolder, "LightInject.cs.pp");
        File.Copy(pathToSourceFile, pathToSourceFileTemplate, true);
        FileUtils.ReplaceInFile(@"namespace \S*", $"namespace $rootnamespace$.{BuildContext.ProjectName}", pathToSourceFileTemplate);
        NuGet.PatchNugetVersionInfo(pathToMetadata, BuildContext.Version);
        FileUtils.ReplaceInFile(@"\[PATH_TO_SOURCEFILE\]", "LightInject.cs", pathToMetadata);
        NuGet.Pack(pathToMetadata, BuildContext.PackageOutputFolder);
    }
}

// private void CreateNugetPackages()
// {
// 	string pathToProjectFile = Path.Combine(BuildContext.BuildFolder, "binary/LightInject/LightInject.csproj");
// 	DotNet.Pack(pathToProjectFile);

// 	// string pathToNuGetBuildDirectory = Path.Combine(PathToBuildDirectory, "NuGetPackages");
// 	// DirectoryUtils.Delete(pathToNuGetBuildDirectory);


// 	// Execute(() => CopySourceFilesToNuGetLibDirectory(), "Copy source files to NuGet lib directory");		
// 	// Execute(() => CopyBinaryFilesToNuGetLibDirectory(), "Copy binary files to NuGet lib directory");

// 	// Execute(() => CreateSourcePackage(), "Creating source package");		
// 	// Execute(() => CreateBinaryPackage(), "Creating binary package");   	
// }

// private void CopySourceFilesToNuGetLibDirectory()
// {	
// 	CopySourceFile("NET45", "net45");
// 	CopySourceFile("NET46", "net46");		
// 	CopySourceFile("NETSTANDARD11", "netstandard1.1");		
// 	CopySourceFile("NETSTANDARD13", "netstandard1.3");	
// 	CopySourceFile("NETSTANDARD16", "netstandard1.6");	
//     CopySourceFile("PCL_111", "portable-net45+netcore45+wpa81");
// 	CopySourceFile("PCL_111", "Xamarin.iOS");
// 	CopySourceFile("PCL_111", "monoandroid");
// 	CopySourceFile("PCL_111", "monotouch");
// 	CopySourceFile("PCL_111", "uap");
// }

// private void CreateSourcePackage()
// {	    
// 	string pathToMetadataFile = Path.Combine(PathToBuildDirectory, "NugetPackages/Source/package/LightInject.Source.nuspec");	
//     PatchNugetVersionInfo(pathToMetadataFile, version);		    
// 	NuGet.CreatePackage(pathToMetadataFile, PathToBuildDirectory);   		
// }

// private void CopySourceFile(string frameworkMoniker, string packageDirectoryName)
// {
// 	string pathToMetadata = "../src/LightInject/NuGet";
// 	string pathToPackageDirectory = Path.Combine(PathToBuildDirectory, "NugetPackages/Source/package");	
// 	RoboCopy(pathToMetadata, pathToPackageDirectory, "LightInject.Source.nuspec");	

// 	string pathToSourceFile = Path.Combine(PathToBuildDirectory, frameworkMoniker ,@"Source\LightInject");
// 	string pathToDestination = Path.Combine(pathToPackageDirectory, "content/" + packageDirectoryName + "/LightInject");
// 	RoboCopy(pathToSourceFile, pathToDestination, "LightInject.cs");
// 	FileUtils.Rename(Path.Combine(pathToDestination, "LightInject.cs"), "LightInject.cs.pp");
// 	ReplaceInFile(@"namespace \S*", "namespace $rootnamespace$.LightInject", Path.Combine(pathToDestination, "LightInject.cs.pp"));
// }

// private void CopyBinaryFile(string frameworkMoniker, string packageDirectoryName)
// {
// 	string pathToMetadata = "../src/LightInject/NuGet";
// 	string pathToPackageDirectory = Path.Combine(PathToBuildDirectory, "NugetPackages/Binary/package");
// 	RoboCopy(pathToMetadata, pathToPackageDirectory, "LightInject.nuspec");
// 	string pathToBinaryFile = Path.Combine(PathToBuildDirectory,frameworkMoniker,@"Binary\LightInject\bin\Release");	
// 	string pathToDestination = Path.Combine(pathToPackageDirectory, "lib/" + packageDirectoryName);
// 	RoboCopy(pathToBinaryFile, pathToDestination, "LightInject.* /xf *.json");
// }

// private string ResolvePathToBinaryFile(string frameworkMoniker)
// {
// 	var pathToBinaryFile = Directory.GetFiles("tmp/" + frameworkMoniker + "/Binary/LightInject/bin/Release","LightInject.dll", SearchOption.AllDirectories).First();
// 	return Path.GetDirectoryName(pathToBinaryFile);		
// }

// private void BuildAllFrameworks()
// {		
// 	BuildDotNet();
// }

// private void Build(string frameworkMoniker)
// {
// 	var pathToSolutionFile = Directory.GetFiles(Path.Combine(PathToBuildDirectory, frameworkMoniker + @"\Binary\"),"*.sln").First();	
// 	WriteLine(pathToSolutionFile);
// 	MsBuild.Build(pathToSolutionFile);
// 	pathToSolutionFile = Directory.GetFiles(Path.Combine(PathToBuildDirectory, frameworkMoniker + @"\Source\"),"*.sln").First();
// 	MsBuild.Build(pathToSolutionFile);
// }





// private void RunUnitTests(string frameworkMoniker)
// {
// 	string pathToTestAssembly = Path.Combine(PathToBuildDirectory, frameworkMoniker + @"\Binary\LightInject.Tests\bin\Release\LightInject.Tests.dll");
// 	string testAdapterSearchDirectory = Path.Combine(PathToBuildDirectory, frameworkMoniker, @"Binary\packages\");
//     string pathToTestAdapterDirectory = ResolveDirectory(testAdapterSearchDirectory, "xunit.runner.visualstudio.testadapter.dll");
// 	MsTest.Run(pathToTestAssembly, pathToTestAdapterDirectory);	
// }

// private void AnalyzeTestCoverage()
// {	
// 	Execute(() => AnalyzeTestCoverage("NET452"), "Analyzing test coverage for NET45");
// 	Execute(() => AnalyzeTestCoverage("NET46"), "Analyzing test coverage for NET46");
// }

// private void AnalyzeTestCoverage(string frameworkMoniker)
// {	
//     string pathToPackagesFolder = Path.Combine(PathToBuildDirectory, "packages");
// 	NuGet.Install("ReportGenerator", pathToPackagesFolder);
// 	NuGet.Install("CoverageToXml", pathToPackagesFolder);
// 	NuGet.Install("xunit.runner.visualstudio -pre", pathToPackagesFolder);

// 	string pathToTestAssembly = Path.Combine(PathToBuildDirectory, $@"Binary\LightInject.Tests\bin\Release\{frameworkMoniker}\LightInject.Tests.dll");	
//     string pathToTestAdapterDirectory = ResolveDirectory(pathToPackagesFolder, "xunit.runner.visualstudio.testadapter.dll");		
//     MsTest.RunWithCodeCoverage(pathToTestAssembly, pathToPackagesFolder,pathToTestAdapterDirectory, "LightInject.dll");
// }



// private void InitializeNugetBuildDirectory()
// {
// 	var pathToBinary = Path.Combine(PathToBuildDirectory +  "/Binary");		
//     CreateDirectory(pathToBinary);
// 	RoboCopy("../vs2017/LightInject/", pathToBinary, "/e /XD bin obj .vs NuGet TestResults packages");	

// 	var pathToSource = Path.Combine(PathToBuildDirectory +  "/Source");	
// 	CreateDirectory(pathToSource);
// 	RoboCopy("../vs2017/LightInject/", pathToSource, "/e /XD bin obj .vs NuGet TestResults packages");					  
// }



// private void RenameSolutionFiles()
// {	
// 	RenameSolutionFile();	
// }

// private void Internalize()
// {
// 	string[] exceptTheseTypes = new string[] {
// 		"IProxy",
// 		"IInvocationInfo", 
// 		"IMethodBuilder", 
// 		"IDynamicMethodSkeleton", 
// 		"IProxyBuilder", 
// 		"IInterceptor", 
// 		"MethodInterceptorFactory",
// 		"TargetMethodInfo",
// 		"OpenGenericTargetMethodInfo",
// 		"DynamicMethodBuilder",
// 		"CachedMethodBuilder",
// 		"TargetInvocationInfo",
// 		"InterceptorInvocationInfo",
// 		"CompositeInterceptor",
// 		"InterceptorInfo",
// 		"ProxyDefinition"		
// 		}; 

// 	string pathToSourceFile = Path.Combine(PathToBuildDirectory, "Source/LightInject/LightInject.cs");
// 	Internalizer.Internalize(pathToSourceFile, exceptTheseTypes);
// }

// private void InternalizeSourceVersions()
// {
// 	Internalize();	
// }

// private void PatchAssemblyInfo()
// {	
// 	var pathToAssemblyInfo = Path.Combine(PathToBuildDirectory, @"Binary\LightInject\AssemblyInfo.cs");	
// 	PatchAssemblyVersionInfo(version, fileVersion, pathToAssemblyInfo);
// 	pathToAssemblyInfo = Path.Combine(PathToBuildDirectory, @"Source\LightInject\AssemblyInfo.cs");
// 	PatchAssemblyVersionInfo(version, fileVersion, pathToAssemblyInfo);	
// 	PatchInternalsVisibleToAttribute(pathToAssemblyInfo);		
// }



