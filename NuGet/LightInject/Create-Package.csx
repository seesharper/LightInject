#load "..\Common.csx"

Console.WriteLine("Start building the LightInject NuGet package");
string currentDirectory = Directory.GetCurrentDirectory();

Directory.Delete(Path.Combine(currentDirectory, @"package\content"), true);

string inputDirectory = Path.Combine(currentDirectory, @"..\..\LightInject");
string inputPath = Path.Combine(inputDirectory, @"LightInject.cs");
string outputFile = "LightInject.cs.pp";


string outputDirectory = Path.Combine(currentDirectory, @"package\content\net40\LightInject");
Directory.CreateDirectory(outputDirectory);
Console.WriteLine("Current output directory is : {0}", outputDirectory);

SourceWriter.Write("NET", inputPath, Path.Combine(outputDirectory, outputFile), true); 

outputDirectory = Path.Combine(currentDirectory, @"package\content\net45\LightInject");
Directory.CreateDirectory(outputDirectory);
Console.WriteLine("Current output directory is : {0}", outputDirectory);
SourceWriter.Write("NET", inputPath, Path.Combine(outputDirectory, outputFile), true); 

outputDirectory = Path.Combine(currentDirectory, @"package\content\netcore45\LightInject");
Directory.CreateDirectory(outputDirectory);
Console.WriteLine("Current output directory is : {0}", outputDirectory);
SourceWriter.Write("NETFX_CORE", inputPath, Path.Combine(outputDirectory, outputFile), true); 


string pathToNuGet = Path.Combine(currentDirectory, @"..\NuGet.exe");
string pathSpecification = Path.Combine(currentDirectory, @"package\LightInject.nuspec");
string packageOutputDirectory = Path.Combine(currentDirectory , @"..");

NuGet.CreatePackage(pathToNuGet, pathSpecification, packageOutputDirectory);

Console.WriteLine("Finished building the LightInject NuGet package");