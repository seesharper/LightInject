using System.Diagnostics;

public static class DirectoryUtils
{
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
        if (process.ExitCode != 0)
        {
            throw new InvalidOperationException(processOutput);
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
        
    	Console.WriteLine("[SourceWriter] Start processing file '{0}'' with directive '{1}'.", inputFile, directive);
        using (var reader = new StreamReader(inputFile))
        {
            using (var writer = new StreamWriter(outputFile))
            {
                Write(directive, reader, writer, processNameSpace, excludeFromCodeCoverage);                    
            }
        }
        Console.WriteLine("[SourceWriter] Finished processing file '{0}'' with directive '{1}'. Output was written to {2}", inputFile, directive, outputFile);        
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
        //Exceptions.Add("OpCodes");
        //Exceptions.Add("OpCode");
        //Exceptions.Add("ILGenerator");        
        Exceptions.Add("DynamicMethodSkeleton");
        Exceptions.Add("DynamicMethod");
        //Exceptions.Add("MethodInfoExtensions");
       


        Includes.Add("internal struct OpCode");
        Includes.Add("internal static class");
        Includes.Add("internal class");
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
         Console.WriteLine("[Publicizer] Start processing file '{0}''.", tempFile);
         using (var reader = new StreamReader(tempFile))
         {             
             using (var writer = new StreamWriter(outputFile))
             {
                 Write(reader, writer);
             }
         }    
         Console.WriteLine("[Publicizer] Finished processing file '{0}'. Output was written to {1}", tempFile, outputFile);        
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

    public static void Write(string inputFile, string outputFile)
     {
         using (var reader = new StreamReader(inputFile))
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
             if (!Exceptions.Any(e => line.Contains(e)))
             {
                 writer.WriteLine(line);
             }             
         }
    }

    
}