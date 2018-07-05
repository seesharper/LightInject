#load "nuget:Dotnet.Build, 0.3.9"
#load "nuget:github-changelog, 0.1.5"
#load "BuildContext.csx"
using static FileUtils;
using static Internalizer;
using static xUnit;
using static DotNet;
using static ChangeLog;
using static ReleaseManagement;

Build(projectFolder);
Test(testProjectFolder);
AnalyzeCodeCoverage(pathToTestAssembly, $"+[{projectName}]*");
Pack(projectFolder, nuGetArtifactsFolder, Git.Default.GetCurrentShortCommitHash());

using(var sourceBuildFolder = new DisposableFolder())
{
    string pathToSourceProjectFolder = Path.Combine(sourceBuildFolder.Path,"LightInject");
    Copy(solutionFolder, sourceBuildFolder.Path, new [] {".vs", "obj"});
    Internalize(pathToSourceProjectFolder, exceptTheseTypes);    
    DotNet.Build(Path.Combine(sourceBuildFolder.Path,"LightInject"));
    using(var nugetPackFolder = new DisposableFolder())
    {
        var contentFolder = CreateDirectory(nugetPackFolder.Path, "content","net45", "LightInject");
        Copy("LightInject.Source.nuspec", nugetPackFolder.Path);
        string pathToSourceFileTemplate = Path.Combine(contentFolder, "LightInject.cs.pp");
        Copy(Path.Combine(pathToSourceProjectFolder, "LightInject.cs"), pathToSourceFileTemplate);        
        FileUtils.ReplaceInFile(@"namespace \S*", $"namespace $rootnamespace$.{projectName}", pathToSourceFileTemplate);
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