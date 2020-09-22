#load "nuget:Dotnet.Build, 0.11.1"
#load "nuget:github-changelog, 0.1.5"
#load "nuget:dotnet-steps, 0.0.2"


using static ChangeLog;
using static ReleaseManagement;


[StepDescription("Runs the tests with test coverage")]
Step testcoverage = () =>
{

    DotNet.TestWithCodeCoverage(Path.GetDirectoryName(BuildContext.TestProjects[0]), BuildContext.TestCoverageArtifactsFolder, BuildContext.CodeCoverageThreshold, "netcoreapp2.0");
};

[StepDescription("Runs all the tests for all target frameworks")]
Step test = () =>
{
    DotNet.Test();
};

[StepDescription("Creates the NuGet packages")]
Step pack = () =>
{
    test();
    testcoverage();
    DotNet.Pack();
    NuGetUtils.CreateSourcePackage(BuildContext.RepositoryFolder, BuildContext.ProjectName, BuildContext.NuGetArtifactsFolder);
};

[DefaultStep]
[StepDescription("Deploys packages if we are on a tag commit in a secure environment.")]
AsyncStep deploy = async () =>
{
    pack();
    await Artifacts.Deploy();
};


await StepRunner.Execute(Args);
return 0;



