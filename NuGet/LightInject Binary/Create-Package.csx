#load "..\Common.csx"

Console.WriteLine("Start building LightInject Binary Package");
string currentDirectory = Directory.GetCurrentDirectory();
string buildDirectory = Path.Combine(currentDirectory, "build");
string inputFile = Path.Combine(currentDirectory, @"..\..\LightInject\ServiceContainer.cs");



Directory.CreateDirectory(buildDirectory);

Console.WriteLine("Current build directory is : {0}", buildDirectory);

SourceWriter.Write("NET", inputFile, Path.Combine(buildDirectory, "ServiceContainer.cs.tmp"), false); 

Publicizer.Write(Path.Combine(buildDirectory, "ServiceContainer.cs.tmp"), Path.Combine(buildDirectory, "ServiceContainer.cs"));

Compiler.Compile(buildDirectory, "LightInject");