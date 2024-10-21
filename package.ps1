if (Test-Path "./.publish") {
    Remove-Item -Recurse -Force "./.publish"
}

$projects = @(
  "src/Didot.Core/Didot.Core.csproj",
  "src/Didot.Cli/Didot.Cli.csproj"
)
$frameworks = @("net7.0", "net8.0")
$runtimes = ("win-x64", "linux-x64")

foreach ($project in $projects) {
    foreach ($framework in $frameworks) {
        foreach ($runtime in $runtimes) {
            Write-Host "Building $project for $framework on $runtime ..."
            dotnet build $project -p:version="$env:GitVersion_SemVer" -c Release -f $framework -r $runtime /p:ContinuousIntegrationBuild=true --nologo
            if ($project -like "*Didot.Cli*") {
                Write-Host "Packaging .exe artifacts $project for $framework on $runtime..."
                dotnet publish $project -c Release -f $framework -r $runtime --no-self-contained -o ./.publish/$framework/$runtime --no-build --nologo
                7z a ./.publish/Didot-$env:GitVersion_SemVer-$framework-$runtime.zip ./.publish/$framework/$runtime/*.*
            }
        }
    }
    Write-Host "Packaging Nuget $project ..."
    dotnet pack $project -p:version="$env:GitVersion_SemVer" -c Release --include-symbols --no-build --nologo
}    