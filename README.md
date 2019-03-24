[![nuget](https://img.shields.io/nuget/v/EnvironmentModuleCore.svg)](https://www.nuget.org/packages/EnvironmentModuleCore/)

# Overview
<table border="0">
 <tr border="0">
    <td width="100" border="0"><img src="https://github.com/MarcusWalther/EnvironmentModuleCoreSrc/blob/master/Icon.png?raw=true" width="100"></td>
    <td border="0">This project contains the C# source code for the EnvironmentModuleCore Library (dll) that is used by the Powershell module "EnvironmentModuleCore". It contains all data and logic classes that cannot be described in the Powershell in an handable fashion.</td>
 </tr>
</table>

# Build
The project is written in C#, based on .Net Core 2.1. It was developed using Visual Studio 2017. In order to build it, load the solution file into Visual Studio and compile it. The file *EnvironmentModuleCore.nuspec* can be used to create a nuget package.

# Referenced Libraries
Scriban (see https://github.com/lunet-io/scriban) - BSD 2-Clause "Simplified" License.
