// <copyright file="EnvironmentModuleInfoBase.cs">
//     Copyright 2019 Marcus Walther
// </copyright>
// <author>Marcus Walther</author>

namespace EnvironmentModuleCore
{
    /// <summary>
    /// This enum defines all types that Environment Modules can have.
    /// </summary>
    public enum EnvironmentModuleType
    {
        /// <summary>
        /// The module is a common module that has dependencies and provides functions etc.
        /// </summary>
        Default,

        /// <summary>
        /// The module is used to load a concrete (default) module with the same name. This type is used by automatically generated modules with the directly unloading flag.
        /// </summary>
        Meta,
        
        /// <summary>
        /// This module does provide functions, aliases etc, but cannot be loaded directly.
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        Abstract
    }

    /// <summary>
    /// This class contains basic information about an environment module. This information must be serializable, because it is written to the cache in order to improve performance.
    /// The information provided by the class can be read from the static description files (pse, psm).
    /// </summary>
    public class EnvironmentModuleInfoBase
    {
        // ATTENTION: Do not add a reference to the Powershell Module object. Otherwise it cannot be serialized on all platforms by the Powershell.

        /// <summary>
        /// Initializes a new instance of the EnvironmentModuleInfoBase class with the given parameters.
        /// </summary>
        /// <param name="fullName">The full name of the module.</param>
        /// <param name="moduleBase">The directory holding the module files (like .psm1, pse1, etc.).</param>
        /// <param name="name">The short name of the module.</param>
        /// <param name="version">The version of the module.</param>
        /// <param name="architecture">The architecture of the module.</param>
        /// <param name="additionalOptions">Additional options specifying the module.</param>
        /// <param name="moduleType">The type of the module.</param>
        public EnvironmentModuleInfoBase(
            string fullName,
            string moduleBase,
            string name,
            string version,
            string architecture,
            string additionalOptions,
            EnvironmentModuleType moduleType)
        {
            FullName = fullName;
            ModuleBase = moduleBase;
            ModuleType = moduleType;
            Name = name;
            Version = version;
            Architecture = architecture;
            AdditionalOptions = additionalOptions;
        }

        #region Properties
        /// <summary>
        /// Gets or sets the additional infos like compiler or compilation flags (e.g. MSVC15, gcc, ...).
        /// </summary>
        public string AdditionalOptions { get; protected set; }

        /// <summary>
        /// Gets or sets the machine architecture of the module (e.g. x86, x64, arm...).
        /// </summary>
        public string Architecture { get; protected set; }

        /// <summary>
        /// Gets or sets the full name of the module. This name can be used to load the module with the help of the powershell-environment.
        /// </summary>
        public string FullName { get; protected set; }

        /// <summary>
        /// Gets or sets the base directory of the environment module. Should be the same as for the underlying 
        /// PowerShell module.
        /// </summary>
        public string ModuleBase { get; protected set; }

        /// <summary>
        /// Gets or sets the the type of the module.
        /// </summary>
        public EnvironmentModuleType ModuleType { get; set; }

        /// <summary>
        /// Gets the short name of the module.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets or sets the version of the application or library.
        /// </summary>
        public string Version { get; protected set; }
        #endregion
    }
}
