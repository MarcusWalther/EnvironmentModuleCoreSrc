param(
    $Configuration = 'Release',
    $Suffix = $null,
    $NugetSource = $null,
    $NugetApiKey = $null
)

task Prepare {
    dotnet restore --interactive
}

task Build Prepare, {
    dotnet build (Join-Path "$PSScriptRoot" "EnvironmentModuleCore.sln") -c "$Configuration" --version-suffix "$Suffix"
}

task Pack {
    if(Test-Path "Package") {
        Remove-Item "Package" -Recurse
    }

    $projectFile = $(Join-Path "$PSScriptRoot" (Join-Path "EnvironmentModuleCore" "EnvironmentModuleCore.csproj"))

    if([string]::IsNullOrEmpty($Suffix)) {
        dotnet pack $projectFile --no-build -c "$Configuration" -o "Package"
    }
    else {
        dotnet pack $projectFile --no-build -c "$Configuration" -o "Package" --version-suffix "$Suffix"
    }
}

task Deploy {
    dotnet nuget push --source "$NugetSource" --api-key "$NugetApiKey" "Package\*.nupkg"
}