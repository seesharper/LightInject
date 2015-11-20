



#load "common.csx"

private const string projectName = "LightInject.Annotation";

private const string portableClassLibraryProjectTypeGuid = "{786C830F-07A1-408B-BD7F-6EE04809D6DB}";
private const string csharpProjectTypeGuid = "{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}";

string pathToSourceFile = @"..\..\LightInject.Annotation\LightInject.Annotation.cs";
string pathToBuildDirectory = @"../tmp/";
private string version = GetVersionNumberFromSourceFile(pathToSourceFile);
private string fileVersion = Regex.Match(version, @"(^[\d\.]+)-?").Groups[1].Captures[0].Value;



   


WriteLine(fileVersion);

WriteLine("LightInject.Annotation version {0}" , version);


Execute(() => InitializBuildDirectories(), "Preparing build directories");
Execute(() => RenameSolutionFiles(), "Renaming solution files");
Execute(() => PatchAssemblyInfo(), "Patching assembly information");
Execute(() => PatchProjectFiles(), "Patching project files");
Execute(() => PatchPackagesConfig(), "Patching packages config");
Execute(() => InternalizeSourceVersions(), "Internalizing source versions");
Execute(() => RestoreNuGetPackages(), "NuGet");
Execute(() => BuildAllFrameworks(), "Building all frameworks");
Execute(() => RunAllUnitTests(), "Running unit tests");
//Execute(() => AnalyzeTestCoverage(), "Analyzing test coverage");
Execute(() => InitializeNuGetPackageDirectories(), "Preparing NuGet build directories");




private void InitializeNuGetPackageDirectories()
{
	string pathToNuGetBuildDirectory = Path.Combine(pathToBuildDirectory, "NuGetPackages");
	DirectoryUtils.Delete(pathToNuGetBuildDirectory);
	
		
	Execute(() => CopySourceFilesToNuGetLibDirectory(), "Copy source files to NuGet lib directory");		
	Execute(() => CopyBinaryFilesToNuGetLibDirectory(), "Copy binary files to NuGet lib directory");
	
	Execute(() => CreateSourcePackage(), "Creating source package");		
	Execute(() => CreateBinaryPackage(), "Creating binary package");		
}

private void CopySourceFilesToNuGetLibDirectory()
{
	CopySourceFile("NET40", "net40");
	CopySourceFile("NET45", "net45");
	CopySourceFile("NET46", "net46");		
	CopySourceFile("DNX451", "dnx451");		
	CopySourceFile("DNXCORE50", "dnxcore50");
	CopySourceFile("PCL_111", "portable-net45+win81+wpa81+MonoAndroid10+MonoTouch10+Xamarin.iOS10");
}

private void CopyBinaryFilesToNuGetLibDirectory()
{
	CopyBinaryFile("NET40", "net40");
	CopyBinaryFile("NET45", "net45");
	CopyBinaryFile("NET46", "net46");	
	CopyBinaryFile("DNX451", "dnx451");
	CopyBinaryFile("DNXCORE50", "dnxcore50");
	CopyBinaryFile("PCL_111", "portable-net45+win81+wpa81+MonoAndroid10+MonoTouch10+Xamarin.iOS10");	
}

private void CreateSourcePackage()
{
	string outputDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
	string pathToMetadataFile = Path.Combine(pathToBuildDirectory, "NugetPackages/Source/package/LightInject.Annotation.Source.nuspec");
	PatchNugetVersionInfo(pathToMetadataFile, version);		
	NuGet.CreatePackage(pathToMetadataFile, outputDirectory);		
}

private void CreateBinaryPackage()
{
	string outputDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
	string pathToMetadataFile = Path.Combine(pathToBuildDirectory, "NugetPackages/Binary/package/LightInject.Annotation.nuspec");
	PatchNugetVersionInfo(pathToMetadataFile, version);
	NuGet.CreatePackage(pathToMetadataFile, outputDirectory);
}

private void CopySourceFile(string frameworkMoniker, string packageDirectoryName)
{
	string pathToMetadata = Path.Combine(pathToBuildDirectory, "../../LightInject.Annotation/NuGet");
	string pathToPackageDirectory = Path.Combine(pathToBuildDirectory, "NugetPackages/Source/package");	
	RoboCopy(pathToMetadata, pathToPackageDirectory, "LightInject.Annotation.Source.nuspec");	
	string pathToSourceFile = "../tmp/" + frameworkMoniker + "/Source/LightInject.Annotation";
	string pathToDestination = Path.Combine(pathToPackageDirectory, "content/" + packageDirectoryName + "/LightInject.Annotation");
	RoboCopy(pathToSourceFile, pathToDestination, "LightInject.Annotation.cs");
	FileUtils.Rename(Path.Combine(pathToDestination, "LightInject.Annotation.cs"), "LightInject.Annotation.cs.pp");
	ReplaceInFile(@"namespace \S*", "namespace $rootnamespace$.LightInject.Annotation", Path.Combine(pathToDestination, "LightInject.Annotation.cs.pp"));
}

private void CopyBinaryFile(string frameworkMoniker, string packageDirectoryName)
{
	string pathToMetadata = Path.Combine(pathToBuildDirectory, "../../LightInject.Annotation/NuGet");
	string pathToPackageDirectory = Path.Combine(pathToBuildDirectory, "NugetPackages/Binary/package");
	RoboCopy(pathToMetadata, pathToPackageDirectory, "LightInject.Annotation.nuspec");
	string pathToBinaryFile =  ResolvePathToBinaryFile(frameworkMoniker);
	string pathToDestination = Path.Combine(pathToPackageDirectory, "lib/" + packageDirectoryName);
	RoboCopy(pathToBinaryFile, pathToDestination, "LightInject.Annotation.*");
}

private string ResolvePathToBinaryFile(string frameworkMoniker)
{
	var pathToBinaryFile = Directory.GetFiles("../tmp/" + frameworkMoniker + "/Binary/LightInject.Annotation/bin/Release","LightInject.Annotation.dll", SearchOption.AllDirectories).First();
	return Path.GetDirectoryName(pathToBinaryFile);		
}


private void BuildAllFrameworks()
{
	Build("Net40");
	Build("Net45");
	Build("Net46");	
	Build("Pcl_111");
	BuildDnx();
}

private void Build(string frameworkMoniker)
{
	var pathToSolutionFile = Directory.GetFiles(Path.Combine(pathToBuildDirectory, frameworkMoniker + @"\Binary\"),"*.sln").First();	
	WriteLine(pathToSolutionFile);
	MsBuild.Build(pathToSolutionFile);
	pathToSolutionFile = Directory.GetFiles(Path.Combine(pathToBuildDirectory, frameworkMoniker + @"\Source\"),"*.sln").First();
	MsBuild.Build(pathToSolutionFile);
}

private void BuildDnx()
{
	string pathToProjectFile = Path.Combine(pathToBuildDirectory, @"DNXCORE50/Binary/LightInject.Annotation/project.json");
	DNU.Build(pathToProjectFile, "DNXCORE50");
	
	pathToProjectFile = Path.Combine(pathToBuildDirectory, @"DNX451/Binary/LightInject.Annotation/project.json");
	DNU.Build(pathToProjectFile, "DNX451");
}

private void RestoreNuGetPackages()
{
	RestoreNuGetPackages("net40");
	RestoreNuGetPackages("net45");
	RestoreNuGetPackages("net46");
	RestoreNuGetPackages("pcl_111");		
}

private void RestoreNuGetPackages(string frameworkMoniker)
{
	string pathToProjectDirectory = Path.Combine(pathToBuildDirectory, frameworkMoniker + @"/Binary/LightInject.Annotation");
	NuGet.Restore(pathToProjectDirectory);
	pathToProjectDirectory = Path.Combine(pathToBuildDirectory, frameworkMoniker + @"/Binary/LightInject.Annotation.Tests");
	NuGet.Restore(pathToProjectDirectory);
	pathToProjectDirectory = Path.Combine(pathToBuildDirectory, frameworkMoniker + @"/Source/LightInject.Annotation");
	NuGet.Restore(pathToProjectDirectory);
	pathToProjectDirectory = Path.Combine(pathToBuildDirectory, frameworkMoniker + @"/Source/LightInject.Annotation.Tests");
	NuGet.Restore(pathToProjectDirectory);
}


private void RunAllUnitTests()
{	
	DirectoryUtils.Delete("TestResults");
	Execute(() => RunUnitTests("Net40"), "Running unit tests for Net40");
	Execute(() => RunUnitTests("Net45"), "Running unit tests for Net45");
	Execute(() => RunUnitTests("Net46"), "Running unit tests for Net46");
	//Execute(() => AnalyzeTestCoverage("Net40"), "Analysing test coverage for Net40");			
}

private void RunUnitTests(string frameworkMoniker)
{
	string pathToTestAssembly = Path.Combine(pathToBuildDirectory, frameworkMoniker + @"/Binary/LightInject.Annotation.Tests/bin/Release/LightInject.Annotation.Tests.dll");
	string pathToTestAdapter = ResolveDirectory("../../packages/", "xunit.runner.visualstudio.testadapter.dll");
	MsTest.Run(pathToTestAssembly, pathToTestAdapter);	
}

private void AnalyzeTestCoverage()
{
	Execute(() => AnalyzeTestCoverage("NET40"), "Analyzing test coverage for NET40");
	Execute(() => AnalyzeTestCoverage("NET45"), "Analyzing test coverage for NET45");
	Execute(() => AnalyzeTestCoverage("NET46"), "Analyzing test coverage for NET46");
}

private void AnalyzeTestCoverage(string frameworkMoniker)
{	
	string pathToTestAdapter = ResolveDirectory("../../packages/", "xunit.runner.visualstudio.testadapter.dll");
	string pathToTestAssembly = Path.Combine(pathToBuildDirectory, frameworkMoniker + @"/Binary/LightInject.Annotation.Tests/bin/Release/LightInject.Annotation.Tests.dll");
	MsTest.RunWithCodeCoverage(pathToTestAssembly, pathToTestAdapter, "lightinject.annotation.dll");
}

private void InitializBuildDirectories()
{
	DirectoryUtils.Delete(pathToBuildDirectory);	
	Execute(() => InitializeNugetBuildDirectory("NET40"), "Preparing Net40");
	Execute(() => InitializeNugetBuildDirectory("NET45"), "Preparing Net45");
	Execute(() => InitializeNugetBuildDirectory("NET46"), "Preparing Net46");
	Execute(() => InitializeNugetBuildDirectory("DNXCORE50"), "Preparing DNXCORE50");
	Execute(() => InitializeNugetBuildDirectory("DNX451"), "Preparing DNX451");
	Execute(() => InitializeNugetBuildDirectory("PCL_111"), "Preparing PCL_111");					
}

private void InitializeNugetBuildDirectory(string frameworkMoniker)
{
	var pathToBinary = Path.Combine(pathToBuildDirectory, frameworkMoniker +  "/Binary");	
	CreateDirectory(pathToBinary);
	RoboCopy("../../../LightInject.Annotation", pathToBinary, "/e /XD bin obj .vs NuGet TestResults packages");	
				
	var pathToSource = Path.Combine(pathToBuildDirectory,  frameworkMoniker +  "/Source");	
	CreateDirectory(pathToSource);
	RoboCopy("../../../LightInject.Annotation", pathToSource, "/e /XD bin obj .vs NuGet TestResults packages");				  
}

private void RenameSolutionFile(string frameworkMoniker)
{
	string pathToSolutionFolder = Path.Combine(pathToBuildDirectory, frameworkMoniker +  "/Binary");
	string pathToSolutionFile = Directory.GetFiles(pathToSolutionFolder, "*.sln").First();
	string newPathToSolutionFile = Regex.Replace(pathToSolutionFile, @"(\w+)(.sln)", "$1_" + frameworkMoniker + "_Binary$2");
	File.Move(pathToSolutionFile, newPathToSolutionFile);
	WriteLine("{0} (Binary) solution file renamed to : {1}", frameworkMoniker, newPathToSolutionFile);
	
	pathToSolutionFolder = Path.Combine(pathToBuildDirectory, frameworkMoniker +  "/Source");
	pathToSolutionFile = Directory.GetFiles(pathToSolutionFolder, "*.sln").First();
	newPathToSolutionFile = Regex.Replace(pathToSolutionFile, @"(\w+)(.sln)", "$1_" + frameworkMoniker + "_Source$2");
	File.Move(pathToSolutionFile, newPathToSolutionFile);
	WriteLine("{0} (Source) solution file renamed to : {1}", frameworkMoniker, newPathToSolutionFile);
}

private void RenameSolutionFiles()
{
	RenameSolutionFile("NET40");
	RenameSolutionFile("NET45");
	RenameSolutionFile("NET46");
	RenameSolutionFile("DNXCORE50");
	RenameSolutionFile("DNX451");
	RenameSolutionFile("PCL_111");
}

private void Internalize(string frameworkMoniker)
{
	string pathToSourceFile = Path.Combine(pathToBuildDirectory, frameworkMoniker + "/Source/LightInject.Annotation/LightInject.Annotation.cs");
	Internalizer.Internalize(pathToSourceFile, frameworkMoniker);
}

private void InternalizeSourceVersions()
{
	Execute (()=> Internalize("NET40"), "Internalizing NET40");
	Execute (()=> Internalize("NET45"), "Internalizing NET45");
	Execute (()=> Internalize("NET46"), "Internalizing NET46");
	Execute (()=> Internalize("DNXCORE50"), "Internalizing DNXCORE50");
	Execute (()=> Internalize("DNX451"), "Internalizing DNX451");
}

private void PatchPackagesConfig()
{
	PatchPackagesConfig("net40");
	PatchPackagesConfig("net45");
	PatchPackagesConfig("net45");	
}


private void PatchPackagesConfig(string frameworkMoniker)
{
	string pathToPackagesConfig = Path.Combine(pathToBuildDirectory, frameworkMoniker + @"/Binary/Lightinject.Annotation/packages.config");
	ReplaceInFile(@"(targetFramework=\"").*(\"".*)", "$1"+ frameworkMoniker + "$2", pathToPackagesConfig);
	
	pathToPackagesConfig = Path.Combine(pathToBuildDirectory, frameworkMoniker + @"/Source/Lightinject.Annotation/packages.config");
	ReplaceInFile(@"(targetFramework=\"").*(\"".*)", "$1"+ frameworkMoniker + "$2", pathToPackagesConfig);
}

private void PatchAssemblyInfo()
{
	Execute(() => PatchAssemblyInfo("Net40"), "Patching AssemblyInfo (Net40)");
	Execute(() => PatchAssemblyInfo("Net45"), "Patching AssemblyInfo (Net45)");
	Execute(() => PatchAssemblyInfo("Net46"), "Patching AssemblyInfo (Net46)");	
	Execute(() => PatchAssemblyInfo("DNXCORE50"), "Patching AssemblyInfo (DNXCORE50)");
	Execute(() => PatchAssemblyInfo("DNX451"), "Patching AssemblyInfo (DNX451)");
	Execute(() => PatchAssemblyInfo("PCL_111"), "Patching AssemblyInfo (PCL_111)");
}

private void PatchAssemblyInfo(string framework)
{	
	var pathToAssemblyInfo = Path.Combine(pathToBuildDirectory, framework + @"\Binary\Lightinject.Annotation\Properties\AssemblyInfo.cs");	
	PatchAssemblyVersionInfo(version, fileVersion, framework, pathToAssemblyInfo);
	pathToAssemblyInfo = Path.Combine(pathToBuildDirectory, framework + @"\Source\Lightinject.Annotation\Properties\AssemblyInfo.cs");
	PatchAssemblyVersionInfo(version, fileVersion, framework, pathToAssemblyInfo);	
	PatchInternalsVisibleToAttribute(pathToAssemblyInfo);		
}

private void PatchInternalsVisibleToAttribute(string pathToAssemblyInfo)
{
	var assemblyInfo = ReadFile(pathToAssemblyInfo);   
	StringBuilder sb = new StringBuilder(assemblyInfo);
	sb.AppendLine(@"[assembly: InternalsVisibleTo(""LightInject.Annotation.Tests"")]");
	WriteFile(pathToAssemblyInfo, sb.ToString());
}

private void PatchProjectFiles()
{
	Execute(() => PatchProjectFile("NET40", "4.0"), "Patching project file (NET40)");
	Execute(() => PatchProjectFile("NET45", "4.5"), "Patching project file (NET45)");
	Execute(() => PatchProjectFile("NET46", "4.6"), "Patching project file (NET46)");
	Execute(() => PatchPortableProjectFile(), "Patching project file (PCL_111)");	
}

private void PatchProjectFile(string frameworkMoniker, string frameworkVersion)
{
	var pathToProjectFile = Path.Combine(pathToBuildDirectory, frameworkMoniker + @"/Binary/Lightinject.Annotation/LightInject.Annotation.csproj");
	PatchProjectFile(frameworkMoniker, frameworkVersion, pathToProjectFile);
	pathToProjectFile = Path.Combine(pathToBuildDirectory, frameworkMoniker + @"/Source/Lightinject.Annotation/LightInject.Annotation.csproj");
	PatchProjectFile(frameworkMoniker, frameworkVersion, pathToProjectFile);
	PatchTestProjectFile(frameworkMoniker);
}
 
private void PatchProjectFile(string frameworkMoniker, string frameworkVersion, string pathToProjectFile)
{
	WriteLine("Patching {0} ", pathToProjectFile);	
	SetProjectFrameworkMoniker(frameworkMoniker, pathToProjectFile);
	SetProjectFrameworkVersion(frameworkVersion, pathToProjectFile);
	
	
	SetHintPath(frameworkMoniker, pathToProjectFile);	
}

private void PatchPortableProjectFile()
{
	PatchProjectFile("PCL_111", "4.5");
	SetTargetFrameworkProfile();
}


private void SetProjectFrameworkVersion(string frameworkVersion, string pathToProjectFile)
{
	XDocument xmlDocument = XDocument.Load(pathToProjectFile);
	var frameworkVersionElement = xmlDocument.Descendants().SingleOrDefault(p => p.Name.LocalName == "TargetFrameworkVersion");
	frameworkVersionElement.Value = "v" + frameworkVersion;
	xmlDocument.Save(pathToProjectFile);
}

private void SetProjectFrameworkMoniker(string frameworkMoniker, string pathToProjectFile)
{
	XDocument xmlDocument = XDocument.Load(pathToProjectFile);
	var defineConstantsElements = xmlDocument.Descendants().Where(p => p.Name.LocalName == "DefineConstants");
	foreach (var defineConstantsElement in defineConstantsElements)
	{
		defineConstantsElement.Value = defineConstantsElement.Value.Replace("NET46", frameworkMoniker); 
	}	
	xmlDocument.Save(pathToProjectFile);
}

private void SetHintPath(string frameworkMoniker, string pathToProjectFile)
{
	if (frameworkMoniker == "PCL_111")
	{
		frameworkMoniker = "portable-net45+win81+wpa81+MonoAndroid10+MonoTouch10+Xamarin.iOS10";
	}
	ReplaceInFile(@"(.*\\packages\\LightInject.*\\lib\\).*(\\.*)","$1"+ frameworkMoniker + "$2", pathToProjectFile);	
}


private void SetTargetFrameworkProfile()
{
	
	var pathToProjectFile = Path.Combine(pathToBuildDirectory, @"PCL_111/Binary/Lightinject.Annotation/LightInject.Annotation.csproj");
	XDocument xmlDocument = XDocument.Load(pathToProjectFile);	
	var frameworkVersionElement = xmlDocument.Descendants().SingleOrDefault(p => p.Name.LocalName == "TargetFrameworkProfile");
	if (frameworkVersionElement == null)
	{
		throw new Exception(pathToProjectFile);
	}
	
	frameworkVersionElement.Value = "Profile111";
	XElement projectTypeGuidsElement = new XElement(frameworkVersionElement.Name.Namespace +  "ProjectTypeGuids");
	projectTypeGuidsElement.Value = portableClassLibraryProjectTypeGuid + ";" + csharpProjectTypeGuid;			
	frameworkVersionElement.AddAfterSelf(projectTypeGuidsElement);
	
	var importElement = xmlDocument.Descendants().SingleOrDefault(p => p.Name.LocalName == "Import");
	importElement.Attributes().First().Value = @"$(MSBuildExtensionsPath32)\Microsoft\Portable\$(TargetFrameworkVersion)\Microsoft.Portable.CSharp.targets";	
	xmlDocument.Save(pathToProjectFile);
}


private void PatchTestProjectFile(string frameworkMoniker)
{
	var pathToProjectFile = Path.Combine(pathToBuildDirectory, frameworkMoniker + @"\Binary\Lightinject.Annotation.Tests\LightInject.Annotation.Tests.csproj");
	WriteLine("Patching {0} ", pathToProjectFile);	
	SetProjectFrameworkMoniker(frameworkMoniker, pathToProjectFile);
	pathToProjectFile = Path.Combine(pathToBuildDirectory, frameworkMoniker + @"\Source\Lightinject.Annotation.Tests\LightInject.Annotation.Tests.csproj");
	WriteLine("Patching {0} ", pathToProjectFile);
	SetProjectFrameworkMoniker(frameworkMoniker, pathToProjectFile);
}
