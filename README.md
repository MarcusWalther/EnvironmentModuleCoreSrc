<p align="center">
  <img src="https://github.com/MarcusWalther/EnvironmentModuleCoreSrc/blob/master/Icon.png?raw=true" height="64">
  <h3 align="center">EnvironmentModuleCoreSrc</h3>
  <p align="left">This project contains the C# source code for the EnvironmentModuleCore Library (dll) that is used by the Powershell module "EnvironmentModuleCore". It contains all data and logic classes that cannot be described in the Powershell in an handable fashion.<p>
  <p align="center"><a href="https://www.nuget.org/packages/EnvironmentModuleCore/"><img src="https://img.shields.io/nuget/v/EnvironmentModuleCore.svg" alt="Nuget Package"></a> <a href="https://github.com/MarcusWalther/EnvironmentModuleCoreSrc/blob/master/LICENSE.md"><img src="https://img.shields.io/badge/License-MIT-yellow.svg" alt="MIT License"></a></p>
</p>

# Build
The project is written in C#, based on .Net Core 2.1. It was developed using Visual Studio 2017. In order to build it, load the solution file into Visual Studio and compile it. The file *EnvironmentModuleCore.nuspec* can be used to create a nuget package.

# References
* Library -- Scriban (see https://github.com/lunet-io/scriban) - BSD 2-Clause "Simplified" License.
* Idea -- Environment Modules on Linux Systems (see http://modules.sourceforge.net)
* Icon -- Adaption of Powershell Icon (see https://de.wikipedia.org/wiki/Datei:PowerShell_5.0_icon.png)
