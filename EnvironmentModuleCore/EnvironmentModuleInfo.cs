// <copyright file="EnvironmentModuleInfo.cs">
//     Copyright 2019 Marcus Walther
// </copyright>
// <author>Marcus Walther</author>

// ReSharper disable UnusedMember.Global
namespace EnvironmentModuleCore
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// This info class represents an environment module. It can be used for modules that are loaded or not already loaded. It contains only meta information.
    /// </summary>
    public class EnvironmentModuleInfo : EnvironmentModuleInfoBase
    {
        #region Private Fields
        /// <summary>
        /// The values associated with these search paths are checked as candidates in order to identify the root path.
        /// </summary>
        private SearchPath[] searchPaths; 
        #endregion

        /// <summary>
        /// Initializes a new instance of the EnvironmentModuleInfo class with the given parameters.
        /// </summary>
        /// <param name="moduleBase">The directory holding the module files (like .psm1, pse1, etc.).</param>
        /// <param name="moduleRoot">The root module that was identified holding the required items.</param>
        /// <param name="tmpDirectory">The tmp. directory that can be used to store session related files etc..</param>
        /// <param name="fullName">The full name of the module.</param>
        /// <param name="name">The short name of the module.</param>
        /// <param name="version">The version of the module.</param>
        /// <param name="architecture">The architecture of the module.</param>
        /// <param name="additionalOptions">Additional options specifying the module.</param>
        /// <param name="moduleType">The type of the module.</param>
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

            Dependencies = Array.Empty<DependencyInfo>();
            SearchPaths = Array.Empty<SearchPath>();
            RequiredItems = Array.Empty<RequiredItem>();

            DirectUnload = false;
            StyleVersion = 1.0;
            Category = string.Empty;
            Parameters = new Dictionary<string, ParameterInfoBase>();
        }

        /// <summary>
        /// Initializes a new instance of the EnvironmentModuleInfo class using the base information object.
        /// </summary>
        /// <param name="infoBase">The base object containing meta information.</param>
        /// <param name="moduleRoot">The root module that was identified holding the required items.</param>
        /// <param name="tmpDirectory">The tmp. directory that can be used to store session related files etc..</param>
        public EnvironmentModuleInfo(EnvironmentModuleInfoBase infoBase, string moduleRoot, string tmpDirectory) :
            this(
                infoBase.ModuleBase,
                moduleRoot,
                tmpDirectory,
                infoBase.FullName,
                infoBase.Name,
                infoBase.Version,
                infoBase.Architecture,
                infoBase.AdditionalOptions,
                infoBase.ModuleType)
        {
        }

        /// <summary>
        /// Initializes a new instance of the EnvironmentModuleInfo class was copy of the given module.
        /// </summary>
        /// <param name="other">The source of the copy constructor.</param>
        public EnvironmentModuleInfo(EnvironmentModuleInfo other) :
            this(
                other.ModuleBase,
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
            RequiredItems = other.RequiredItems;
            DirectUnload = other.DirectUnload;
            StyleVersion = other.StyleVersion;
            Category = other.Category;
            Parameters = other.Parameters;
        }

        #region Properties
        /// <summary>
        /// Gets or sets the root directory of the environment module. The value can be null if the module has type meta or abstract.
        /// </summary>
        public string ModuleRoot { get; set; }

        /// <summary>
        /// Gets or sets the temporary directory that can be used to store files. The temporary directory is individual for each console session.
        /// </summary>
        public string TmpDirectory { get; set; }

        /// <summary>
        /// Gets or sets the environment modules that must be loaded prior this module can be used.
        /// </summary>
        public DependencyInfo[] Dependencies { get; set; }

        /// <summary>
        /// Gets or sets the search items that are checked as candidates in order to identify the root path.
        /// </summary>
        public SearchPath[] SearchPaths
        {
            get => searchPaths;

            set
            {
                searchPaths = value;
                Array.Sort(searchPaths);
            }
        }

        /// <summary>
        /// Gets or sets the items that must be available in the candidate in order to use it as module root path.
        /// </summary>
        public RequiredItem[] RequiredItems { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the module should be unloaded after the import, so that just the dependencies will remain.
        /// </summary>
        public bool DirectUnload { get; set; }

        /// <summary>
        /// Gets or sets the version of the code style used to write the pse/psm file. This field is reserved for the future and not used right now.
        /// </summary>
        public double StyleVersion { get; set; }

        /// <summary>
        /// Gets or sets the category (default storage path) of the environment module.
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Gets or sets the parameters defined by the module.
        /// </summary>
        public Dictionary<string, ParameterInfoBase> Parameters { get; set; }
        #endregion

        #region Public Functions
        /// <summary>
        /// Compares two environment module infos. They are equal if they have the same name, version and architecture.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns>True if the infos are equal, false otherwise.</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is EnvironmentModule module))
            {
                return false;
            }

            return (Name == module.Name) && (Version == module.Version) && (Architecture == module.Architecture);
        }

        /// <summary>
        /// Get the hash code of the info object. The hash is only based on the full name.
        /// </summary>
        /// <returns>The generated hash code.</returns>
        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        /// <summary>
        /// Get the string representing the info object.
        /// </summary>
        /// <returns>The string representing the object.</returns>
        public override string ToString()
        {
            return Name;
        } 
        #endregion
    }
}
