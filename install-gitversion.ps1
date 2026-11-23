$ErrorActionPreference = "Stop"

$requiredVersion = [Version]"6.8.2"
$dotnetToolsPath = Join-Path $env:USERPROFILE ".dotnet\tools"
$gitVersionExe = Join-Path $dotnetToolsPath "dotnet-gitversion.exe"

function Get-GitVersionVersion {
    param(
        [Parameter(Mandatory)]
        [string] $Command
    )

    try {
        $versionOutput = (& $Command /version 2>&1 | Out-String).Trim()

        if ($LASTEXITCODE -ne 0) {
            return $null
        }

        $versionMatch = [regex]::Match($versionOutput, "^(\d+\.\d+\.\d+)(?:\+\S+)?$")

        if (-not $versionMatch.Success) {
            return $null
        }

        return [Version]$versionMatch.Groups[1].Value
    }
    catch {
        return $null
    }
}

$gitVersion = Get-Command $gitVersionExe -ErrorAction SilentlyContinue
$installedVersion = if ($gitVersion) {
    Get-GitVersionVersion -Command $gitVersion.Source
}

if ($installedVersion -eq $requiredVersion) {
    Write-Host "GitVersion $installedVersion is already installed."
    Write-Host "Path: $($gitVersion.Source)"
    exit 0
}

if ($installedVersion) {
    Write-Host "GitVersion $installedVersion differs from the required version $requiredVersion. Reinstalling..."
}
elseif ($gitVersion) {
    Write-Host "The installed GitVersion version could not be determined. Reinstalling..."
}
else {
    Write-Host "GitVersion is not installed. Installing..."
}

$dotnet = Get-Command dotnet -ErrorAction SilentlyContinue

if (-not $dotnet) {
    throw ".NET SDK is not installed or not available in PATH. Install the .NET SDK first."
}

$globalTools = dotnet tool list --global

if ($LASTEXITCODE -ne 0) {
    throw "Failed to list globally installed .NET tools."
}

if ($globalTools | Select-String -Pattern "^gitversion\.tool\s" -Quiet) {
    dotnet tool update --global GitVersion.Tool --version $requiredVersion --allow-downgrade
}
else {
    dotnet tool install --global GitVersion.Tool --version $requiredVersion
}

if ($LASTEXITCODE -ne 0) {
    throw "Failed to install GitVersion.Tool."
}

if (-not (Test-Path $gitVersionExe)) {
    throw "dotnet-gitversion.exe was not found after installing GitVersion.Tool."
}

$installedVersion = Get-GitVersionVersion -Command $gitVersionExe

if ($installedVersion -ne $requiredVersion) {
    throw "GitVersion.Tool $installedVersion was installed, but version $requiredVersion is required."
}

Write-Host "GitVersion $installedVersion installed successfully."
Write-Host "Path: $gitVersionExe"
