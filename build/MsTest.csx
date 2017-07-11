#load "Command.csx"
#load "FileUtils.csx"
#load "PathResolver.csx"
#load "NuGet.csx"
using System.Text.RegularExpressions;

public static class MsTest
{    
    
    static MsTest()
    {
        NuGet.Install("ReportGenerator", BuildContext.BuildPackagesFolder);
	    NuGet.Install("CoverageToXml", BuildContext.BuildPackagesFolder);
	    NuGet.Install("xunit.runner.visualstudio -pre", BuildContext.BuildPackagesFolder);            
    }

    public static void RunWithCodeCoverage(string pathToTestAssembly, params string[] includedAssemblies)
    {
        string pathToMsTest = PathResolver.GetPathToVsTest();
        string pathToTestAdapter = FileUtils.FindDirectory(BuildContext.BuildPackagesFolder, "xunit.runner.visualstudio.testadapter.dll");
        var args = $"\"{pathToTestAssembly}\" /Enablecodecoverage /InIsolation /TestAdapterPath:\"{pathToTestAdapter}\" /TestCaseFilter:\"Category!=Verification\"";
        Write.Info("Executing test with code coverage");
        //string result = Command.Execute(pathToMsTest, args, @"(Test execution.*)|(Test Run.*)|(Total tests.*)|\s*(.*coverage)|(Attachments:)");        
        string result = Command.Execute(pathToMsTest, args,".");        

        Write.Info(result);
        //string result = Command.Execute(pathToMsTest, pathToTestAssembly + " /Enablecodecoverage" + " /TestAdapterPath:" + pathToTestAdapter, @"(Test execution.*)|(Test Run.*)|(Total tests.*)|\s*(.*coverage)|(Attachments:)");        
        var pathToCoverageFile = GetPathToCoverageFile(result);        
        if (pathToCoverageFile == null)
        {
            throw new InvalidOperationException("PathToCoverageFile is null");
        }

        var pathToCoverageXmlFile = GetPathToCoverageXmlFile(pathToCoverageFile);       
             
        var directory = Path.GetDirectoryName(pathToCoverageFile);        
        ConvertCoverageFileToXml(pathToCoverageFile);
        CreateSummaryFile(pathToCoverageXmlFile, directory, includedAssemblies);
        ValidateCodeCoverage(directory);        
    }

    private static void ValidateCodeCoverage(string directory)
    {
        var pathToSummaryFile = Path.Combine(directory, "Summary.xml");       
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

    public static void ConvertCoverageFileToXml(string pathToCoverageFile)
    {        
        string pathToCoverageToXml = FileUtils.FindFile(BuildContext.BuildPackagesFolder, "CoverageToXml.exe");                        
        Command.Execute(pathToCoverageToXml, $"\"{pathToCoverageFile}\"", null);
    }

    public static void CreateSummaryFile(string pathToCoverageXmlFile, string directory, string[] includedAssemblies)
    {        
        var filters = includedAssemblies.Select (a => "+" + a).Aggregate ((current, next) => current + ";" + next).ToLower();
        string pathToReportGenerator = FileUtils.FindFile(BuildContext.BuildPackagesFolder, "ReportGenerator.exe");
        var args = $"-reports:\"{pathToCoverageXmlFile}\" -targetdir:\"{directory}\" -reporttypes:xmlsummary -filters:{filters}";
        Command.Execute(pathToReportGenerator, args);
                
        // Command.Execute(pathToReportGenerator, "-reports:" + StringUtils.Quote(pathToCoverageXmlFile) + " -targetdir:" + StringUtils.Quote(directory)
        //     + " -reporttypes:xmlsummary -filters:" + filters );        
    }


    public static string GetPathToCoverageFile(string result)
    {
        var path = Regex.Match(result, @"Attachments:\s*(.*.coverage)").Groups[1].Value;
        WriteLine(path);        
        return path;        
    }

    private static string GetPathToCoverageXmlFile(string pathToCoverageFile)
    {
        return Path.Combine(Path.GetDirectoryName(pathToCoverageFile), Path.GetFileNameWithoutExtension(pathToCoverageFile)) + ".coveragexml";
    }

}