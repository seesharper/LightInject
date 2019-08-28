#load "nuget:Dotnet.Build, 0.7.1"
#load "nuget:github-changelog, 0.1.5"
#load "nuget:dotnet-steps, 0.0.1"
#load "BuildContext.csx"

using static ChangeLog;
using static ReleaseManagement;


[StepDescription("Runs the tests with test coverage")]
Step testcoverage = () =>
{
    DotNet.TestWithCodeCoverage(projectName, testProjectFolder, coverageArtifactsFolder, targetFramework: "netcoreapp2.0", threshold: 90);
};

[StepDescription("Runs all the tests for all target frameworks")]
Step test = () =>
{
    DotNet.Test(testProjectFolder);
};

[StepDescription("Creates the NuGet packages")]
Step pack = () =>
{
    test();
    testcoverage();
    DotNet.Pack(projectFolder, nuGetArtifactsFolder, Git.Default.GetCurrentShortCommitHash());
    NuGet.CreateSourcePackage(repoFolder, projectName, nuGetArtifactsFolder);
};

[DefaultStep]
[StepDescription("Deploys packages if we are on a tag commit in a secure environment.")]
AsyncStep deploy = async () =>
{
    pack();
    if (!BuildEnvironment.IsSecure)
    {
        Logger.Log("Deployment can only be done in a secure environment");
        return;
    }

    await CreateReleaseNotes();

    if (Git.Default.IsTagCommit())
    {
        Git.Default.RequireCleanWorkingTree();
        await ReleaseManagerFor(owner, projectName, BuildEnvironment.GitHubAccessToken)
        .CreateRelease(Git.Default.GetLatestTag(), pathToReleaseNotes, Array.Empty<ReleaseAsset>());
        NuGet.TryPush(nuGetArtifactsFolder);
    }
};


await StepRunner.Execute(Args);
return 0;


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
