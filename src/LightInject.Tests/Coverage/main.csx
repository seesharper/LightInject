#load "nuget:Dotnet.Build, 0.3.9"

using System.Xml.Linq;

var rootPath = Path.Combine(FileUtils.GetScriptFolder(),"..");
var pathToCoverageResultFolder = Path.Combine(rootPath, "CoverageResult");

Command.Execute("dotnet", $"test -c release -f netcoreapp2.0 /property:CollectCoverage=true /property:CoverletOutputFormat=\\\"opencover,lcov\\\" /property:CoverletOutput={pathToCoverageResultFolder}/",rootPath);

var pathToOpenCoverResult = Path.Combine(pathToCoverageResultFolder, "coverage.opencover.xml");

Command.Execute("dotnet",$"reportgenerator \"-reports:{pathToOpenCoverResult}\"  \"-targetdir:{pathToCoverageResultFolder}\" \"-reportTypes:Html;XmlSummary\"", rootPath);

var coverageSummary = XDocument.Load(Path.Combine(pathToCoverageResultFolder, "summary.xml"));

var classesWithoutFullCoverage = coverageSummary.Descendants("Class").Where(e => e.Attribute("coverage").Value != "100");
foreach (var classWithoutFullCoverage in classesWithoutFullCoverage)
{
    WriteLine($"Class {classWithoutFullCoverage.Attribute("name")} has only {classWithoutFullCoverage.Attribute("coverage")}% coverage.");
}

WriteLine($"Open \"{Path.Combine(pathToCoverageResultFolder,"index.htm")}\" for a full coverage report");

