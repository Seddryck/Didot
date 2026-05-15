[CmdletBinding()]
param()

$searchRoots = @(
    ${env:ProgramFiles(x86)}
    $env:ProgramFiles
    "$env:LOCALAPPDATA\Programs"
) | Where-Object { $_ -and (Test-Path $_) }
function Find-Iscc {
    $command = Get-Command iscc.exe -ErrorAction SilentlyContinue
    if ($command) {
        return Split-Path $command.Source -Parent
    }
    foreach ($root in $searchRoots) {
        $match = Get-ChildItem `
            -Path $root `
            -Filter ISCC.exe `
            -File `
            -Recurse `
            -ErrorAction SilentlyContinue |
            Select-Object -First 1
        if ($match) {
            return $match.Directory.FullName
        }
    }
    return $null
}
$isccFolder = Find-Iscc
if (-not $isccFolder) {
    Write-Host "ISCC.exe not found. Installing Inno Setup..."
    winget install JRSoftware.InnoSetup `
        --accept-package-agreements `
        --accept-source-agreements
    $isccFolder = Find-Iscc
}

if (-not $isccFolder) {
    throw "Unable to locate ISCC.exe after installation."
}

Write-Host "Inno Setup installation path: $isccFolder"
$userPath = [Environment]::GetEnvironmentVariable("Path", "User")

if ($userPath -notlike "*$isccFolder*") {

    [Environment]::SetEnvironmentVariable(
        "Path",
        "$userPath;$isccFolder",
        "User"
    )

    $env:Path = "$env:Path;$isccFolder"

    Write-Host "Added '$isccFolder' to user PATH."
} else {
    Write-Host "ISCC.exe folder is already in user PATH."
}

if (-not (Get-Command iscc.exe -ErrorAction SilentlyContinue)) {
    throw "ISCC.exe still cannot be resolved from PATH."
}

Write-Host "ISCC.exe is available."
