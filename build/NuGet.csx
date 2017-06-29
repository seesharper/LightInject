#load "Command.csx"
#load "Write.csx"
#load "FileUtils.csx"
public static class NuGet
{
    public static void Pack(string pathToMetadata, string outputDirectory)
    {
        string arguments = $"pack \"{pathToMetadata}\" -OutputDirectory \"{outputDirectory}\"";
        Write.Info($"Running NuGet pack with args {arguments}");
        Command.Execute("nuget", arguments, ".");
    }

    public static void Restore(string projectDirectory)
    {
        var result = Command.Execute("nuget", "restore " + Path.Combine(projectDirectory, "packages.config") + " -PackagesDirectory " + Path.Combine(projectDirectory, @"..\packages"), ".");
    }

    public static void Update(string pathToSolutionFile)
    {
        Command.Execute("nuget", "update " + pathToSolutionFile, ".");
    }

    public static void Install(string packageName, string outputDirectory)
    {
        Command.Execute("nuget", $"install {packageName} -OutputDirectory {outputDirectory}", ".");
    }

    public static void PatchNugetVersionInfo(string pathToNugetSpecification, string version)
    {
        FileUtils.ReplaceInFile(@"(<version>)(.+)(<\/version>)", "${1}" + version + "$3", pathToNugetSpecification);
    }
}