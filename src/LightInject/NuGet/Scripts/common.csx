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
    public static void Build(string pathToProjectFile)
    {        
        Command.Execute("cmd.exe","/C dnu.cmd --version " , ".");
        Command.Execute("cmd.exe","/C dnu.cmd restore " + pathToProjectFile, ".");        
        Command.Execute("cmd.exe","/C dnu.cmd build " + pathToProjectFile + " --configuration Release" , ".");   
    }        
}


public class VersionInfo
{
    public string PackageName {get; set;}
    public string Version {get;set;}
}

public static void RoboCopy(string source, string destination, string arguments = null)
{
    Command.Execute("robocopy", string.Format("{0} {1} {2}", source, destination, arguments));    
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

public static string Format(string message, params object[] arguments)
{
    StringBuilder sb = new StringBuilder();
    sb.Append('\t', depth -1);
    sb.Append(string.Format(message, arguments));    
    return sb.ToString();
}

public static string Format(string message,int indentation, params object[] arguments)
{
    StringBuilder sb = new StringBuilder();
    sb.Append('\t', indentation);
    sb.Append(string.Format(message, arguments));    
    return sb.ToString();
}

public static void CopyFiles(string sourceDirectory, 
                      string destinationDirectory,                       
                      string searchPattern)
{
    DirectoryInfo source = new DirectoryInfo(sourceDirectory);
    DirectoryInfo destination = new DirectoryInfo(destinationDirectory);

    FileInfo[] files = source.GetFiles(searchPattern);

    //this section is what's really important for your application.
    foreach (FileInfo file in files)
    {
        file.CopyTo(destination.FullName + "\\" + file.Name, true);
    }
}
public static string GetNugetVersionNumber(string pathToNugetSpecification)
{
    using(StreamReader reader = new StreamReader(pathToNugetSpecification))
    {
        string nuspec = reader.ReadToEnd();
        var match = Regex.Match(nuspec, @"<version>(.+)<\/version>");
        return match.Groups[1].Value;
    }
}

public static void PatchAssemblyVersionInfo(string version, string frameworkMoniker, string pathToAssemblyInfo)
{    
    var assemblyInfo = ReadFile(pathToAssemblyInfo);    
    var pachedAssemblyInfo = Regex.Replace(assemblyInfo, @"Version\(""(.+?"")", string.Format(@"Version(""{0}""", version));    
    pachedAssemblyInfo = Regex.Replace(pachedAssemblyInfo, @"(AssemblyCopyright\(""\D+)(\d*)", "${1}" + DateTime.Now.Year);    
    WriteFile(pathToAssemblyInfo, pachedAssemblyInfo);    
}




public static void CreatePackage(string pathToSpecification, string outputDirectory)
{
    string arguments = "pack " + StringUtils.Quote(pathToSpecification) + " -OutputDirectory " + StringUtils.Quote(outputDirectory);
    var result = Command.Execute("nuget.exe", arguments); 
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




public static class VersionUtils
{
    public static string GetVersionString(string pathToSourceFile)
    {
        using (var reader = new StreamReader(pathToSourceFile))
        {
           while(!reader.EndOfStream)
           {
               var line = reader.ReadLine();
               if (line.Contains(" version "))
               {
                    return line.Substring(line.IndexOf(" version ") + 9).Trim();
               }
           }
        }
        throw new InvalidOperationException("version string not found");
    }

    public static void UpdateNuGetPackageSpecification(string pathToPackageSpecification, string version, Dictionary<string, string> dependencies)
    {
        Console.WriteLine("[VersionUtils] Updating {0} to version {1}", pathToPackageSpecification, version);
        var tempFile = Path.GetTempFileName();
        using (var reader = new StreamReader(pathToPackageSpecification))
        {
            using (var writer = new StreamWriter(tempFile))
            {
                while(!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (line.Contains("<version>"))
                    {
                        line = string.Format("        <version>{0}</version>", version);
                    }           

                    if (line.Contains("<dependency id"))    
                    {
                        XElement element = XElement.Parse(line);
                        var idAttribute = element.Attribute("id");
                        if (dependencies.ContainsKey(idAttribute.Value))
                        {
                            var versionAttribute = element.Attribute("version");
                            versionAttribute.Value = dependencies[idAttribute.Value];
                        }
                        line = "            " + element.ToString();
                    }

                    writer.WriteLine(line);
                } 
           }           
        }

        File.Copy(tempFile, pathToPackageSpecification, true);
    }


    public static void UpdateAssemblyInfo(string pathToAssemblyInfoFile, string version)
    {        
        var tempFile = Path.GetTempFileName();
        using (var reader = new StreamReader(pathToAssemblyInfoFile))
        {
            using (var writer = new StreamWriter(tempFile))
            {
                while(!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (line.Contains("[assembly: AssemblyVersion"))
                    {
                        line = string.Format("[assembly: AssemblyVersion(\"{0}\")]", version);
                    }
                    if (line.Contains("[assembly: AssemblyFileVersion"))
                    {
                        line = string.Format("[assembly: AssemblyFileVersion(\"{0}\")]", version);
                    }
                    if (!line.Contains("InternalsVisibleTo"))    
                    {
                        writer.WriteLine(line);
                    }
                } 
           }           
        }

        File.Copy(tempFile, pathToAssemblyInfoFile, true);
    }


}

 public static void DeleteAllPackages(string directory)
    {
        foreach(var file in Directory.GetFiles(directory, "*.nupkg"))
        {            
            File.Delete(file);
        }
    }

public static class DirectoryUtils
{   
    public static void Delete(string path)
    {
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



   


    public static void DeleteContentFolder(string currentDirectory)
    {
        string contentFolder = Path.Combine(currentDirectory , @"package\content");
        if (Directory.Exists(contentFolder))
        {
            Directory.Delete(contentFolder, true);
        }
    }

    public static void DeleteLibFolder(string currentDirectory)
    {
        string libFolder = Path.Combine(currentDirectory , @"package\lib");
        if (Directory.Exists(libFolder))
        {
            Directory.Delete(libFolder, true);
        }
    }

    public static void DeleteBuildFolder(string currentDirectory)
    {
        string buildFolder = Path.Combine(currentDirectory , @"build");
        if (Directory.Exists(buildFolder))
        {
            Directory.Delete(buildFolder, true);
        }
    }
}

public class FileUtils
{
    public static void CopyAll(string sourceDirectory, string targetDirectory)
    {
        Directory.CreateDirectory(targetDirectory);
        foreach(var file in Directory.GetFiles(sourceDirectory))
        {
            File.Copy(file, Path.Combine(targetDirectory, Path.GetFileName(file)));    
        }        
    }

    public static void DeleteAllTempFiles(string directory)
    {
        foreach(var file in Directory.GetFiles(directory, "*.tmp"))
        {
            File.Delete(file);
        }

        foreach(var file in Directory.GetFiles(directory, "*.cs"))
        {
            File.Delete(file);
        }
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
    public static void CreatePackage(string pathToNuget, string pathToSpecification, string outputDirectory)
    {
        string arguments = "pack " + StringUtils.Quote(pathToSpecification) + " -OutputDirectory " + StringUtils.Quote(outputDirectory);
        var result = Command.Execute(pathToNuget, arguments);
        Console.WriteLine(result);
    }
    
    public static void Restore(string projectDirectory)
    {
        var result = Command.Execute("nuget", "restore " + Path.Combine(projectDirectory, "packages.config") + " -PackagesDirectory " + Path.Combine(projectDirectory, @"..\packages"),".");
        
    }
}





public class DirectiveEvaluator
{
    public bool Execute(string directive, string expression)
    {
        string[] subExpressions =
            expression.Split(new[] { "||" }, StringSplitOptions.None).Select(e => e.Trim()).ToArray();

        return subExpressions.Any(subExpression => directive == subExpression);
    }
}

public class SourceWriter
{
    private static DirectiveEvaluator directiveEvaluator = new DirectiveEvaluator();

    public static void Write(string directive, string inputFile, string outputFile, bool processNameSpace, bool excludeFromCodeCoverage)
    {            
        using (var reader = new StreamReader(inputFile))
        {
            using (var writer = new StreamWriter(outputFile))
            {
                Write(directive, reader, writer, processNameSpace, excludeFromCodeCoverage);                    
            }
        }     
    }



    public static void Write(string directive, StreamReader reader, StreamWriter writer, bool processNameSpace, bool excludeFromCodeCoverage)
    {
        bool shouldWrite = true;                        
        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();
            if (line.StartsWith("#if"))
            {
                shouldWrite = directiveEvaluator.Execute(directive, line.Substring(4));
                continue;
            }

            if (line.StartsWith("#endif"))
            {
                shouldWrite = true;
                continue;
            }


            if (shouldWrite)
            {
                if (line.Contains("namespace") && processNameSpace)
                {
                    line = line.Insert(10, "$rootnamespace$.");
                }

                if (line.Contains("typeof(LightInject.Web.LightInjectHttpModuleInitializer)") && processNameSpace)
                {
                    line = line.Replace("typeof(LightInject.Web.LightInjectHttpModuleInitializer)", "typeof($rootnamespace$.LightInject.Web.LightInjectHttpModuleInitializer)");
                }

                if (line.Contains("typeof(LightInject.Wcf.LightInjectWcfInitializer)") && processNameSpace)
                {
                    line = line.Replace("typeof(LightInject.Wcf.LightInjectWcfInitializer)", "typeof($rootnamespace$.LightInject.Wcf.LightInjectWcfInitializer)");
                }

                if (line.Contains("LightInject.Wcf.LightInjectServiceHostFactory, LightInject.Wcf") && processNameSpace)
                {
                    line = line.Replace("LightInject.Wcf.LightInjectServiceHostFactory, LightInject.Wcf", "$rootnamespace$.LightInject.Wcf.LightInjectServiceHostFactory, $assemblyname$");
                }

                

                if (line.Contains("public class") || line.Contains("internal class") || line.Contains("internal static class"))
                {                        
                    var lineWithOutIndent = line.TrimStart(new char[] { ' ' });
                    var indentLength = line.Length - lineWithOutIndent.Length;
                    var indent = line.Substring(0, indentLength);
                    
                            
                    if (excludeFromCodeCoverage)
                    {
                        writer.WriteLine("{0}{1}", indent, "[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]");                        
                    }
                    
                }

                
                if (!reader.EndOfStream)
                {
                    writer.WriteLine(line);
                }
                else
                {
                    writer.Write(line);
                }

            }
        }            
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
        source = Regex.Replace(source, "public (.*class|struct|interface)", "internal $1");
        
        // Exclude classes from code coverage
        source = Regex.Replace(source, @"(([^\S\r\n]*)internal.*class.*)", "$2[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]\r\n$1");
                        
        WriteFile(pathToSourceFile, source);
    }
}


public class Publicizer
{
    private static readonly List<string> Exceptions = new List<string>();
    private static readonly List<string> Includes = new List<string>();
   

    static Publicizer()
    {
       

        Exceptions.Add("TypeHelper");
        Exceptions.Add("ReflectionHelper"); 
        Exceptions.Add("LifetimeHelper"); 
        Exceptions.Add("NamedDelegateTypeExtensions"); 
        Exceptions.Add("DelegateTypeExtensions"); 
        Exceptions.Add("LazyTypeExtensions");
        Exceptions.Add("EnumerableTypeExtensions");
        Exceptions.Add("FuncTypeExtensions");
        //Exceptions.Add("OpCodes");
        //Exceptions.Add("OpCode");
        //Exceptions.Add("ILGenerator");        
        Exceptions.Add("DynamicMethodSkeleton");
        //Exceptions.Add("DynamicMethod");
        //Exceptions.Add("MethodInfoExtensions");
       


        Includes.Add("internal struct OpCode");
        Includes.Add("internal static class");
        Includes.Add("internal class");
        Includes.Add("internal sealed class");
        Includes.Add("internal interface");
        Includes.Add("internal abstract class");
        Includes.Add("internal LightInjectServiceLocator");
        Includes.Add("internal static void SetServiceContainer");
        Includes.Add("internal LightInjectMvcDependencyResolver");
                

    }
    
    public static void Write(string directive, string inputFile, string outputFile)
     {
         var tempFile = Path.GetTempFileName();
         SourceWriter.Write(directive, inputFile, tempFile, false, false);         
         using (var reader = new StreamReader(tempFile))
         {             
             using (var writer = new StreamWriter(outputFile))
             {
                 Write(reader, writer);
             }
         }             
     }

    public static void Write(StreamReader reader, StreamWriter writer)
    {
         while (!reader.EndOfStream)
         {
             var line = reader.ReadLine();
             if (Includes.Any(i => line.Contains(i)) && !Exceptions.Any(e => line.Contains(e)))
             {
                 line = line.Replace("internal", "public");                 
             }
             
             if (line.Contains(@"Gets or sets the parent <see cref=""Scope""/>."))
             {
                line = line.Replace("Gets or sets", "Gets");
             }

             if (line.Contains(@"Gets or sets the child <see cref=""Scope""/>."))
             {
                line = line.Replace("Gets or sets", "Gets");
             }   

             writer.WriteLine(line);
         }
    }
}

public class AssemblyInfoWriter
{
    private static readonly List<string> Exceptions = new List<string>();

    static AssemblyInfoWriter()
    {
        Exceptions.Add("InternalsVisibleTo");                        
    }

    public static void Write(string inputFile, string outputFile, string version)
     {
         var tempFile = Path.GetTempFileName();
         using (var reader = new StreamReader(inputFile))
         {             
             using (var writer = new StreamWriter(tempFile))
             {
                 Write(reader, writer, version);
             }
         }    


     }

    public static void Write(StreamReader reader, StreamWriter writer, string version)
    {
         while (!reader.EndOfStream)
         {
             var line = reader.ReadLine();
             if (!Exceptions.Any(e => line.Contains(e)))
             {
                 writer.WriteLine(line);
             }             
         }
    }

    
}
   
