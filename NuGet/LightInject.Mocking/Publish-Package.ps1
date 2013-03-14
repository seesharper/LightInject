param([string] $package)

$scriptPath = (Split-Path $MyInvocation.MyCommand.Path)
$nugetpath = resolve-path "$scriptpath/.."



pushd $nugetpath

$keyfile = "$env:USERPROFILE\DropBox\NuGet Access Key.txt"


$apikey = get-content $keyfile
$appid = "LightInject.Mocking"

.\NuGet.exe SetApiKey $apikey
.\NuGet.exe push $package 

popd