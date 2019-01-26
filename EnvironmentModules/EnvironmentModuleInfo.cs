namespace EnvironmentModules
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Management.Automation;

    public class EnvironmentModuleInfo : EnvironmentModuleInfoBase
    {
        /// <summary>
        /// The base directory of the environment module. Should be the same as for the underlaying 
        /// PowerShell module.
        /// </summary>
        public DirectoryInfo ModuleBase { get; set; }

        /// <summary>
        /// The temporary directory that can be used to store files.
        /// </summary>
        public DirectoryInfo TmpDirectory { get; set; }

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
        public string AdditionalInfo { get; set; }

        /// <summary>
        /// All environment modules that must be loaded prior this module can be used.
        /// </summary>
        public string[] RequiredEnvironmentModules { get; set; }

        /// <summary>
        /// The values associated with these search paths are checked.
        /// </summary>
        private SearchPath[] searchPaths;

        public SearchPath[] SearchPaths
        {
            get { return searchPaths; }
            set
            {
                searchPaths = value;
                Array.Sort(searchPaths);
            }
        }


        /// <summary>
        /// The files that must be available in the folder candidate.
        /// </summary>
        public string[] RequiredFiles { get; set; }
 
        /// <summary>
        /// This value indicates whether the module should be unloaded after the import, so that just the dependencies will remain.
        /// </summary>
        public bool DirectUnload { get; set; }

        /// <summary>
        /// The version of the code style used to write the pse/psm file.
        /// </summary>
        public double StyleVersion { get; set; }

        /// <summary>
        /// The category (default storage path) of the environment module.
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// The parameters defaults defined by the module.
        /// </summary>
        public Dictionary<string, string> Parameters { get; set; }

        public EnvironmentModuleInfo(
            PSModuleInfo psModuleInfo,
            DirectoryInfo moduleBase,
            DirectoryInfo tmpDirectory,
            string name,
            string version,
            string architecture,
            string additionalInfo = "",
            EnvironmentModuleType moduleType = EnvironmentModuleType.Default,
            string[] requiredEnvironmentModules = null,
            SearchPath[] searchPaths = null,
            string[] requiredFiles = null,
            bool directUnload = false,
            double styleVersion = 0.0,
            string category = "",
            Dictionary<string, string> parameters = null) : base(psModuleInfo, moduleType)
        {
            ModuleBase = moduleBase;
            TmpDirectory = tmpDirectory;
            Name = name;
            Version = version;
            Architecture = architecture;
            AdditionalInfo = additionalInfo;

            RequiredEnvironmentModules = requiredEnvironmentModules ?? new string[0];
            SearchPaths = searchPaths ?? new SearchPath[0];
            RequiredFiles = requiredFiles ?? new string[0];

            DirectUnload = directUnload;
            StyleVersion = styleVersion;
            Category = category;
            Parameters = parameters == null ? new Dictionary<string, string>() : parameters;
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="other"></param>
        public EnvironmentModuleInfo(EnvironmentModuleInfo other) :
            this(other.PSModuleInfo,
                 other.ModuleBase,
                 other.TmpDirectory,
                 other.Name, other.Version,
                 other.Architecture,
                 other.AdditionalInfo,
                 other.ModuleType,
                 other.RequiredEnvironmentModules,
                 other.SearchPaths,
                 other.RequiredFiles,
                 other.DirectUnload,
                 other.StyleVersion,
                 other.Category,
                 other.Parameters)
        { }
    }
}
