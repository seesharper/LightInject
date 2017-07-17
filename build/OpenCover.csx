#load "NuGet.csx"
#load "FileUtils.csx"
#load "Context.csx"
using System.Text.RegularExpressions;

public static class OpenCover
{    
    private static string pathToOpenCover;
    private static string pathToReportGenerator;
    static OpenCover()
    {
        NuGet.Install("OpenCover");
        NuGet.Install("ReportGenerator");
        pathToOpenCover = FileUtils.FindFile(BuildContext.BuildPackagesFolder,"OpenCover.Console.exe");
        pathToReportGenerator = FileUtils.FindFile(BuildContext.BuildPackagesFolder,"ReportGenerator.exe");
    }

    public static void Execute(string pathToTestRunner,string testRunnerArgs, string pathToCoverageFile, string filter = null)
    {        
        if (filter == null)
        {
            filter = $"+[{BuildContext.ProjectName}]*";
        }
        var args = $"-target:\"{pathToTestRunner}\" -targetargs:\"{testRunnerArgs}\" -output:\"{pathToCoverageFile}\" -filter:\"{filter}\" -register:user -excludebyattribute:*.ExcludeFromCodeCoverage*";
        Command.Execute(pathToOpenCover, args, ".");
        CreateSummaryFile(pathToCoverageFile);
    }

    private static void CreateSummaryFile(string pathToCoverageFile)
    {
        //var filters = includedAssemblies.Select (a => "+" + a).Aggregate ((current, next) => current + ";" + next).ToLower();
        var targetDirectory = Path.GetDirectoryName(pathToCoverageFile);
        var args = $"-reports:\"{pathToCoverageFile}\" -targetdir:\"{targetDirectory}\" -reporttypes:xmlsummary";
        Command.Execute(pathToReportGenerator, args);
        var pathToSummaryFile = Path.Combine(targetDirectory, "summary.xml");
        ValidateCodeCoverage(pathToSummaryFile);
    }

    private static void ValidateCodeCoverage(string pathToSummaryFile)
    {
        
        var summaryContent = FileUtils.ReadFile(pathToSummaryFile);                                                                           
        var coverage = Regex.Match(summaryContent, "LineCoverage>(.*)<",RegexOptions.IgnoreCase).Groups[1].Captures[0].Value;
        
        WriteLine("Code coverage is {0}", coverage);
                                
        if (coverage != "100%")
        {            
            MatchCollection matchesRepresentingClassesWithInsufficientCoverage = Regex.Matches(summaryContent, @"Class name=""(.*?)"" coverage=""(\d{1,2}|\d+\.\d+)""");
            foreach (Match match in matchesRepresentingClassesWithInsufficientCoverage)
            {
                var className = match.Groups[1].Captures[0].Value.Replace("&lt;", "<").Replace("&gt;", ">");
                var classCoverage = match.Groups[2].Captures[0].Value;
                WriteLine("Class name: {0} has only {1}% coverage", className, classCoverage);    
            }
                        
            throw new InvalidOperationException("Deploy failed. Test coverage is only " + coverage);
        }  
        
                
    }
}