param($rootPath, $toolsPath, $package, $project)

$projectPath = Split-Path $project.FullName

$filePath = Join-Path $projectPath "ExpressionExtensions.cs"

if (Test-Path $filePath)
{
	$project.ProjectItems.Item("ExpressionExtensions.cs").Delete()
}

