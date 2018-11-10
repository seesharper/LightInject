#load "nuget:Dotnet.Build, 0.3.9"
using static FileUtils;
using System.Xml.Linq;

var owner = "seesharper";
var projectName = "LightInject";
var root = FileUtils.GetScriptFolder();
var solutionFolder = Path.Combine(root,"..","src");

var repoFolder  = Path.Combine(root, "..");

var projectFolder = Path.Combine(root, "..", "src", "LightInject");

var testProjectFolder = Path.Combine(root, "..", "src", "LightInject.Tests");

var pathToTestAssembly = Path.Combine(testProjectFolder, "bin","release", "net46", "LightInject.Tests.dll");


var artifactsFolder = CreateDirectory(root, "Artifacts");
var gitHubArtifactsFolder = CreateDirectory(artifactsFolder, "GitHub");
var nuGetArtifactsFolder = CreateDirectory(artifactsFolder, "NuGet");

string[] exceptTheseTypes = new string[] {
		"IProxy",
		"IInvocationInfo",
		"IMethodBuilder",
		"IDynamicMethodSkeleton",
		"IProxyBuilder",
		"IInterceptor",
		"MethodInterceptorFactory",
		"TargetMethodInfo",
		"OpenGenericTargetMethodInfo",
		"DynamicMethodBuilder",
		"CachedMethodBuilder",
		"TargetInvocationInfo",
		"InterceptorInvocationInfo",
		"CompositeInterceptor",
		"InterceptorInfo",
		"ProxyDefinition"
		};

var pathToReleaseNotes = Path.Combine(gitHubArtifactsFolder, "ReleaseNotes.md");

var version = ReadVersion();

var pathToGitHubReleaseAsset = Path.Combine(gitHubArtifactsFolder, $"{projectName}.{version}.zip");

string ReadVersion()
{
    var projectFile = XDocument.Load(Directory.GetFiles(projectFolder, "*.csproj").Single());
    var versionPrefix = projectFile.Descendants("VersionPrefix").SingleOrDefault()?.Value;
    var versionSuffix = projectFile.Descendants("VersionSuffix").SingleOrDefault()?.Value;
	var version = projectFile.Descendants("Version").SingleOrDefault()?.Value;

	if (version != null)
	{
		return version;
	}


    if (versionSuffix != null)
    {
        return $"{versionPrefix}-{versionSuffix}";
    }
    else
    {
        return versionPrefix;
    }
}