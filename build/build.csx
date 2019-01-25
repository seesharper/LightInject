#load "nuget:Dotnet.Build, 0.3.9"
#load "nuget:github-changelog, 0.1.5"
#load "BuildContext.csx"
using static FileUtils;
using static Internalizer;
using static xUnit;
using static DotNet;
using static ChangeLog;
using static ReleaseManagement;
using System.Text.RegularExpressions;

Build(projectFolder);
Test(testProjectFolder);
AnalyzeCodeCoverage(pathToTestAssembly, $"+[{projectName}]*");
Pack(projectFolder, nuGetArtifactsFolder, Git.Default.GetCurrentShortCommitHash());

using (var sourceRepoFolder = new DisposableFolder())
{
    string pathToSourceProjectFolder = Path.Combine(sourceRepoFolder.Path,"src","LightInject");
    string pathToSourceFile = Path.Combine(pathToSourceProjectFolder, "LightInject.cs");
    Copy(repoFolder, sourceRepoFolder.Path, new [] {".vs", "obj"});
    Internalize(pathToSourceProjectFolder, exceptTheseTypes);
    DotNet.Build(Path.Combine(sourceRepoFolder.Path,"src","LightInject"));
    using(var nugetPackFolder = new DisposableFolder())
    {
        Copy("LightInject.Source.nuspec", nugetPackFolder.Path);
        PrepareSourceFile(nugetPackFolder.Path, pathToSourceFile, "netcoreapp2.0");
        PrepareSourceFile(nugetPackFolder.Path, pathToSourceFile, "netstandard2.0");
        PrepareSourceFile(nugetPackFolder.Path, pathToSourceFile, "net46");
        NuGet.Pack(nugetPackFolder.Path, nuGetArtifactsFolder, version);
    }
}

if (BuildEnvironment.IsSecure)
    {
        await CreateReleaseNotes();

        if (Git.Default.IsTagCommit())
        {
            Git.Default.RequreCleanWorkingTree();
            await ReleaseManagerFor(owner, projectName,BuildEnvironment.GitHubAccessToken)
            .CreateRelease(Git.Default.GetLatestTag(), pathToReleaseNotes, Array.Empty<ReleaseAsset>());
            NuGet.TryPush(nuGetArtifactsFolder);
        }
    }

private async Task CreateReleaseNotes()
{
    Logger.Log("Creating release notes");
    var generator = ChangeLogFrom(owner, projectName, BuildEnvironment.GitHubAccessToken).SinceLatestTag();
    if (!Git.Default.IsTagCommit())
    {
        generator = generator.IncludeUnreleased();
    }
    await generator.Generate(pathToReleaseNotes, FormattingOptions.Default.WithPullRequestBody());
}

private void PrepareSourceFile(string nugetPackFolder, string pathToSourceFile ,string targetFramework)
{
    var contentFolder = CreateDirectory(nugetPackFolder, "contentFiles", "cs", targetFramework, "LightInject");
    string pathToSourceFileTemplate = Path.Combine(contentFolder, "LightInject.cs.pp");
    Copy(pathToSourceFile, pathToSourceFileTemplate);
    var frameworkConstant = targetFramework.ToUpper().Replace(".","_");
    var lines = File.ReadAllLines(pathToSourceFileTemplate).ToList();
    lines.Insert(0, $"#define {frameworkConstant}");
    File.WriteAllLines(pathToSourceFileTemplate, lines);
    FileUtils.ReplaceInFile(@"namespace \S*", $"namespace $rootnamespace$.{projectName}", pathToSourceFileTemplate);
}