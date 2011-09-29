cls



$scriptPath = (Split-Path $MyInvocation.MyCommand.Path)

pushd $scriptPath



$sourcePath = Join-Path $scriptPath "/../LightInject/ServiceContainer.cs"

$nugetPath = Join-Path $scriptPath "/NuGet.exe"

$packagePath = Join-Path $scriptPath "/Package"

$toolsPath =  Join-Path $scriptPath "/Package/Tools"


$nugetPath = Join-Path $scriptPath "/NuGet.exe"

$destinationPath = Join-Path $toolsPath "/ServiceContainer.cs_"


copy $sourcePath $destinationPath

.\NuGet.exe pack package\LightInject.nuspec 

popd