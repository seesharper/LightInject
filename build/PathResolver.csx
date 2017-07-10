#load "NuGet.csx"
#load "Context.csx"
#load "FileUtils.csx"
using System.Text.RegularExpressions;

public static class PathResolver
{
    public static string GetPathToMsBuild()
    {        
        var pathToVisualStudioInstallation = GetPathToVisualStudioInstallation();
        var pathToMsBuild = Path.Combine(pathToVisualStudioInstallation, @"MSBuild\15.0\Bin\MsBuild.exe");
        return pathToMsBuild;
    }

    public static string GetPathToVsTest()
    {        
        var pathToVisualStudioInstallation = GetPathToVisualStudioInstallation();
        var pathToTestConsole = Path.Combine(pathToVisualStudioInstallation, @"Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe");
        return pathToTestConsole;
    }

    public static string GetPathToVisualStudioInstallation()
    {
        var pathToVsWhere = FileUtils.FindFile(BuildContext.BuildPackagesFolder ,"vswhere.exe");
        if (pathToVsWhere == null)
        {
            NuGet.Install("vswhere", BuildContext.BuildPackagesFolder);
        }
        pathToVsWhere = FileUtils.FindFile(BuildContext.BuildPackagesFolder,"vswhere.exe");

        var result = Command.Execute(pathToVsWhere, "", ".");
        WriteLine(result);                
        var match = Regex.Match(result,@"installationPath:\s*(.*)\r\n");            
        var pathToVisualStudioInstallation = match.Groups[1].Captures[0].Value; 
        return pathToVisualStudioInstallation;
    }
}