namespace EnvironmentModules
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    public class EnvironmentModuleInfo : EnvironmentModuleInfoBase
    {
        /// <summary>
        /// The temporary directory that can be used to store files.
        /// </summary>
        public string TmpDirectory { get; set; }

        /// <summary>
        /// All environment modules that must be loaded prior this module can be used.
        /// </summary>
        public DependencyInfo[] Dependencies { get; set; }

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
            string moduleBase,
            string tmpDirectory,
            string fullName,
            string name,
            string version,
            string architecture,
            string additionalOptions = "",
            EnvironmentModuleType moduleType = EnvironmentModuleType.Default,
            DependencyInfo[] dependencies = null,
            SearchPath[] searchPaths = null,
            string[] requiredFiles = null,
            bool directUnload = false,
            double styleVersion = 0.0,
            string category = "",
            Dictionary<string, string> parameters = null) : base(fullName, moduleBase, name, version, architecture, additionalOptions, moduleType)
        {
            TmpDirectory = tmpDirectory;

            Dependencies = dependencies ?? new DependencyInfo[0];
            SearchPaths = searchPaths ?? new SearchPath[0];
            RequiredFiles = requiredFiles ?? new string[0];

            DirectUnload = directUnload;
            StyleVersion = styleVersion;
            Category = category;
            Parameters = parameters ?? new Dictionary<string, string>();
        }

        public EnvironmentModuleInfo(
            EnvironmentModuleInfoBase infoBase,
            string tmpDirectory,
            DependencyInfo[] dependencies = null,
            SearchPath[] searchPaths = null,
            string[] requiredFiles = null,
            bool directUnload = false,
            double styleVersion = 0.0,
            string category = "",
            Dictionary<string, string> parameters = null) :
            this(infoBase.ModuleBase,
                 tmpDirectory,
                 infoBase.FullName,
                 infoBase.Name,
                 infoBase.Version,
                 infoBase.Architecture,
                 infoBase.AdditionalOptions,
                 infoBase.ModuleType,
                 dependencies,
                 searchPaths,
                 requiredFiles,
                 directUnload,
                 styleVersion,
                 category,
                 parameters)
        { }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="other"></param>
        public EnvironmentModuleInfo(EnvironmentModuleInfo other) :
            this(other.ModuleBase,
                 other.TmpDirectory,
                 other.FullName,
                 other.Name,
                 other.Version,
                 other.Architecture,
                 other.AdditionalOptions,
                 other.ModuleType,
                 other.Dependencies,
                 other.SearchPaths,
                 other.RequiredFiles,
                 other.DirectUnload,
                 other.StyleVersion,
                 other.Category,
                 other.Parameters)
        { }
    }
}
