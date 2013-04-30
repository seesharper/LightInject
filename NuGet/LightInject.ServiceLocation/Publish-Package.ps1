param([string] $package)
$scriptpath = split-path -parent $MyInvocation.MyCommand.Path
$nugetpath = resolve-path "$scriptpath/.."



pushd $nugetpath

$keyfile = "$env:USERPROFILE\DropBox\NuGet Access Key.txt"


$apikey = get-content $keyfile
$appid = "LightInject.ServiceLocation"

.\NuGet.exe SetApiKey $apikey
.\NuGet.exe push $package 

popd