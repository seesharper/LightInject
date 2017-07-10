// #r "nuget:System.Diagnostics.Process, 4.3.0"
// #load "context.csx"
// #load "write.csx"
// #load "command.csx"

// using System.Diagnostics;
// using System.Xml.Linq;
// using System.Text.RegularExpressions;
// using Microsoft.Win32;
// using System.Globalization;
// using System.Collections.ObjectModel;
// using System.Threading;








// public static void RoboCopy(string source, string destination, string arguments = null)
// {    
//     if (!Directory.Exists(source))
//     {
//         throw new InvalidOperationException(string.Format("The directory {0} does not exist", source));
//     }
    
//     Command.Execute("robocopy", string.Format("{0} {1} {2}", source, destination, arguments));    
// }

// public static void RemoveDirectory(string directory)
// {
//     if (Directory.Exists(directory))
//     {
//         DirectoryUtils.Delete(directory);        
//     }
// }

// public static void CreateDirectory(string directory)
// {
//     RemoveDirectory(directory);
//     Directory.CreateDirectory(directory);
// }

// public static string ResolveDirectory(string path, string filePattern)
// {
//     string pathToFile = GetFile(path, filePattern);
//     return Path.GetDirectoryName(pathToFile);
// }



// // private static string CopyToNuGetBuildDirectory(string projectPath)
// // {
// //     var projectDirectory = Path.GetDirectoryName(projectPath);
// //     var tempSolutionFolder = GetTemporaryDirectory();
// //     string projectName = Regex.Match(projectPath, @"..\\(.+)").Groups[1].Value;
// //     var tempProjectFolder = Path.Combine(tempSolutionFolder, projectName);
// //     RoboCopy(projectPath, tempProjectFolder, "/S /XD obj bin");
// //     NuGet.Restore(tempProjectFolder);
// //     RemoveInternalsVisibleToAttribute(Path.Combine(tempProjectFolder, @"Properties\AssemblyInfo.cs"));
// //     return tempProjectFolder;
// // }

// public static string GetTemporaryDirectory()
// {
//    string tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetFileNameWithoutExtension(Path.GetRandomFileName()));
//    Directory.CreateDirectory(tempDirectory);
//    return tempDirectory;
// }

// public static string RemoveInternalsVisibleToAttribute(string assemblyInfo)
// {    
//     return Regex.Replace(assemblyInfo, @"(\[assembly: InternalsVisibleTo.+\])", string.Empty); 
// }

// public static void Execute(Action action, string description)
// {    
//     Write.Start(description);    
//     Stopwatch watch = new Stopwatch();
//     watch.Start();
//     action();
//     watch.Stop();        
//     Write.End("...Done! ({0} ms)", watch.ElapsedMilliseconds);                      
// }



// public static string GetVersionNumberFromSourceFile(string pathToSourceFile)
// {
//     var source = ReadFile(pathToSourceFile);
//     var versionNumber = Regex.Match(source, @"version\s(\d\.\d\.\d\S*)").Groups[1].Value;
//     return versionNumber;
// }





// public static class MsBuild
// {
//     public static void Build(string pathToSolutionFile)
//     {
//         string pathToMsBuild = PathResolver.GetPathToMsBuild();      
//         Command.Execute(pathToMsBuild, pathToSolutionFile + " /property:Configuration=Release /p:VisualStudioVersion=12.0 /verbosity:minimal",".");     
//     }       
// }





// public static class MsTest
// {    
//     public static void Run(string pathToTestAssembly, string pathToTestAdapter)
//     {         
//         string pathToMsTest = PathResolver.GetPathToVsTest();
//         string result = Command.Execute(pathToMsTest, pathToTestAssembly + " /TestAdapterPath:" + pathToTestAdapter, @"(Total tests:.*)|(Test execution time:.*)|(Failed.*)");
//     }
    
//     public static void RunWithCodeCoverage(string pathToTestAssembly, string pathToPackagesFolder, params string[] includedAssemblies)
//     {
//         string pathToMsTest = PathResolver.GetPathToVsTest();
//         string pathToTestAdapter = ResolveDirectory(pathToPackagesFolder, "xunit.runner.visualstudio.testadapter.dll");
//         string result = Command.Execute(pathToMsTest, pathToTestAssembly + " /Enablecodecoverage" + " /TestAdapterPath:" + pathToTestAdapter, @"(Test execution.*)|(Test Run.*)|(Total tests.*)|\s*(.*coverage)|(Attachments:)");        
//         var pathToCoverageFile = GetPathToCoverageFile(result);        
//         var pathToCoverageXmlFile = GetPathToCoverageXmlFile(pathToCoverageFile);       
             
//         var directory = Path.GetDirectoryName(pathToCoverageFile);        
//         ConvertCoverageFileToXml(pathToCoverageFile, pathToPackagesFolder);
//         CreateSummaryFile(pathToCoverageXmlFile, directory, includedAssemblies, pathToPackagesFolder);
//         ValidateCodeCoverage(directory);        
//     }

//     private static void ValidateCodeCoverage(string directory)
//     {
//         var pathToSummaryFile = Path.Combine(directory, "Summary.xml");       
//         var summaryContent = ReadFile(pathToSummaryFile);                                                                           
//         var coverage = Regex.Match(summaryContent, "LineCoverage>(.*)<",RegexOptions.IgnoreCase).Groups[1].Captures[0].Value;
        
//         WriteLine("Code coverage is {0}", coverage);
                                
//         if (coverage != "100%")
//         {            
//             MatchCollection matchesRepresentingClassesWithInsufficientCoverage = Regex.Matches(summaryContent, @"Class name=""(.*?)"" coverage=""(\d{1,2}|\d+\.\d+)""");
//             foreach (Match match in matchesRepresentingClassesWithInsufficientCoverage)
//             {
//                 var className = match.Groups[1].Captures[0].Value.Replace("&lt;", "<").Replace("&gt;", ">");
//                 var classCoverage = match.Groups[2].Captures[0].Value;
//                 WriteLine("Class name: {0} has only {1}% coverage", className, classCoverage);    
//             }
                        
//             throw new InvalidOperationException("Deploy failed. Test coverage is only " + coverage);
//         }  
        
                
//     }

//     public static void ConvertCoverageFileToXml(string pathToCoverageFile, string pathToPackagesFolder)
//     {        
//         string pathToCoverageToXml = GetFile(pathToPackagesFolder, "CoverageToXml.exe");                
//         Command.Execute(pathToCoverageToXml, StringUtils.Quote(pathToCoverageFile), null);
//     }

//     public static void CreateSummaryFile(string pathToCoverageXmlFile, string directory, string[] includedAssemblies, string pathToPackagesFolder)
//     {        
//         var filters = includedAssemblies.Select (a => "+" + a).Aggregate ((current, next) => current + ";" + next).ToLower();
//         string pathToReportGenerator = GetFile(pathToPackagesFolder, "ReportGenerator.exe");
//         Command.Execute(pathToReportGenerator, "-reports:" + StringUtils.Quote(pathToCoverageXmlFile) + " -targetdir:" + StringUtils.Quote(directory)
//             + " -reporttypes:xmlsummary -filters:" + filters );        
//     }


//     public static string GetPathToCoverageFile(string result)
//     {
//         var path = Regex.Match(result, @"Attachments:\s*(.*.coverage)").Groups[1].Value;
//         WriteLine(path);        
//         return path;        
//     }

//     private static string GetPathToCoverageXmlFile(string pathToCoverageFile)
//     {
//         return Path.Combine(Path.GetDirectoryName(pathToCoverageFile), Path.GetFileNameWithoutExtension(pathToCoverageFile)) + ".coveragexml";
//     }

// }

// public static class DirectoryUtils
// {   
    
// }

// public class FileUtils
// {    
//     public static void Rename(string pathToFile, string newName)
//     {
//         string directory = Path.GetDirectoryName(pathToFile);
//         string pathToNewFile = Path.Combine(directory, newName);
//         File.Move(pathToFile, pathToNewFile);
//     }

// }




// public static class StringUtils
// {
//     public static string Quote(string value)
//     {
//         return "\"" + value + "\"";
//     }
// }






    
   

   



   
