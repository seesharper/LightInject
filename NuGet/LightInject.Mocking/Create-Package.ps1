cls

$scriptpath = split-path -parent $MyInvocation.MyCommand.Path
$nugetpath = resolve-path "$scriptpath/../../nuget/nuget.exe"
$sourcepath = resolve-path "$scriptpath../../../LightInject.Mocking/MockingExtensions.cs"

write-host $scriptpath
write-host $nugetpath
write-host $sourcepath



pushd $scriptpath

.\..\Nuget.exe pack package/LightInject.Mocking.nuspec





# $packagePath = Join-Path $scriptPath "/Package"

# $toolsPath =  Join-Path $scriptPath "/../Package/Tools"


# $nugetPath = Join-Path $scriptPath "/../NuGet.exe"

# $destinationPath = Join-Path $toolsPath "/MockingExtensions.cs"


# copy $sourcePath $destinationPath

# .\NuGet.exe pack package\LightInject.Mocking.nuspec 

popd