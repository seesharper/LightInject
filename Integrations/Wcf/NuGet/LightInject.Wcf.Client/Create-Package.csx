#load "..\Common.csx"

Console.WriteLine("Start building the LightInject.Web NuGet package");
string currentDirectory = Directory.GetCurrentDirectory();

string inputDirectory = Path.Combine(currentDirectory, @"..\..\LightInject.Web");
string inputPath = Path.Combine(inputDirectory, @"LightInjectWeb.cs");
string outputFile = "LightInjectWeb.cs.pp";


string outputDirectory = Path.Combine(currentDirectory, @"package\content\net40");
Directory.CreateDirectory(outputDirectory);
Console.WriteLine("Current output directory is : {0}", outputDirectory);

SourceWriter.Write("NET", inputPath, Path.Combine(outputDirectory, outputFile), true); 

outputDirectory = Path.Combine(currentDirectory, @"package\content\net45");
Directory.CreateDirectory(outputDirectory);
Console.WriteLine("Current output directory is : {0}", outputDirectory);
SourceWriter.Write("NET", inputPath, Path.Combine(outputDirectory, outputFile), true); 

string pathToNuGet = Path.Combine(currentDirectory, @"..\NuGet.exe");
string pathSpecification = Path.Combine(currentDirectory, @"package\LightInject.Web.nuspec");
string packageOutputDirectory = Path.Combine(currentDirectory , @"..");

NuGet.CreatePackage(pathToNuGet, pathSpecification, packageOutputDirectory);

Console.WriteLine("Finished building the LightInject.Web NuGet package");