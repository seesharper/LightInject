function PreProcess([string] $framework, [string] $nameSpace, [string]$sourceFile, [string]$destinationFile )
{
	$reader = New-Object System.IO.StreamReader ($sourceFile)
	$writer = New-Object System.IO.StreamWriter ($destinationFile)
	
	[bool]$shouldWrite = $true
	
	
	while (([string]$line = $reader.ReadLine()) -ne $null )
	{
		if($line.Contains("namespace LightInject"))
		{
			$line = $line.Replace('LightInject', $nameSpace)			
		}
		
		
						
		if($line.StartsWith("#if") -or $line.StartsWith("#endif"))
		{		
			if($line.StartsWith("#if"))	
			{
				$stringExpression = $line.Substring(4).Trim();
				$shouldWrite = EvaluateExpression $stringExpression $framework
				
				if($stringExpression -eq "TEST")
				{
					$shouldWrite = $false;
				}
			}
			else
			{
				$shouldWrite = $true
			}		
		}
	
		if($line.StartsWith("#endif"))
		{		
			$shouldWrite = $true
		}
	
		if($shouldWrite -and -not $line.StartsWith("#") -and -not $line.StartsWith("//#"))
		{
			if($line.Contains("internal class"))
			{
				$writer.WriteLine("    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]")
			}
			$writer.WriteLine($line)
		}		
	}
	
	$reader.Close();
	$reader.Dispose();
	
	$writer.Close();
	$writer.Dispose();		
}


function EvaluateExpression([string] $stringExpression, [string] $framework)
{
	[System.Linq.Expressions.Expression]$expression = CreateBinaryExpression $stringExpression $framework	
	$lambdaExpression = [System.Linq.Expressions.Expression]::Lambda($expression,$null) 
	$delegate =  $lambdaExpression.Compile() 
	$result = $delegate.Invoke()  
	return $result	
}


function CreateBooleanConstantExpression([string] $directive,[string]$framework )
{				
	if(($framework -eq $directive) -or  ($framework -eq $directive.SubString(1)))
	{
		if($directive.StartsWith("!"))
		{
			return [System.Linq.Expressions.Expression][System.Linq.Expressions.Expression]::Constant($false)
		}
		else
		{
			return [System.Linq.Expressions.Expression][System.Linq.Expressions.Expression]::Constant($true)
		}
	}
	else
	{
		if($directive.StartsWith("!"))
		{
			return [System.Linq.Expressions.Expression][System.Linq.Expressions.Expression]::Constant($true)
		}
		else
		{
			return [System.Linq.Expressions.Expression][System.Linq.Expressions.Expression]::Constant($false)
		}
	}
}

function CreateBinaryExpression([string] $stringExpression, [string]$framework)
{
	$parts = $stringExpression.Split(" ")
	
	[System.Linq.Expressions.Expression]$expression	| Out-Null	
	
	for($i=0;$i -le $parts.Length-1; $i++)
	{
		$part = $parts[$i].Trim()
		if($part -ne "&&" -and $part -ne "||")
		{			
			$expression = CreateBooleanConstantExpression $part $framework
		}
		else
		{
			$i++
			if($part -eq "&&")
			{				
				$part = $parts[$i].Trim()
				[System.Linq.Expressions.Expression]$rightExpression = CreateBooleanConstantExpression $part $framework
				$expression = [System.Linq.Expressions.Expression]::AndAlso($expression,$rightExpression)                
			}
			else
			{
				$part = $parts[$i].Trim()
				[System.Linq.Expressions.Expression]$rightExpression = CreateBooleanConstantExpression $part $framework
				$expression = [System.Linq.Expressions.Expression]::OrElse($expression,$rightExpression)
			}	
		}		
	}			
	return [System.Linq.Expressions.Expression]$expression
}

cls

$scriptpath = split-path -parent $MyInvocation.MyCommand.Path
$nugetpath = resolve-path "$scriptpath/../../nuget/nuget.exe"
$sourcepath = resolve-path "$scriptpath../../../LightInject/ServiceContainer.cs"

PreProcess "NET" "`$rootnamespace`$" $sourcepath "$scriptpath\package\content\net35\ServiceContainer.cs.pp"
PreProcess "NET" "`$rootnamespace`$" $sourcepath "$scriptpath\package\content\net40\ServiceContainer.cs.pp"
PreProcess "NET" "`$rootnamespace`$" $sourcepath "$scriptpath\package\content\net45\ServiceContainer.cs.pp"

PreProcess "SILVERLIGHT" "`$rootnamespace`$" $sourcepath "$scriptpath\package\content\sl40\ServiceContainer.cs.pp"

pushd $scriptpath

.\..\Nuget.exe pack package/LightInject.nuspec

popd
