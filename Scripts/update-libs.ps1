[CmdletBinding()] 
Param(
	$Source = "..\..\..\App\Libs\Shared",
    $Destination = "..\Libs\Shared"
);

$dependencies = @{}
$dependencies.Add(1, "Contract")

$dependencies.GETENUMERATOR() |  Sort-Object KEY  | % { 
        "Updating Shared Libs with " + $_.VALUE
        $SourcePath = [IO.Path]::GetFullPath((Join-Path $PWD (Join-Path $Source $_.VALUE))) 
        Write-Host "Source Path: " + $SourcePath
        Get-ChildItem -Path $SourcePath | Copy-Item -Destination $Destination -Recurse  
    }