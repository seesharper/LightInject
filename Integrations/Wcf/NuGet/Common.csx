//MsTest.CreateSummaryFile(@"C:\Users\bri\Documents\GitHub\LightInject\NuGet\Scripts\TestResults\00b9623d-be49-4ebf-87c0-9ba64340c33c\bri_BRI-8760W 2014-05-08 22_02_53.coveragexml",
  //  @"C:\Users\bri\Documents\GitHub\LightInject\NuGet\Scripts\TestResults\00b9623d-be49-4ebf-87c0-9ba64340c33c");

//MsTest.ConvertCoverageFileToXml(@"C:\Users\bri\Documents\GitHub\LightInject\NuGet\Scripts\TestResults\00b9623d-be49-4ebf-87c0-9ba64340c33c\bri_BRI-8760W 2014-05-08 22_02_53.coverage");





using System.Diagnostics;
using System.Xml.Linq;
using System.Text.RegularExpressions;

public class VersionInfo
{
    public string PackageName {get; set;}
    public string Version {get;set;}
}

public static void RoboCopy(string source, string destination, string arguments = null)
{
    Command.Execute("robocopy", string.Format("{0} {1} {2}", source, destination, arguments));
}

public static void Restore(string projectDirectory)
{
    Command.Execute("nuget", "restore " + Path.Combine(projectDirectory, "packages.config") + " -PackagesDirectory " + Path.Combine(projectDirectory, @"..\packages"));
}

public static void RemoveDirectory(string directory)
{
    if (Directory.Exists(directory))
    {
        Directory.Delete(directory, true);
    }
}

private static string CopyToNuGetBuildDirectory(string projectPath)
{
    var projectDirectory = Path.GetDirectoryName(projectPath);
    var tempSolutionFolder = GetTemporaryDirectory();
    string projectName = Regex.Match(projectPath, @"..\\(.+)").Groups[1].Value;
    var tempProjectFolder = Path.Combine(tempSolutionFolder, projectName);
    RoboCopy(projectPath, tempProjectFolder, "/S /XD obj bin");
    Restore(tempProjectFolder);
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
    Console.Write(description);
    Stopwatch watch = new Stopwatch();
    watch.Start();
    action();
    watch.Stop();
    Console.WriteLine("...Done! ({0} milliseconds)", watch.ElapsedMilliseconds);
}

public static T Execute<T>(Func<T> func, string description)
{
    Console.Write(description);
    Stopwatch watch = new Stopwatch();
    watch.Start();
    var result = func();
    watch.Stop();
    Console.WriteLine("...Done! ({0} milliseconds)", watch.ElapsedMilliseconds);
    return result;
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

public static string PatchAssemblyVersionInfo(string version, string assemblyInfo)
{    
    return Regex.Replace(assemblyInfo, @"Version\(""(.+?"")", string.Format(@"Version(""{0}""", version));    
}


public static void Build(string pathToSolutionFile)
{
    string pathToMsBuild = @"C:\Windows\Microsoft.NET\Framework\v4.0.30319\MsBuild.exe";
    Command.Execute(pathToMsBuild, pathToSolutionFile + " /property:Configuration=Release /verbosity:quiet /nologo");     
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
        string pathToMsBuild = @"C:\Program Files (x86)\MSBuild\12.0\Bin\MsBuild.exe";
        Command.Execute(pathToMsBuild, pathToSolutionFile + " /property:Configuration=Release /p:VisualStudioVersion=12.0 /verbosity:quiet /nologo");     
    }
}

public static class MsTest
{
    public static void Run(string pathToTestAssembly)
    {
        string pathToMsTest = @"C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe";
        string result = Command.Execute(pathToMsTest, pathToTestAssembly);
    }

    public static void RunWithCodeCoverage(string pathToTestAssembly, params string[] includedAssemblies)
    {
        string pathToMsTest = @"C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe";
        string result = Command.Execute(pathToMsTest, pathToTestAssembly + " /Enablecodecoverage");

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
        var doc = XDocument.Load(pathToSummaryFile);               
        var coverage = doc.Root.Elements().Single( e => e.Name.LocalName == "Summary").Elements().Single (e => e.Name.LocalName == "Coverage").Value;
        if (coverage != "100%")
        {
            Console.WriteLine(doc);
            throw new InvalidOperationException("Deploy failed. Test coverage is only " + coverage);
        }        
    }


    public static void ConvertCoverageFileToXml(string pathToCoverageFile)
    {
        Command.Execute(@"..\CoverageToXml\CoverageToXml\bin\release\CoverageToXml.exe", StringUtils.Quote(pathToCoverageFile));
    }

    public static void CreateSummaryFile(string pathToCoverageXmlFile, string directory, string[] includedAssemblies)
    {
        
        var filters = includedAssemblies.Select (a => "+" + a).Aggregate ((current, next) => current + ";" + next).ToLower();

        Command.Execute(@"..\..\packages\ReportGenerator.1.9.1.0\ReportGenerator.exe", "-reports:" + StringUtils.Quote(pathToCoverageXmlFile) + " -targetdir:" + StringUtils.Quote(directory)
            + " -reporttypes:xmlsummary -filters:" + filters );        
    }


    public static string GetPathToCoverageFile(string result)
    {
        result = result.Substring(result.IndexOf("Attachments:"));
            
        StringReader reader = new StringReader(result);
    
        reader.ReadLine();
    
        return reader.ReadLine().Trim();
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
    public static void Delete(string directory)
    {
         if (Directory.Exists(directory))
        {
            Directory.Delete(directory, true);
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
    public static string Execute(string commandPath, string arguments)
    {
        var startInformation = CreateProcessStartInfo(commandPath, arguments);
        var process = CreateProcess(startInformation);
        var processOutput = process.StandardOutput.ReadToEnd();
        process.WaitForExit();
        if (process.ExitCode != 0 && commandPath != "robocopy")
        {
            throw new InvalidOperationException(processOutput);
            Console.WriteLine(processOutput);
        }     
        return processOutput;
    }

    private static ProcessStartInfo CreateProcessStartInfo(string commandPath, string arguments)
    {
        var startInformation = new ProcessStartInfo(StringUtils.Quote(commandPath));
        startInformation.CreateNoWindow = true;
        startInformation.Arguments =  arguments;
        startInformation.RedirectStandardOutput = true;
        startInformation.UseShellExecute = false;
        return startInformation;
    }

    private static Process CreateProcess(ProcessStartInfo startInformation)
    {
        var process = new Process();
        process.StartInfo = startInformation;
        process.Start();
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
}

public class Compiler
{
	public static void Compile(string sourceDirectory, string outputFileName)
	{
		Console.WriteLine(CreateFileList(sourceDirectory));
		//return;

		var startInformation = CreateProcessStartInfo(sourceDirectory, outputFileName);
        var process = CreateProcess(startInformation);
        var processOutput = process.StandardOutput.ReadToEnd();
        process.WaitForExit();
        Console.WriteLine(processOutput);
        FileUtils.DeleteAllTempFiles(sourceDirectory);
	}

    private static ProcessStartInfo CreateProcessStartInfo(string sourceDirectory, string outputFileName)
    {
        var startInformation = new ProcessStartInfo(@"C:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe");
        startInformation.CreateNoWindow = true;
        startInformation.Arguments =  "/target:library /platform:ARM /warnaserror+ /warn:1 /optimize /debug:pdbonly "  + "/doc:" + Quote(Path.Combine(sourceDirectory,outputFileName + ".xml")) + " /out:" + Quote(Path.Combine(sourceDirectory, outputFileName + ".dll")) +  " " + Quote(Path.Combine(sourceDirectory, "*.cs"));
        Console.WriteLine(startInformation.Arguments);
        startInformation.RedirectStandardOutput = true;
        startInformation.UseShellExecute = false;
        return startInformation;
    }

    private static string Quote(string value)
    {
    	return "\"" + value + "\"";
    }


    private static Process CreateProcess(ProcessStartInfo startInformation)
    {
        var process = new Process();
        process.StartInfo = startInformation;
        process.Start();
        return process;
    }

    private static string CreateFileList(string sourceDirectory)
    {
    	return Directory.GetFiles(sourceDirectory, "*.cs").Aggregate ((current,next) =>  Quote(current) + " " + Quote(next));
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
   
