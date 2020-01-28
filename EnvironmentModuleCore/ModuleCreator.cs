// <copyright file="ModuleCreator.cs">
//     Copyright 2019 Marcus Walther
// </copyright>
// <author>Marcus Walther</author>

namespace EnvironmentModuleCore
{
    using System;
    using System.Collections.Generic;
    using System.IO;

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
        /// <param name="description">The description for the module.</param>
        /// <param name="dependencies">Additional environment modules that must be loaded as dependencies.</param>
        // ReSharper disable once UnusedMember.Global
        public static void CreateMetaEnvironmentModule(
            string name, 
            string storageDirectory,
            string workingDirectory, 
            bool directUnload, 
            string description,
            DependencyInfo[] dependencies = null)
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

            if (dependencies == null)
            {
                dependencies = new DependencyInfo[] { };
            }

            var modelDefinition = new
            {
                Author = "EnvironmentModule",
                CompanyName = string.Empty,
                Name = name,
                DateTime.Now.Year,
                Date            = DateTime.Now.ToString("dd/MM/yyyy"),
                Guid            = Guid.NewGuid(),
                SearchPaths = new SearchPath[]{},
                RequiredItems = new RequiredItem[] {},
                RequiredModules = "\"EnvironmentModuleCore\"",
                Dependencies = dependencies,
                CustomCode      = string.Empty,
                Description = description,
                DirectUnload    = $"${directUnload}",
                ModuleType      = EnvironmentModuleType.Meta.ToString(),
                Parameters      = new Dictionary<string, string>()
            };

            FileInfo templatePsd = new FileInfo(Path.Combine(workingDirectory, "Templates\\EnvironmentModule.psd1.template"));
            FileInfo templatePsm = new FileInfo(Path.Combine(workingDirectory, "Templates\\MetaEnvironmentModule.psm1.template"));
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
        /// <param name="company">The company of the module</param>
        /// <param name="version">The version of the module.</param>
        /// <param name="architecture">The architecture of the module.</param>
        /// <param name="requiredItems">The items that are required by the module.</param>
        /// <param name="defaultSearchPaths">The default search paths to consider for the mounting process.</param>
        /// <param name="dependencies">The dependencies of the environment module.</param>
        /// <param name="parameters">The parameters as key value pairs.</param>
        // ReSharper disable once UnusedMember.Global
        public static void CreateEnvironmentModule(
            string name,
            string storageDirectory,
            string description,
            string workingDirectory = null,
            string author = null,
            string company = null,
            string version = null,
            string architecture = null,
            RequiredItem[] requiredItems = null,
            SearchPath[] defaultSearchPaths = null,
            DependencyInfo[] dependencies = null,
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

            if (string.IsNullOrEmpty(company))
            {
                company = string.Empty;
            }

            if (string.IsNullOrEmpty(description))
            {
                description = string.Empty;
            }

            if (dependencies == null)
            {
                dependencies = new DependencyInfo[] { };
            }

            if (parameters == null)
            {
                parameters = new Dictionary<string, string>();
            }

            var modelDefinition = new
            {
                Author = author,
                CompanyName = company,
                Name = name,
                Version = version,
                DateTime.Now.Year,
                Date = DateTime.Now.ToString("dd/MM/yyyy"),
                Guid = Guid.NewGuid(),
                SearchPaths = defaultSearchPaths,
                RequiredItems = requiredItems,
                RequiredModules = "\"EnvironmentModuleCore\"",
                Dependencies = dependencies,
                Description = description,
                CustomCode = string.Empty,
                DirectUnload = "$false",
                ModuleType = EnvironmentModuleType.Default.ToString(),
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
