namespace EnvironmentModules
{
    using System;
    using System.Collections.Generic;

    public class EnvironmentModuleInfo : EnvironmentModuleInfoBase
    {
        /// <summary>
        /// The root directory of the environment module. Can be null.
        /// </summary>
        public string ModuleRoot { get; set; }

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
            string moduleRoot,
            string tmpDirectory,
            string fullName,
            string name,
            string version,
            string architecture,
            string additionalOptions,
            EnvironmentModuleType moduleType = EnvironmentModuleType.Default) : base(fullName, moduleBase, name, version, architecture, additionalOptions, moduleType)
        {
            ModuleRoot = moduleRoot;
            TmpDirectory = tmpDirectory;

            Dependencies = new DependencyInfo[0];
            SearchPaths = new SearchPath[0];
            RequiredFiles = new string[0];

            DirectUnload = false;
            StyleVersion = 1.0;
            Category = "";
            Parameters = new Dictionary<string, string>();
        }

        public EnvironmentModuleInfo(
            EnvironmentModuleInfoBase infoBase,
            string moduleRoot,
            string tmpDirectory) :
            this(infoBase.ModuleBase,
                 moduleRoot,
                 tmpDirectory,
                 infoBase.FullName,
                 infoBase.Name,
                 infoBase.Version,
                 infoBase.Architecture,
                 infoBase.AdditionalOptions,
                 infoBase.ModuleType)
        { }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="other"></param>
        public EnvironmentModuleInfo(EnvironmentModuleInfo other) :
            this(other.ModuleBase,
                other.ModuleRoot,
                 other.TmpDirectory,
                 other.FullName,
                 other.Name,
                 other.Version,
                 other.Architecture,
                 other.AdditionalOptions,
                 other.ModuleType)
        {
            Dependencies = other.Dependencies;
            SearchPaths = other.SearchPaths;
            RequiredFiles = other.RequiredFiles;
            DirectUnload = other.DirectUnload;
            StyleVersion = other.StyleVersion;
            Category = other.Category;
            Parameters = other.Parameters;
        }
    }
}
