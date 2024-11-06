#load "nuget:Dotnet.Build, 0.16.1"
#load "nuget:github-changelog, 0.1.5"
#load "nuget:dotnet-steps, 0.0.2"


using System.Xml.Linq;
using static ChangeLog;
using static ReleaseManagement;


[StepDescription("Runs the tests with test coverage")]
Step testcoverage = () =>
{
    DotNet.TestWithCodeCoverage(Path.GetDirectoryName(BuildContext.TestProjects[0]), BuildContext.TestCoverageArtifactsFolder, BuildContext.CodeCoverageThreshold, "net8.0");
};

[StepDescription("Runs all the tests for all target frameworks")]
Step test = () =>
{
    Test();
    // DotNet.Test();
};

public static void Test()
{
    var testProjects = BuildContext.TestProjects;
    foreach (var testProject in testProjects)
    {
        var projectFile = XDocument.Load(testProject);
        var testTargetFrameworks = projectFile.Descendants("TestTargetFrameworks").FirstOrDefault()?.Value;
        if (testTargetFrameworks != null)
        {
            foreach (var testTargetFramework in testTargetFrameworks.Split(";"))
            {
                Command.Execute("dotnet", $"test {testProject} --configuration Release /p:TestTargetFramework={testTargetFramework}");
            }
        }
        else
        {
            Command.Execute("dotnet", "test " + testProject + " --configuration Release");
        }
    }
}


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



