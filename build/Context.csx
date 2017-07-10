#load "FileUtils.csx"
using System.IO;
using System.Text.RegularExpressions;

public static class BuildContext
{
    public static string BuildFolder;

    public static string BinaryBuildFolder;

    public static string SourceBuildFolder;

    public static string BinaryProjectFolder;

    public static string BinaryTestProjectFolder;

    public static string SourceProjectFolder;

    public static string SourceTestProjectFolder;

    public static string BuildPackagesFolder;

    public static string SourceFolder;

    public static string Version;

    public static string FileVersion;

    public static string ProjectName;

    public static string TestProjectName;

    public static string SolutionFolder;
    
    public static string PackageOutputFolder;

    public static string SourceFileName;

    public static void Initialize(string projectName)
    {
        ProjectName = projectName;
        TestProjectName = $"{ProjectName}.Tests"; 
        BuildFolder = Path.GetFullPath("../tmp");        
        BinaryBuildFolder = Path.Combine(BuildFolder, "binary");
        BinaryProjectFolder = Path.Combine(BinaryBuildFolder,ProjectName);  
        BinaryTestProjectFolder = Path.Combine(BinaryBuildFolder,$"{ProjectName}.Tests");      
        SourceBuildFolder = Path.Combine(BuildFolder, "source");
        SourceTestProjectFolder = Path.Combine(SourceBuildFolder,$"{ProjectName}.Tests");      
        SourceProjectFolder = Path.Combine(SourceBuildFolder,ProjectName);
        BuildPackagesFolder = Path.Combine(BuildFolder, "packages");        
        SolutionFolder = @"..\src";
        SourceFolder = $@"..\src\{projectName}";
        PackageOutputFolder = Path.Combine(BuildFolder, "NuGetPackages");
        SourceFileName = Path.Combine(SourceProjectFolder,$"{ProjectName}.cs");
        ReadVersionFile((Path.Combine(SourceFolder, "version.txt")));        
    }

    private static void ReadVersionFile(string pathToVersionFile)
    {
        var version = FileUtils.ReadFile(pathToVersionFile);
        FileVersion = Regex.Match(version, @"(^[\d\.]+)-?").Groups[1].Captures[0].Value;
        Version = Regex.Match(version, @"(\d\.\d\.\d\S*)").Groups[1].Value;
    }
}

