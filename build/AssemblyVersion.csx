#load "FileUtils.csx"
#load "Write.csx"
using System.Text.RegularExpressions;

public static class AssemblyInfo
{
    public static void SetVersionInfo(string version, string fileVersion, string pathToProject)
    {
        var pathToAssemblyInfo = FileUtils.FindFile(pathToProject, "AssemblyInfo.cs");
        Write.Info($"Patching {pathToAssemblyInfo} with version {version} and fileversion {fileVersion}");
        var assemblyInfo = FileUtils.ReadFile(pathToAssemblyInfo);
        var patchedAssemblyInfo = Regex.Replace(assemblyInfo, @"((?<=AssemblyVersion\(.)[\d\.]+)", fileVersion);
        patchedAssemblyInfo = Regex.Replace(patchedAssemblyInfo, @"((?<=AssemblyFileVersion\(.)[\d\.]+)", fileVersion);
        patchedAssemblyInfo = Regex.Replace(patchedAssemblyInfo, @"((?<=AssemblyInformationalVersion\(.)[\d\.]+)", version);
        patchedAssemblyInfo = Regex.Replace(patchedAssemblyInfo, @"(AssemblyCopyright\(""\D+)(\d*)", "${1}" + DateTime.Now.Year);
        FileUtils.WriteFile(pathToAssemblyInfo, patchedAssemblyInfo);
    }

    public static void AddInternalsVisibleToAttribute(string pathToProject, string assenblyName)
    {
        var pathToAssemblyInfo = FileUtils.FindFile(pathToProject, "AssemblyInfo.cs");
        var assemblyInfo = FileUtils.ReadFile(pathToAssemblyInfo);   
        StringBuilder sb = new StringBuilder(assemblyInfo);
        sb.AppendLine($"[assembly: InternalsVisibleTo(\"{assenblyName}\")]");
        FileUtils.WriteFile(pathToAssemblyInfo, sb.ToString());
    }
}
