cls



$scriptPath = (Split-Path $MyInvocation.MyCommand.Path)

pushd $scriptPath



$sourcePath = Join-Path $scriptPath "/../ExpressionTools/ExpressionExtensions.cs"

$nugetPath = Join-Path $scriptPath "/NuGet.exe"

$packagePath = Join-Path $scriptPath "/Package"

$toolsPath =  Join-Path $scriptPath "/Package/Tools"


$nugetPath = Join-Path $scriptPath "/NuGet.exe"

$destinationPath = Join-Path $toolsPath "/ExpressionExtensions.cs_"


copy $sourcePath $destinationPath

.\NuGet.exe pack package\ExpressionTools.nuspec 

popd