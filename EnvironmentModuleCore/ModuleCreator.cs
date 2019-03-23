// <copyright file="ModuleCreator.cs">
//     Copyright 2019 Marcus Walther
// </copyright>
// <author>Marcus Walther</author>

namespace EnvironmentModuleCore
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    /// <summary>
    /// This helper class can be used to create new environment modules.
    /// </summary>
    // ReSharper disable once UnusedMember.Global
    public class ModuleCreator
    {
        /// <summary>
        /// Create a new meta environment module with the given parameters.
        /// </summary>
        /// <param name="name">The name of the meta module to create.</param>
        /// <param name="storageDirectory">The directory to store the created module files.</param>
        /// <param name="workingDirectory">The working directory containing the template folder and template files.</param>
        /// <param name="directUnload">True if the module should be marked with the flag "direct unload".</param>
        /// <param name="additionalDescription">An additional description for the module.</param>
        /// <param name="additionalEnvironmentModules">Additional environment modules that must be loaded as dependencies.</param>
        // ReSharper disable once UnusedMember.Global
        public static void CreateMetaEnvironmentModule(
            string name, 
            string storageDirectory,
            string workingDirectory, 
            bool directUnload, 
            string additionalDescription,
            string[] additionalEnvironmentModules)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ModuleException("The name cannot be empty");
            }

            if (workingDirectory == null)
            {
                workingDirectory = Directory.GetCurrentDirectory();
            }

            if (!new DirectoryInfo(workingDirectory).Exists)
            {
                throw new ModuleException($"The given working directory '{workingDirectory}' does not exist");
            }

            if (additionalEnvironmentModules == null)
            {
                additionalEnvironmentModules = new string[] { };
            }

            var modelDefinition = new
            {
                Author = "EnvironmentModule",
                CompanyName = string.Empty,
                Name = name,
                DateTime.Now.Year,
                Date            = DateTime.Now.ToString("dd/MM/yyyy"),
                Guid            = Guid.NewGuid(),
                ModuleRoot      = "\".\\\"",
                RequiredModules = "\"EnvironmentModuleCore\"",
                Dependencies = additionalEnvironmentModules.Length > 0 ? additionalEnvironmentModules.Select(x => "\"x\"").Aggregate((a, b) => a + "," + b) : string.Empty,
                CustomCode      = string.Empty,
                AdditionalDescription = additionalDescription,
                DirectUnload    = $"${directUnload}",
                ModuleType      = EnvironmentModuleType.Meta.ToString(),
                Category        = string.Empty,
                Parameters      = new Dictionary<string, string>()
            };

            FileInfo templatePsd = new FileInfo(Path.Combine(workingDirectory, "Templates\\EnvironmentModule.psd1.template"));
            FileInfo templatePsm = new FileInfo(Path.Combine(workingDirectory, "Templates\\EnvironmentModule.psm1.template"));
            FileInfo templatePse = new FileInfo(Path.Combine(workingDirectory, "Templates\\EnvironmentModule.pse1.template"));

            CreateModuleFromTemplates(modelDefinition, templatePsd, templatePsm, templatePse, storageDirectory, name, null, null);
        }

        /// <summary>
        /// Create a new environment module with the given parameters.
        /// </summary>
        /// <param name="name">The name of the environment module to create.</param>
        /// <param name="storageDirectory">The directory to store the created module files.</param>
        /// <param name="description">The description of the module.</param>
        /// <param name="workingDirectory">The working directory containing the template folder and template files.</param>
        /// <param name="author">The author of the module</param>
        /// <param name="version">The version of the module.</param>
        /// <param name="architecture">The architecture of the module.</param>
        /// <param name="executable">The path to the executable of the module.</param>
        /// <param name="dependencies">The dependencies of the environment module.</param>
        /// <param name="category">The category of the environment module.</param>
        /// <param name="parameters">The parameters as key value pairs.</param>
        // ReSharper disable once UnusedMember.Global
        public static void CreateEnvironmentModule(
            string name,
            string storageDirectory,
            string description,
            string workingDirectory = null,
            string author = null,
            string version = null,
            string architecture = null,
            string executable = null,
            DependencyInfo[] dependencies = null,
            string category = null,
            Dictionary<string, string> parameters = null)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ModuleException("The name cannot be empty");
            }

            if (workingDirectory == null)
            {
                workingDirectory = Directory.GetCurrentDirectory();
            }

            if (!new DirectoryInfo(workingDirectory).Exists)
            {
                throw new ModuleException($"The given working directory '{workingDirectory}' does not exist");
            }

            if (string.IsNullOrEmpty(author))
            {
                author = string.Empty;
            }

            if (string.IsNullOrEmpty(description))
            {
                description = string.Empty;
            }

            if (dependencies == null)
            {
                dependencies = new DependencyInfo[] { };
            }

            if (string.IsNullOrEmpty(category))
            {
                category = string.Empty;
            }

            if (parameters == null)
            {
                parameters = new Dictionary<string, string>();
            }

            FileInfo executableFile;
            if (!string.IsNullOrEmpty(executable))
            {
                executableFile = new FileInfo(executable);
                if (!executableFile.Exists)
                {
                    throw new ModuleException("The executable does not exist");
                }
            }
            else
            {
                throw new ModuleException("No executable given");
            }

            var modelDefinition = new
            {
                Author = author,
                CompanyName = string.Empty,
                Name = name,
                Version = version,
                DateTime.Now.Year,
                Date = DateTime.Now.ToString("dd/MM/yyyy"),
                Guid = Guid.NewGuid(),
                // ReSharper disable once PossibleNullReferenceException
                ModuleRoot = executableFile.Directory.FullName,
                FileName = executableFile.Name,
                RequiredModules = "\"EnvironmentModuleCore\"",
                Dependencies = dependencies.Length > 0 ? dependencies.Select(x => $"@{{Name=\"{x.ModuleFullName}\"; Optional=${x.IsOptional.ToString()}}}").Aggregate((a, b) => a + "," + b) : string.Empty,
                AdditionalDescription = description,
                CustomCode = string.Empty,
                DirectUnload = "$false",
                ModuleType = EnvironmentModuleType.Default.ToString(),
                Category = category,
                Parameters = parameters
            }; 

            FileInfo templatePsd = new FileInfo(Path.Combine(workingDirectory, "Templates\\EnvironmentModule.psd1.template"));
            FileInfo templatePsm = new FileInfo(Path.Combine(workingDirectory, "Templates\\EnvironmentModule.psm1.template"));
            FileInfo templatePse = new FileInfo(Path.Combine(workingDirectory, "Templates\\EnvironmentModule.pse1.template"));

            CreateModuleFromTemplates(modelDefinition, templatePsd, templatePsm, templatePse, storageDirectory, name, version, architecture);
        }

        /// <summary>
        /// Create a new environment module from the given template files.
        /// </summary>
        /// <param name="modelDefinition">The model definition that should be used as data source.</param>
        /// <param name="templatePsd">The template psd file to use.</param>
        /// <param name="templatePsm">The template psm file to use.</param>
        /// <param name="templatePse">The template pse file to use.</param>
        /// <param name="storageDirectory">The directory to store the created module files.</param>
        /// <param name="name">The name of the module.</param>
        /// <param name="version">The version of the module.</param>
        /// <param name="architecture">The architecture of the module.</param>
        private static void CreateModuleFromTemplates(object modelDefinition, FileInfo templatePsd, FileInfo templatePsm, FileInfo templatePse, string storageDirectory, string name, string version, string architecture)
        {
            string targetName = $"{name}{(string.IsNullOrEmpty(version) ? "" : "-" + version)}{(string.IsNullOrEmpty(architecture) ? "" : "-" + architecture)}";
            DirectoryInfo targetDirectory = new DirectoryInfo(Path.Combine(storageDirectory, targetName));

            if (targetDirectory.Exists)
            {
                throw new ModuleException($"A directory at path {targetDirectory.FullName} does already exist");
            }

            if (!new DirectoryInfo(storageDirectory).Exists)
            {
                throw new ModuleException("The root directory does not exist");
            }

            targetDirectory.Create();

            Dictionary<string, string> templateFiles = new Dictionary<string, string>
            {
                [templatePsd.FullName] = Path.Combine(targetDirectory.FullName, targetName + ".psd1"),
                [templatePsm.FullName] = Path.Combine(targetDirectory.FullName, targetName + ".psm1"),
                [templatePse.FullName] = Path.Combine(targetDirectory.FullName, targetName + ".pse1")
            };

            TemplateRenderer.CreateConcreteFilesFromTemplates(modelDefinition, templateFiles);
        }
    }
}
