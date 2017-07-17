#! "netcoreapp1.1"
#r "nuget:NetStandard.Library,1.6.1"
#r "nuget:Microsoft.Extensions.CommandLineUtils, 1.1.1"
#load "Context.csx"
#load "Tasks.csx"
#load "AssemblyVersion.csx"
#load "Internalizer.csx"
#load "DotNet.csx"
#load "NuGet.csx"
#load "MsTest.csx"
#load "Write.csx"
#load "xUnit.csx"
#load "CsProj.csx"

using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Linq;
using System;
using System.Text.RegularExpressions;
using Microsoft.Extensions.CommandLineUtils;


Tasks.Add(() => Compile(), () => Test(), () => Pack());
Tasks.Execute(Args.ToArray());


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
        AssemblyInfo.AddInternalsVisibleToAttribute(BuildContext.SourceBuildFolder, "DynamicAssembly");
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
    //DotNet.Test(BuildContext.SourceTestProjectFolder);    
    AnalyzeTestCoverage();    
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
        var pathToSourceFileTemplate = Path.Combine(BuildContext.SourceProjectFolder, $"{BuildContext.ProjectName}.cs.pp");
        File.Copy(pathToSourceFile, pathToSourceFileTemplate, true);
        FileUtils.ReplaceInFile(@"namespace \S*", $"namespace $rootnamespace$.{BuildContext.ProjectName}", pathToSourceFileTemplate);
        NuGet.PatchNugetVersionInfo(pathToMetadata, BuildContext.Version);
        FileUtils.ReplaceInFile(@"\[PATH_TO_SOURCEFILE\]", $"{BuildContext.ProjectName}.cs", pathToMetadata);
        NuGet.Pack(pathToMetadata, BuildContext.PackageOutputFolder);
    }
}

private void AnalyzeTestCoverage()
{	
	AnalyzeTestCoverage("NET452");
    AnalyzeTestCoverage("NET46");	
}

private void AnalyzeTestCoverage(string frameworkMoniker)
{	    	
    string pathToTestAssembly = Path.Combine(BuildContext.BinaryTestProjectFolder, 
    $@"bin\Release\{frameworkMoniker}\{BuildContext.ProjectName}.Tests.dll");	    
    xUnit.AnalyzeCodeCoverage(pathToTestAssembly);                
}





