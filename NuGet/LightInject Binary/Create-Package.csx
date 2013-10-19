#load "..\Common.csx"

Console.WriteLine("Start building LightInject Binary Package");
string currentDirectory = Directory.GetCurrentDirectory();
string buildDirectory = Path.Combine(currentDirectory, "build");
string sourceFile = Path.Combine(currentDirectory, @"..\..\LightInject\LightInject.cs");
string assemblyInfoFile = Path.Combine(currentDirectory, @"..\..\LightInject\Properties\AssemblyInfo.cs");

DirectoryUtils.DeleteBuildFolder(currentDirectory);
DirectoryUtils.DeleteLibFolder(currentDirectory);
Directory.CreateDirectory(buildDirectory);


Console.WriteLine("Current build directory is : {0}", buildDirectory);

SourceWriter.Write("NET", sourceFile, Path.Combine(buildDirectory, "LightInject.cs.tmp"), false); 
Publicizer.Write(Path.Combine(buildDirectory, "LightInject.cs.tmp"), Path.Combine(buildDirectory, "LightInject.cs"));
AssemblyInfoWriter.Write(assemblyInfoFile, Path.Combine(buildDirectory, "AssemblyInfo.cs"));
Compiler.Compile(buildDirectory, "LightInject");

FileUtils.CopyAll(buildDirectory, Path.Combine(currentDirectory, @"Package\lib\net40"));
FileUtils.CopyAll(buildDirectory, Path.Combine(currentDirectory, @"Package\lib\net45"));

string pathToNuGet = Path.Combine(currentDirectory, @"..\NuGet.exe");
string pathSpecification = Path.Combine(currentDirectory, @"package\LightInject.nuspec");
string packageOutputDirectory = Path.Combine(currentDirectory , @"..");

NuGet.CreatePackage(pathToNuGet, pathSpecification, packageOutputDirectory);


Console.WriteLine("Finished building LightInject Binary Package");