using System.Diagnostics;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using Microsoft.Win32;
using System.Globalization;
using System.Collections.ObjectModel;
using System.Threading;

private static int depth = 0;

private static string lastWriteOperation;





public static class DNU
{
    public static void Build(string pathToProjectFile, string frameworkMoniker)
    {        
        Command.Execute("cmd.exe","/C dnu.cmd --version " , ".");
        Command.Execute("cmd.exe","/C dnu.cmd restore " + pathToProjectFile, ".");        
        Command.Execute("cmd.exe","/C dnu.cmd build " + pathToProjectFile + " --framework " + frameworkMoniker + " --configuration Release" , ".");   
    }        
}

public static void RoboCopy(string source, string destination, string arguments = null)
{
    Command.Execute("robocopy", string.Format("{0} {1} {2}", source, destination, arguments));    
}

public static void RoboCopy(string source, string destination, string file, string arguments)
{
    Command.Execute("robocopy", string.Format("{0} {1} {2} {3}", source, destination, arguments));
}


private static void WriteStart(string message, params object[] arguments)
{
    StringBuilder sb = new StringBuilder();    
    sb.Append("\n");
    sb.Append(' ', depth);
    sb.Append(string.Format(message, arguments));            
    Console.Out.Flush();
    Console.Write(sb.ToString());
    Console.Out.Flush();
    lastWriteOperation = "WriteStart";
}

private static void WriteLine(string message,  params object[] arguments)
{
    if (message == null)
    {
        return;
    }
    StringBuilder sb = new StringBuilder();    
    if (lastWriteOperation == "WriteStart")
    {
        sb.Append("\n");
    }
    sb.Append(' ', depth);
    sb.Append(string.Format(message, arguments));            
    Console.WriteLine(sb.ToString());
    lastWriteOperation = "WriteLine";
}

private static void WriteEnd(string message, params object[] arguments)
{
    if (lastWriteOperation == "WriteStart")
    {
        Console.WriteLine(string.Format(message,arguments));
    }
    else
    {
        StringBuilder sb = new StringBuilder();    
        sb.Append(' ', depth);
        sb.Append(string.Format(message, arguments));            
        Console.WriteLine(sb.ToString());
        lastWriteOperation = "WriteLine";
    }
}

public static void RemoveDirectory(string directory)
{
    if (Directory.Exists(directory))
    {
        Directory.Delete(directory, true);
    }
}

public static void CreateDirectory(string directory)
{
    RemoveDirectory(directory);
    Directory.CreateDirectory(directory);
}

public static string ResolveDirectory(string path, string filePattern)
{
    string pathToFile = Directory.GetFiles(path, "xunit.runner.visualstudio.testadapter.dll", SearchOption.AllDirectories).Single();
    return Path.GetDirectoryName(pathToFile);
}


private static string CopyToNuGetBuildDirectory(string projectPath)
{
    var projectDirectory = Path.GetDirectoryName(projectPath);
    var tempSolutionFolder = GetTemporaryDirectory();
    string projectName = Regex.Match(projectPath, @"..\\(.+)").Groups[1].Value;
    var tempProjectFolder = Path.Combine(tempSolutionFolder, projectName);
    RoboCopy(projectPath, tempProjectFolder, "/S /XD obj bin");
    NuGet.Restore(tempProjectFolder);
    RemoveInternalsVisibleToAttribute(Path.Combine(tempProjectFolder, @"Properties\AssemblyInfo.cs"));
    return tempProjectFolder;
}

public static string GetTemporaryDirectory()
{
   string tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetFileNameWithoutExtension(Path.GetRandomFileName()));
   Directory.CreateDirectory(tempDirectory);
   return tempDirectory;
}

public static string RemoveInternalsVisibleToAttribute(string assemblyInfo)
{    
    return Regex.Replace(assemblyInfo, @"(\[assembly: InternalsVisibleTo.+\])", string.Empty); 
}

public static void Execute(Action action, string description)
{
    int currentIndentation = depth;    
    WriteStart(description);
    depth++;              
    Stopwatch watch = new Stopwatch();
    watch.Start();
    action();
    watch.Stop();    
    depth--; 
    WriteEnd("...Done! ({0} ms)", watch.ElapsedMilliseconds);                      
}

public static void PatchAssemblyVersionInfo(string version, string fileVersion, string frameworkMoniker, string pathToAssemblyInfo)
{    
    var assemblyInfo = ReadFile(pathToAssemblyInfo);    
    var patchedAssemblyInfo = Regex.Replace(assemblyInfo, @"((?<=AssemblyVersion\(.)[\d\.]+)", fileVersion);
    patchedAssemblyInfo = Regex.Replace(patchedAssemblyInfo, @"((?<=AssemblyFileVersion\(.)[\d\.]+)", fileVersion);
    patchedAssemblyInfo = Regex.Replace(patchedAssemblyInfo, @"((?<=AssemblyInformationalVersion\(.)[\d\.]+)", version);        
    patchedAssemblyInfo = Regex.Replace(patchedAssemblyInfo, @"(AssemblyCopyright\(""\D+)(\d*)", "${1}" + DateTime.Now.Year);        
    WriteFile(pathToAssemblyInfo, patchedAssemblyInfo);    
}



public static string GetVersionNumberFromSourceFile(string pathToSourceFile)
{
    var source = ReadFile(pathToSourceFile);
    var versionNumber = Regex.Match(source, @"version\s(.+).").Groups[1].Value;
    return versionNumber;
}

public static void PatchNugetVersionInfo(string pathToNugetSpecification, string version)
{
    ReplaceInFile(@"(<version>)(.+)(<\/version>)", "${1}" + version + "$3", pathToNugetSpecification);   
}

public static void ReplaceInFile(string pattern, string value, string pathToFile)
{
    var source = ReadFile(pathToFile);
    var replacedSource = Regex.Replace(source, pattern, value);
    WriteFile(pathToFile, replacedSource);
}


public static string ReadFile(string pathToFile)
{
    using(var reader = new StreamReader(pathToFile))
    {
        return reader.ReadToEnd();
    }
}

public static void WriteFile(string pathToFile, string content)
{
    using(var writer = new StreamWriter(pathToFile))
    {
        writer.Write(content);
    }
}

public static class MsBuild
{
    public static void Build(string pathToSolutionFile)
    {
        string pathToMsBuild = ResolvePathToMsBuild();        
        Command.Execute(pathToMsBuild, pathToSolutionFile + " /property:Configuration=Release /p:VisualStudioVersion=12.0 /verbosity:minimal",".");     
    }
    
    private static string ResolvePathToMsBuild()
    {                
        return Path.Combine(PathResolver.GetPathToMsBuildTools(), "MsBuild.exe");                                                  
    }
}

public static class PathResolver
{
    public static string GetPathToMsBuildTools()
    {
        string keyName = @"SOFTWARE\Wow6432Node\Microsoft\MSBuild\ToolsVersions";
        string decimalSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator; 
        
        RegistryKey key = Registry.LocalMachine.OpenSubKey(keyName);
        string[] subKeynames = key.GetSubKeyNames().Select(n => n.Replace(".", decimalSeparator)).ToArray();        
        Collection<decimal> versionNumbers = new Collection<decimal>();
        
        for (int i = 0; i < subKeynames.Length; i++)
        {
            decimal versionNumber;
            if (decimal.TryParse(subKeynames[i], out versionNumber))
            {
                versionNumbers.Add(versionNumber);
            }                        
        }
        
        decimal latestVersionNumber = versionNumbers.OrderByDescending(n => n).First();
        RegistryKey latestVersionSubKey = key.OpenSubKey(latestVersionNumber.ToString().Replace(decimalSeparator, "."));
        string pathToMsBuildTools = (string)latestVersionSubKey.GetValue("MSBuildToolsPath");
        return pathToMsBuildTools;
    }
}



public static class MsTest
{    
    public static void Run(string pathToTestAssembly, string pathToTestAdapter)
    {
        string pathToMsTest = @"C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe";
        string result = Command.Execute(pathToMsTest, pathToTestAssembly + " /TestAdapterPath:" + pathToTestAdapter, @"(Total tests:.*)|(Test execution time:.*)|(Failed.*)");
    }
    
    public static void RunWithCodeCoverage(string pathToTestAssembly, string pathToTestAdapter, params string[] includedAssemblies)
    {
        string pathToMsTest = @"C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe";
        string result = Command.Execute(pathToMsTest, pathToTestAssembly + " /Enablecodecoverage" + " /TestAdapterPath:" + pathToTestAdapter, @"(Test execution.*)|(Test Run.*)|(Total tests.*)|\s*(.*coverage)|(Attachments:)");        
        var pathToCoverageFile = GetPathToCoverageFile(result);        
        var pathToCoverageXmlFile = GetPathToCoverageXmlFile(pathToCoverageFile);    
        var directory = Path.GetDirectoryName(pathToCoverageFile);        
        ConvertCoverageFileToXml(pathToCoverageFile);
        CreateSummaryFile(pathToCoverageXmlFile, directory, includedAssemblies);
        ValidateCodeCoverage(directory);        
    }


    private static void ValidateCodeCoverage(string directory)
    {
        var pathToSummaryFile = Path.Combine(directory, "Summary.xml");       
        var summaryContent = ReadFile(pathToSummaryFile);                                                                           
        var coverage = Regex.Match(summaryContent, "LineCoverage>(.*)<").Groups[1].Captures[0].Value;
        
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
        string pathToCoverageToXml = "../../packages/CoverageToXml.1.0.0/CoverageToXml.exe";        
        Command.Execute(pathToCoverageToXml, StringUtils.Quote(pathToCoverageFile), null);
    }

    public static void CreateSummaryFile(string pathToCoverageXmlFile, string directory, string[] includedAssemblies)
    {
        
        var filters = includedAssemblies.Select (a => "+" + a).Aggregate ((current, next) => current + ";" + next).ToLower();

        Command.Execute("../../packages/ReportGenerator.2.1.7.0/tools/ReportGenerator.exe", "-reports:" + StringUtils.Quote(pathToCoverageXmlFile) + " -targetdir:" + StringUtils.Quote(directory)
            + " -reporttypes:xmlsummary -filters:" + filters );        
    }


    public static string GetPathToCoverageFile(string result)
    {
        var path = Regex.Match(result, @"Attachments:\s*(.*.coverage)").Groups[1].Value;        
        return path;        
    }

    private static string GetPathToCoverageXmlFile(string pathToCoverageFile)
    {
        return Path.Combine(Path.GetDirectoryName(pathToCoverageFile), Path.GetFileNameWithoutExtension(pathToCoverageFile)) + ".coveragexml";
    }

}

public static class DirectoryUtils
{   
    public static void Delete(string path)
    {
        if (!Directory.Exists(path))
        {
            return;
        }
                
        // http://stackoverflow.com/questions/329355/cannot-delete-directory-with-directory-deletepath-true
        foreach (string directory in Directory.GetDirectories(path))
        {
            Delete(directory);
        }
        
        try
        {
            Directory.Delete(path, true);
        }
        catch (IOException) 
        {
            Directory.Delete(path, true);
        }
        catch (UnauthorizedAccessException)
        {
            Directory.Delete(path, true);
        }
    } 
}

public class FileUtils
{    
    public static void Rename(string pathToFile, string newName)
    {
        string directory = Path.GetDirectoryName(pathToFile);
        string pathToNewFile = Path.Combine(directory, newName);
        File.Move(pathToFile, pathToNewFile);
    }

}


public static class Command
{
    private static StringBuilder lastProcessOutput = new StringBuilder();
    
    private static StringBuilder lastStandardErrorOutput = new StringBuilder();    
          
    public static string Execute(string commandPath, string arguments, string capture = null)
    {
        lastProcessOutput.Clear();
        lastStandardErrorOutput.Clear();
        var startInformation = CreateProcessStartInfo(commandPath, arguments);
        var process = CreateProcess(startInformation);
        SetVerbosityLevel(process, capture);        
        process.Start();        
        RunAndWait(process);                
               
        if (process.ExitCode != 0 && commandPath != "robocopy")
        {                      
            WriteLine(lastStandardErrorOutput.ToString());
            throw new InvalidOperationException("Command failed");
        }   
        
        return lastProcessOutput.ToString();
    }

    private static ProcessStartInfo CreateProcessStartInfo(string commandPath, string arguments)
    {
        var startInformation = new ProcessStartInfo(StringUtils.Quote(commandPath));
        startInformation.CreateNoWindow = true;
        startInformation.Arguments =  arguments;
        startInformation.RedirectStandardOutput = true;
        startInformation.RedirectStandardError = true;
        startInformation.UseShellExecute = false;
        
        return startInformation;
    }

    private static void RunAndWait(Process process)
    {        
        process.BeginErrorReadLine();
        process.BeginOutputReadLine();         
        process.WaitForExit();                
    }

    private static void SetVerbosityLevel(Process process, string capture = null)
    {
        if(capture != null)
        {
            process.OutputDataReceived += (s, e) => 
            {
                if (e.Data == null)
                {
                    return;
                }
                              
                if (Regex.Matches(e.Data, capture,RegexOptions.Multiline).Count > 0)
                {
                    lastProcessOutput.AppendLine(e.Data);
                    WriteLine(e.Data);        
                }                                             
            };                        
        }
        process.ErrorDataReceived += (s, e) => 
        {
            lastStandardErrorOutput.AppendLine();
            lastStandardErrorOutput.AppendLine(e.Data);                        
        };
    }

    private static Process CreateProcess(ProcessStartInfo startInformation)
    {
        var process = new Process();
        process.StartInfo = startInformation;       
        //process.EnableRaisingEvents = true;
        return process;
    }
}

public static class StringUtils
{
    public static string Quote(string value)
    {
        return "\"" + value + "\"";
    }
}

public static class NuGet
{
    public static void CreatePackage(string pathToMetadata, string outputDirectory)
    {
        string arguments = "pack " + StringUtils.Quote(pathToMetadata) + " -OutputDirectory " + StringUtils.Quote(outputDirectory);
        Command.Execute("nuget", arguments, ".");        
    }
    
    public static void Restore(string projectDirectory)
    {
        var result = Command.Execute("nuget", "restore " + Path.Combine(projectDirectory, "packages.config") + " -PackagesDirectory " + Path.Combine(projectDirectory, @"..\packages"),".");        
    }
}

public static class Internalizer
{
    public static void Internalize(string pathToSourceFile, string frameworkMoniker)
    {
        var source = ReadFile(pathToSourceFile);
        
        // Include source code that matches the framework moniker.    
           
        source = Regex.Replace(source, @"#if.*" + frameworkMoniker + ".*\r\n((.*\r\n)*?)#endif\r\n", "$1");
                
        // Exclude source code that does not match the framework moniker.                        
        source = Regex.Replace(source, @"#if.*\r\n((.*\r\n)*?)#endif\r\n","");
                                
        // Make all public classes internal
        source = Regex.Replace(source, "public (.*class |struct |interface )", "internal $1");
                        
        // Exclude classes from code coverage
        source = Regex.Replace(source, @"(([^\S\r\n]*)internal.*class.*)", "$2[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]\r\n$1");

        // Update version information with the framework moniker.        
        source = Regex.Replace(source, @"(LightInject version \S*)", "$1 (" + frameworkMoniker + ")");
                        
        WriteFile(pathToSourceFile, source);
    }
}


    
   

   



   
