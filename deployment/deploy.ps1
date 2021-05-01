[CmdletBinding()] 
Param(
	$Clean = "false",
    $Restore = "false",
    $UpdateLibs = "true",
    $Build = "false",
    $Publish = "true",
    $PublishFolder = "",
    $SharedLibsFolder = "",
    $PackFolder = "",
    $Configuration = "Release",
    $Runtime = "win-x64",
    $PreRelease = ""
);

$DeploymentPath = Split-Path $MyInvocation.MyCommand.Path -Parent
$ConfigPath = Join-Path $DeploymentPath "config.xml"
Write-Host "Reading configuration file: " $ConfigPath -ForegroundColor Green
[xml]$xmlConfig = Get-Content -Path $ConfigPath

function Display-Header ($Header) {
    
    $charput = ""
	For ($i=0; $i -lt $Header.length +10; $i++) {
        $charput = $charput + '#'
    }

    $charputEmpty = "#"
	For ($i=0; $i -lt ($Header.length +10)-2; $i++) {
        $charputEmpty = $charputEmpty + ' '
    }
    $charputEmpty = $charputEmpty + "#"

    Write-Host $charput -ForegroundColor Yellow
    Write-Host $charput -ForegroundColor Yellow
    Write-Host $charputEmpty -ForegroundColor Yellow
    $HeaderLine = '# '+ $Header

    For ($i=0; $i -lt $charput.Length - $Header.Length - 3; $i++) {
        $HeaderLine = $HeaderLine + ' '
    }
    $HeaderLine = $HeaderLine + '#'

    Write-Host $HeaderLine -ForegroundColor Yellow
    Write-Host $charputEmpty -ForegroundColor Yellow
    Write-Host $charput -ForegroundColor Yellow
    Write-Host $charput -ForegroundColor Yellow
}

function Force-Resolve-Path {
    <#
    .SYNOPSIS
        Calls Resolve-Path but works for files that don't exist.
    #>
    param (
        [string] $FileName
    )

    $FileName = Resolve-Path $FileName -ErrorAction SilentlyContinue `
                                       -ErrorVariable _frperror
    if (-not($FileName)) {
        $FileName = $_frperror[0].TargetObject
    }

    return $FileName
}

function Get-Setting($SettingKey) {
    $SettingValue = $xmlConfig.Deployment.Settings.Setting | Where-Object Key -EQ $SettingKey | Select -Unique | % { $_.Value }
    return $SettingValue
}

function Get-Publish-Folder($Directory) {
    # The priority for output path is from arguments
    $Output = ""
    $Output = $PublishFolder
    if($Output -eq "") {
        # If publish folder from arguments is not set, try get the publish folder from the config file
        $Output = Force-Resolve-Path ( Join-Path (Join-Path $DeploymentPath (Get-Setting -SettingKey 'PublishFolder')) $Directory)
    }
    else {
        # Combine publish folder with the Library directory
        $Output = Force-Resolve-Path ( Join-Path $Output $Directory)
    }

    # Create the output folder if doesn't exist
    New-Item -ItemType Directory -Force -Path $Output | Out-Null
    return $Output
}

function Get-SharedLibs-Folder($Directory) {
    # The priority for output path is from arguments
    $Output = $SharedLibsFolder
    if($Output -eq "") {
        # If publish folder from arguments is not set, try get the publish folder from the config file
        $Output = Force-Resolve-Path ( Join-Path (Join-Path $DeploymentPath (Get-Setting -SettingKey 'SharedLibsFolder')) $Directory)
    }
    else {
        # Combine publish folder with the Library directory
        $Output = Force-Resolve-Path ( Join-Path $Output $Directory)
    }

    # Create the output folder if doesn't exist
    New-Item -ItemType Directory -Force -Path $Output | Out-Null
    return $Output
}

function Get-Pack-Folder {
    # The priority for output path is from arguments
    $Output = $PackFolder
    if($Output -eq "") {
        $Output = Force-Resolve-Path (Join-Path $DeploymentPath (Get-Setting -SettingKey 'PackFolder'))
    }

    # Create the output folder if doesn't exist
    New-Item -ItemType Directory -Force -Path $Output | Out-Null
    return $Output
}

function Clean-Project-Output($ProjectPath) {
    dotnet clean $ProjectPath
}

function Restore-Project-Dependencies($ProjectPath) {
    dotnet clean $ProjectPath
}

function Update-SharedLibs() {
    $SharedLibsSourceFolder = Force-Resolve-Path (Join-Path $DeploymentPath (Get-Setting -SettingKey 'SharedLibsFolder'))
    $DependenciesDestinationFolder = Force-Resolve-Path (Join-Path $DeploymentPath (Get-Setting -SettingKey 'DependenciesPath'))
	Write-Host "DependenciesDestinationFolder" $DependenciesDestinationFolder
	Write-Host "SharedLibsSourceFolder" $SharedLibsSourceFolder
    $xmlConfig.Deployment.Dependencies.Dependency | Sort-Object Order | ForEach-Object { 
        $DependencyDirectory = Force-Resolve-Path (Join-Path $SharedLibsSourceFolder $_.Directory)

        if(Test-Path $DependencyDirectory -PathType Container) {
            Get-ChildItem -Path $DependencyDirectory | Copy-Item -Destination $DependenciesDestinationFolder -Recurse
        }
        else {
            Write-Host "Source Folder not found for shared libs:" $DependencyDirectory -ForegroundColor Red
        }
    }
      
}

function Build-Project($ProjectPath) {
    dotnet build $ProjectPath --configuration $Configuration --runtime $Runtime
}

function Get-Version-Suffix($ProjectPath) {
    $commitHash = "c$((git -C (Split-Path $ProjectPath -Parent) rev-parse HEAD).Substring(0,8))";
    if([string]::IsNullOrEmpty($PreRelease)) {
        return $commitHash
    } 
    else {
        return "$($commitHash)+$($PreRelease)"
    }
}

function Execute-Publish-Command ($ProjectPath, $PublishOutput, $SelfContained) {
	$VersionSuffix = Get-Version-Suffix -ProjectPath $ProjectPath
    dotnet publish $ProjectPath --configuration $Configuration --runtime $Runtime /p:DebugType=None /p:DebugSymbols=false /p:CopyLocalLockFileAssemblies=true --version-suffix $VersionSuffix --self-contained $SelfContained --output $PublishOutput | Out-Default
}

function Publish-Project($ProjectPath, $Directory, $SelfContained, $Clean) {
    $PublishOutput = ""
    $PublishOutput = Get-Publish-Folder -Directory $Directory
    if($Clean -eq "true") {
        Get-ChildItem -Path $PublishOutput -Include *.* -Recurse | foreach { $_.Delete()} | Out-Null
    }
    Execute-Publish-Command -PublishOutput $PublishOutput -ProjectPath $ProjectPath -SelfContained $SelfContained | Out-Null
    return $PublishOutput
}

function Share-Library($PublishOutput, $Directory) {
    $SharedLibsDestinationFolder = Get-SharedLibs-Folder -Directory $Directory
    Get-ChildItem -Path $SharedLibsDestinationFolder -Include *.* -Recurse | foreach { $_.Delete() }
    Get-ChildItem -Path $PublishOutput  | Copy-Item -Destination $SharedLibsDestinationFolder -Recurse -Force
}

function Pack($PublishOutput, $Name) {
    $PackFolder = Get-Pack-Folder
    $PluginBuilder = Resolve-Path (Join-Path $DeploymentPath (Get-Setting -SettingKey 'PluginsBuilder')) | select -ExpandProperty Path 
    $PackCommand = $PluginBuilder + " --path=" + $PublishOutput + " --output=" + $PackFolder + " --manifest=true --pack=true"
    Write-Host $PackCommand
    iex $PackCommand;
}

$SourceAbsolutPath = Resolve-Path (Join-Path $DeploymentPath (Get-Setting -SettingKey 'SourcePath'))

Write-Host "Source Absolut Path:" $SourceAbsolutPath


if($UpdateLibs -eq 'true') {
        Write-Host "Updating Shared Libs:" $_.Name
        Update-SharedLibs
}

Write-Host "Deploying libraries"

$xmlConfig.Deployment.Projects.Project | Sort-Object Order | ForEach-Object {

    $ProjectAbsolutPath = ""
    $ProjectAbsolutPath = Join-Path (Join-Path $SourceAbsolutPath $_.Directory) $_.File
    if($Clean -eq 'true') {
        Display-Header ( "Cleaning Project Output:" + $_.Name)
        Write-Host "Cleaning Project Output:" $_.Name
        Clean-Project-Output -ProjectPath $ProjectAbsolutPath
    }

    if($Restore -eq 'true') {
    Display-Header ( "Restoring Project Output:" + $_.Name)
        Write-Host "Restoring Project Dependencies:" $_.Name
        Restore-Project-Dependencies -ProjectPath $ProjectAbsolutPath
    }

    if($Build -eq 'true') {
    Display-Header ( "Building Project Output:" + $_.Name)
        Write-Host "Building Project:" $_.Name
        Build-Project -ProjectPath $ProjectAbsolutPath
    }

    if($Publish -eq 'true') {
    Display-Header ( "Publishing Project Output:" + $_.Name)
        Write-Host "Publishing Project:" $_.Name

        $DeploymentDir = $_.DeploymentDir
        if([string]::IsNullOrEmpty($DeploymentDir)) {
            $DeploymentDir = $_.Directory
        }

        $PublishOutputFolder = Publish-Project -ProjectPath $ProjectAbsolutPath -Directory $DeploymentDir -SelfContained $_.SelfContained -Clean $_.CleanDeploymentDir

        if($_.Share -eq 'true') {
        Display-Header ( "Sharing Project Output:" + $_.Name)
            Write-Host "Sharing Library:" $_.Name
            Share-Library -PublishOutput $PublishOutputFolder -Directory $DeploymentDir
        }

        if($_.Pack -eq 'true') {
        Display-Header ( "Packing Project Output:" + $_.Name)
            Write-Host "Packing Library:" $_.Name
            #Pack -PublishOutput (Resolve-Path (Join-Path $PublishOutputFolder $DeploymentDir)) -Name $_.Name
            Pack -PublishOutput $PublishOutputFolder -Name $_.DeploymentDir
        }
    }
}


