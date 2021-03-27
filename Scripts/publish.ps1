[CmdletBinding()] 
Param(
    $PluginName = "Zip",
    $Clean = "true",
    $Restore = "true",
    $Build = "true",
    $Publish = "true",
    $Pack = "true",
	$DeploymentPath = "..\..\..\Deployment\Plugins",
    $PluginsBuilderPath = "..\..\..\Deployment\PluginsBuilder.CLI\Routindo.PluginsBuilder.CLI.exe"
);

$clean_publish = 1;

$separator = "\";
$SourceFolder = ".." + $separator + "Source";
$configuration = "Release";
$runtime = "win-x64";


clear
$projects = @{}
$projects.Add(1,'Routindo.Plugins.' +  $PluginName)
$projects.Add(2,'Routindo.Plugins.' +  $PluginName + '.UI')
$projects.Add(3,'Routindo.Plugins.' +  $PluginName + '.Components')

$output = $SourceFolder + $separator + $projects[1] + $separator + "bin" + $separator + $configuration + $separator + "Publish" + $separator + $runtime;

if($Clean -eq "true") {
    # Restore dependencies 
    $projects.GETENUMERATOR() |  Sort-Object KEY  | % { 
        "Cleaning " + $_.KEY.ToString() + " - " + $_.VALUE
        $clean_command = "dotnet clean" + " " + $SourceFolder + $separator + $_.VALUE + $separator + $_.VALUE + ".csproj"
        iex $clean_command
    }
}

if($Restore -eq "true") {
    # Restore dependencies 
    $projects.GETENUMERATOR() |  Sort-Object KEY  | % { 
        "Restoring " + $_.KEY.ToString() + " - " + $_.VALUE
        $restore_command = "dotnet restore" + " " + $SourceFolder + $separator + $_.VALUE + $separator + $_.VALUE + ".csproj"
        iex $restore_command
    }
}

if($Build -eq "true") {
    # Build projects 
    $projects.GETENUMERATOR() |  Sort-Object KEY  | % { 
        "Building " + $_.KEY.ToString() + " - " + $_.VALUE
        $build_command = "dotnet build" + " " + $SourceFolder + $separator + $_.VALUE + $separator + $_.VALUE + ".csproj"
        iex $build_command
    }
}

if($Publish -eq "true") { 
    if($clean_publish -eq 1) {
        Get-ChildItem $output -Recurse | Remove-Item -Recurse
    }
    # Publish projects
    $projects.GETENUMERATOR() |  Sort-Object KEY  | % { 
        "Publishing " + $_.KEY.ToString() + " - " + $_.VALUE
        $publish_command = "dotnet publish" + " " + $SourceFolder + $separator + $_.VALUE + $separator + $_.VALUE + ".csproj" + " " + "--configuration " + $configuration + " --runtime " + $runtime + " /p:DebugType=None /p:DebugSymbols=false /p:CopyLocalLockFileAssemblies=true" + " --output " + $output 
        iex $publish_command
    }
}

if($Pack -eq "true") {
    Write-Host "Packing a new version from " $PluginName
    $pluginLibrary = Resolve-Path $([IO.Path]::Combine($output, $projects[1] + ".dll"))
    $pluginVersion = [System.Diagnostics.FileVersionInfo]::GetVersionInfo($pluginLibrary).ProductVersion
    Write-Host "Target version:" + $pluginVersion
    $PluginBuilderAbsolutePath = Resolve-Path $PluginsBuilderPath  | select -ExpandProperty Path 
    Write-Host $PluginBuilderAbsolutePath
    Write-Host $pluginLibrary
    $command = $PluginBuilderAbsolutePath + " --path=" + $pluginLibrary + " --output=" + $DeploymentPath + " --manifest=true --pack=true"
    Write-Host $command
    iex $command;
}