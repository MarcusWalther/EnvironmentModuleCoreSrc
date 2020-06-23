param(
    $Configuration = 'Release',
    $Suffix = ''
)

task Build {
    dotnet build (Join-Path "$PSScriptRoot" "EnvironmentModuleCore.sln") -c "$Configuration" --version-suffix "$Suffix"
}

task Pack {
    $projectFile = $(Join-Path "$PSScriptRoot" (Join-Path "EnvironmentModuleCore" "EnvironmentModuleCore.csproj"))

    if([string]::IsNullOrEmpty($Suffix)) {
        dotnet pack $projectFile --no-build -c "$Configuration" -o "Package"
    }
    else {
        dotnet pack $projectFile --no-build -c "$Configuration" -o "Package" --version-suffix "$Suffix"
    }
}