param(
    [switch]$InstallerOnly
)

if (Test-Path "./.publish") {
    Remove-Item -Recurse -Force "./.publish"
}

$env:SuppressNETCoreSdkPreviewMessage = "true"

$version = $env:GitVersion_SemVer
if ([string]::IsNullOrWhiteSpace($version)) {
    $version = "0.0.0-local"
}

if ($InstallerOnly) {
    Write-Host "Creating only installer for Didot CLI version $version ..."
    $projects = @(
        "src/Didot.Cli/Didot.Cli.csproj"
    )
    $frameworks = @("net10.0")
    $runtimes = @("win-x64")
}
else {
    Write-Host "Creating all packages for Didot CLI and Core version $version ..."
    $projects = @(
        "src/Didot.Core/Didot.Core.csproj",
        "src/Didot.Cli/Didot.Cli.csproj"
    )
    $frameworks = @("net8.0", "net9.0", "net10.0")
    $runtimes = @("win-x64", "linux-x64")
}

foreach ($project in $projects) {
    foreach ($framework in $frameworks) {
        if ($project -like "*Didot.Core*") {
            dotnet build $project -p:version="$version" -c Release -f $framework /p:ContinuousIntegrationBuild=true --nologo
        } else {
            foreach ($runtime in $runtimes) {
                Write-Host "Building $project for $framework on $runtime ..."
                dotnet build $project -p:version="$version" -c Release -f $framework -r $runtime /p:ContinuousIntegrationBuild=true --nologo
                Write-Host "Packaging .exe artifacts $project for $framework on $runtime..."
                dotnet publish $project -p:version="$version" -c Release -f $framework -r $runtime --no-self-contained -o ./.publish/$framework/$runtime --no-build --nologo
                if (-not $InstallerOnly) {
                    Write-Host "Creating archive .zip of $project for $framework on $runtime..."
                    7z a ./.publish/Didot-$version-$framework-$runtime.zip ./.publish/$framework/$runtime/*.*
                }
            }
        }
    }
    if (-not $InstallerOnly) {
        Write-Host "Packaging Nuget $project ..."
        if ($project -like "*Didot.Core*") {
            dotnet pack $project -p:version="$version" -c Release --include-symbols --no-build --nologo
        } else {
            dotnet pack $project -p:version="$version" -c Release -p:IncludeSymbols=false --no-build --nologo
        }
    }
}
Write-Host "Building Windows installer for Didot CLI ..."
iscc /Qp "/DAppVersion=$version" "/DTargetFramework=net10.0" "/DRuntimeIdentifier=win-x64" ".\distribution\installer\didot-cli.iss"
