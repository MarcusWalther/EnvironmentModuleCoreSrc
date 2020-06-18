param(
    $Configuration = 'Release'
)

task Build {
    dotnet build (Join-Path "$PSScriptRoot" "EnvironmentModuleCore.sln") -c "$Configuration"
}