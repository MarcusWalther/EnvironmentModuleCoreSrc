<p align="center">
  <img src="https://github.com/MarcusWalther/EnvironmentModuleCoreSrc/blob/master/Icon.png?raw=true" height="64">
  <h3 align="center">EnvironmentModuleCoreSrc</h3>
  <p align="left">This project contains the C# source code for the EnvironmentModuleCore Library (dll) that is used by the Powershell module "EnvironmentModuleCore". It contains all data and logic classes that cannot be described in the Powershell in an handable fashion.<p>
  <p align="center">
    <a href="https://www.nuget.org/packages/EnvironmentModuleCore/">
      <img src="https://img.shields.io/nuget/v/EnvironmentModuleCore.svg" alt="Nuget Package">
    </a>
    <a href="">
      <img src="https://dev.azure.com/MarcusWalther/EnvironmentModuleCoreSrc/_apis/build/status/Master.EnvironmentModuleCoreSrc?branchName=master" alt="Azure Pipeline">
    </a>
    <a href="https://github.com/MarcusWalther/EnvironmentModuleCoreSrc/blob/master/LICENSE.md">
      <img src="https://img.shields.io/badge/License-MIT-yellow.svg" alt="MIT License">
    </a>
  </p>
</p>

# Build
The project is written in C#, based on .Net Standard 2.0. It was developed using Visual Studio 2022. In order to build it, load the solution file into Visual Studio and compile it. Alternatively it is possible to build and pack the solution on the command line.

```powershell
# Requires the module InvokeBuild (https://github.com/nightroman/Invoke-Build)
Invoke-Build Build
Invoke-Build Pack
```

# References
* Library -- Scriban (see https://github.com/lunet-io/scriban) - BSD 2-Clause "Simplified" License.
* Powershell Module -- InvokeBuild (see https://github.com/nightroman/Invoke-Build) - Apache License, Version 2.0
* Idea -- Environment Modules on Linux Systems (see http://modules.sourceforge.net)
* Icon -- Adaption of Powershell Icon (see https://de.wikipedia.org/wiki/Datei:PowerShell_5.0_icon.png)
