#load "NuGet.csx"
#load "FileUtils.csx"
#load "OpenCover.csx"
#load "Context.csx"



public static class xUnit
{
    private static readonly string pathToTestRunner;

    static xUnit()
    {
        NuGet.Install("xunit.runner.console");    
        pathToTestRunner = FileUtils.FindFile(BuildContext.BuildPackagesFolder, "xunit.console.exe");
    }

    public static void AnalyzeCodeCoverage(string pathToTestAssembly, string filter = null)
    {        
        string pathToCoverageFile = Path.Combine(Path.GetDirectoryName(pathToTestAssembly),"coverage.xml");
        var testRunnerArgs = $"{pathToTestAssembly} -noshadow -notrait \"Category=Verification\"";
        Command.Execute(pathToTestRunner, testRunnerArgs, ".");
        OpenCover.Execute(pathToTestRunner, testRunnerArgs, pathToCoverageFile, filter);
        
    }
}