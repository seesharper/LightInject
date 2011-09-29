param([string] $package)

$scriptPath = (Split-Path $MyInvocation.MyCommand.Path)

pushd $scriptPath

$keyfile = "$env:USERPROFILE\DropBox\NuGet Access Key.txt"


$apikey = get-content $keyfile
$appid = "ExpressionTools"

.\NuGet.exe push $apiid $package $apikey -source http://packages.nuget.org/v1

popd