#load "..\Common.csx"

Console.WriteLine("Start building the LightInject.Annotation NuGet package");
string currentDirectory = Directory.GetCurrentDirectory();

string inputDirectory = Path.Combine(currentDirectory, @"..\..\LightInject.Annotation");
string inputPath = Path.Combine(inputDirectory, @"LightInject.Annotation.cs");
string outputFile = "LightInject.Annotation.cs.pp";

DirectoryUtils.DeleteContentFolder(currentDirectory);

string outputDirectory = Path.Combine(currentDirectory, @"package\content\net40\LightInject\Annotation");
Directory.CreateDirectory(outputDirectory);
Console.WriteLine("Current output directory is : {0}", outputDirectory);

SourceWriter.Write("NET", inputPath, Path.Combine(outputDirectory, outputFile), true); 

outputDirectory = Path.Combine(currentDirectory, @"package\content\net45\LightInject\Annotation");
Directory.CreateDirectory(outputDirectory);
Console.WriteLine("Current output directory is : {0}", outputDirectory);
SourceWriter.Write("NET", inputPath, Path.Combine(outputDirectory, outputFile), true); 

outputDirectory = Path.Combine(currentDirectory, @"package\content\netcore45\LightInject\Annotation");
Directory.CreateDirectory(outputDirectory);
Console.WriteLine("Current output directory is : {0}", outputDirectory);
SourceWriter.Write("NETFX_CORE", inputPath, Path.Combine(outputDirectory, outputFile), true); 


string pathToNuGet = Path.Combine(currentDirectory, @"..\NuGet.exe");
string pathSpecification = Path.Combine(currentDirectory, @"package\LightInject.Annotation.nuspec");
string packageOutputDirectory = Path.Combine(currentDirectory , @"..");

NuGet.CreatePackage(pathToNuGet, pathSpecification, packageOutputDirectory);

Console.WriteLine("Finished building the LightInject.Annotation NuGet package");