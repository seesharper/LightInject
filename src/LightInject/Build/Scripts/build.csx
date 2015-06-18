
#load "common.csx"

string pathToSolutionFile = @"..\..\LightInject.sln";
string pathToSourceFile = @"..\..\LightInject\LightInject.cs";

private string version = GetVersionNumberFromSourceFile(pathToSourceFile);

Console.WriteLine("LightInject version {0}" , version);
Console.ForegroundColor = ConsoleColor.Blue;

Execute(() => MsBuild.Build(pathToSolutionFile), "INFO: Building LightInject");



MsBuild.Build(pathToSolutionFile);



Console.WriteLine("TEST");


public class Test
{
		
}