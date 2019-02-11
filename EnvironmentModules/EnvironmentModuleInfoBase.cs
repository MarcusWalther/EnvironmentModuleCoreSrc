using System.Management.Automation;

namespace EnvironmentModules
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
        /// The module is used to load any concrete (default) module with the same name. This type is used by automatically generated modules.
        /// </summary>
        Meta,
        /// <summary>
        /// This module does provide functions, aliases etc, but can only be loaded by a default module as dependency.
        /// </summary>
        Abstract
    }

    /// <summary>
    /// This class contains basic information about an environment module. This information is written to the cache in order to improve performance.
    /// </summary>
    public class EnvironmentModuleInfoBase
    {
        /// <summary>
        /// The full name of the module. This name can be used to load the module with the help of the powershell-environment.
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// The short name of the module.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The version of the application or library.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// The machine code of the module (e.g. x86, x64, arm...).
        /// </summary>
        public string Architecture { get; set; }

        /// <summary>
        /// Additional infos like compiler or compilation flags (e.g. MSVC15, gcc, ...).
        /// </summary>
        public string AdditionalOptions { get; set; }

        /// <summary>
        /// Specifies the type of the module.
        /// </summary>
        public EnvironmentModuleType ModuleType { get; set; }

        /// <summary>
        /// The PS module info object associated to the environment module.
        /// </summary>
        public PSModuleInfo PSModuleInfo { get; set; }

        public EnvironmentModuleInfoBase(PSModuleInfo psModuleInfo,
            string name,
            string version,
            string architecture,
            string additionalOptions = "",
            EnvironmentModuleType moduleType = EnvironmentModuleType.Default)
        {
            FullName = psModuleInfo.Name;
            ModuleType = moduleType;
            PSModuleInfo = psModuleInfo;
            Name = name;
            Version = version;
            Architecture = architecture;
            AdditionalOptions = additionalOptions;
        }
    }
}
