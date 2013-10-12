$scriptpath = split-path -parent $MyInvocation.MyCommand.Path
$assemblyPath = resolve-path "$scriptpath/../LightInject.PreProcessor.dll"
$sourcepath = resolve-path "$scriptpath/Source"
write "Loading assembly"
[Reflection.Assembly]::LoadFile($assemblyPath)
write "Processing $sourcepath"

$targetPath = $scriptpath + "\package\content\net40\"
new-item  $targetPath -itemtype directory -ErrorAction SilentlyContinue
[LightInject.PreProcessor.Publicizer]::Write($sourcepath,"$targetPath\ServiceContainer.cs.pp")



pushd $scriptpath

'.\..\Nuget.exe pack package/LightInject.nuspec -OutputDirectory "..\"'

popd

