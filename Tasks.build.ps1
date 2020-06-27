param(
    $Configuration = 'Release',
    $Suffix = "local",
    $NugetSource = $null,
    $NugetApiKey = $null
)

task Prepare {
    dotnet restore --interactive
}

task Build Prepare, {
    $cmdArguments = "build", "`"$(Join-Path `"$PSScriptRoot`" EnvironmentModuleCore.sln)`"", "-c", "$Configuration"
    if(-not [string]::IsNullOrEmpty($Suffix)) {
        $cmdArguments += "--version-suffix", "$Suffix"
    }

    dotnet $cmdArguments
}

task Pack {
    if(Test-Path "Package") {
        Remove-Item "Package" -Recurse
    }

    $projectFile = $(Join-Path "$PSScriptRoot" (Join-Path "EnvironmentModuleCore" "EnvironmentModuleCore.csproj"))
    $cmdArguments = "pack", "$projectFile", "--no-build", "-c", "$Configuration", "-o", "Package"
    if(-not [string]::IsNullOrEmpty($Suffix)) {
        $cmdArguments += "--version-suffix", "$Suffix"
    }

    dotnet $cmdArguments
}

task Deploy {
    Get-ChildItem "Package/*.nupkg" | ForEach-Object {dotnet nuget push --source "$NugetSource" --api-key "$NugetApiKey" "$($_.FullName)"}
}