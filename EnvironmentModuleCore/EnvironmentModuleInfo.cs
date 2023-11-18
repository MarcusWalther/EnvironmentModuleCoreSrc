// <copyright file="EnvironmentModuleInfo.cs">
//     Copyright 2019 Marcus Walther
// </copyright>
// <author>Marcus Walther</author>

// ReSharper disable UnusedMember.Global

using System.IO;

namespace EnvironmentModuleCore
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// This info class represents an environment module. It can be used for modules that are loaded or not already loaded. It contains only meta information.
    /// </summary>
    public class EnvironmentModuleInfo : EnvironmentModuleInfoBase
    {
        public delegate void PathAddedHandler(PathInfo sender, EnvironmentModuleInfo module);
        public event PathInfo.PathUpdateHandler OnPathChanged;
        public event PathAddedHandler OnPathAdded;

        #region Private Fields
        /// <summary>
        /// The values associated with these search paths are checked as candidates in order to identify the root path.
        /// </summary>
        private SearchPath[] searchPaths;

        /// <summary>
        /// All path info objects that can be accesses using the Properties "AppendPaths", "Paths" and "PrependPaths".
        /// </summary>
        private readonly Dictionary<string, PathInfo> pathInfos;
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
            MergeModules = Array.Empty<string>();

            DirectUnload = false;
            StyleVersion = 1.0;
            Category = string.Empty;
            Parameters = new Dictionary<Tuple<string, string>, ParameterInfoBase>();
            pathInfos = new Dictionary<string, PathInfo>();
        }

        /// <summary>
        /// Initializes a new instance of the EnvironmentModuleInfo class without any values.
        /// </summary>
        public EnvironmentModuleInfo() : this(null, null, null, null, null, null, null, null)
        {
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
            MergeModules = other.MergeModules;
            DirectUnload = other.DirectUnload;
            StyleVersion = other.StyleVersion;
            Category = other.Category;
            Parameters = other.Parameters;
            pathInfos = new Dictionary<string, PathInfo>();
            foreach (PathInfo otherPath in other.Paths)
            {
                AddPath(otherPath.PathType, otherPath.Variable, string.Join(Path.PathSeparator.ToString(), otherPath.Values), otherPath.Key);
            }
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
        /// Gets or sets a list of module files that should be considered for merging.
        /// </summary>
        public string[] MergeModules { get; set; }

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
        /// Gets or sets the parameters defined by the module. The key is ("ParameterName", "VirtualEnvironment).
        /// </summary>
        public Dictionary<Tuple<string, string>, ParameterInfoBase> Parameters { get; set; }

        /// <summary>
        /// Gets a collection of paths that are added to the environment variables when the module is loaded. The values
        /// are removed from the environment-variable when unload is called.
        /// </summary>
        public HashSet<PathInfo> Paths => new HashSet<PathInfo>(pathInfos.Values);

        /// <summary>
        /// Gets a collection of paths that are appended to the environment variables when the module is loaded. The values
        /// are removed from the environment when unload is called.
        /// </summary>
        public HashSet<PathInfo> AppendPaths
        {
            get { return new HashSet<PathInfo>(pathInfos.Values.Where(x => x.PathType == PathType.APPEND)); }
        }

        /// <summary>
        /// Gets a collection of paths that are prepended to the environment variables when the module is loaded. The values
        /// are removed from the environment-variable when unload is called.
        /// </summary>
        public HashSet<PathInfo> PrependPaths
        {
            get { return new HashSet<PathInfo>(pathInfos.Values.Where(x => x.PathType == PathType.PREPEND)); }
        }

        /// <summary>
        /// Gets a collection of paths that are set to the environment variables when the module is loaded. The values
        /// are removed from the environment-variable when unload is called.
        /// </summary>
        public HashSet<PathInfo> SetPaths
        {
            get { return new HashSet<PathInfo>(pathInfos.Values.Where(x => x.PathType == PathType.SET)); }
        }
        #endregion

        #region Public Functions
        /// <summary>
        /// Add a prepend environment variable manipulation to the definition.
        /// </summary>
        /// <param name="envVar">The environment variable to modify.</param>
        /// <param name="path">The value to prepend.</param>
        /// <param name="key">The unique key of the path modification.</param>
        /// <returns>The path info object containing the information of the path.</returns>
        public PathInfo AddPrependPath(string envVar, string path, string key = null)
        {
            return AddPath(PathType.PREPEND, envVar, path, key);
        }

        /// <summary>
        /// Add a append environment variable manipulation to the definition.
        /// </summary>
        /// <param name="envVar">The environment variable to modify.</param>
        /// <param name="path">The value to append.</param>
        /// <param name="key">The unique key of the path modification.</param>
        /// <returns>The path info object containing the information of the path.</returns>
        public PathInfo AddAppendPath(string envVar, string path, string key = null)
        {
            return AddPath(PathType.APPEND, envVar, path, key);
        }

        /// <summary>
        /// Add a environment variable manipulation to the definition that overwrites or defines an environment variable.
        /// </summary>
        /// <param name="envVar">The environment variable to modify.</param>
        /// <param name="path">The value to set.</param>
        /// <param name="key">The unique key of the path modification.</param>
        /// <returns>The path info object containing the information of the path.</returns>
        public PathInfo AddSetPath(string envVar, string path, string key = null)
        {
            return AddPath(PathType.SET, envVar, path, key);
        }

        /// <summary>
        /// Add a new path info to the internal storage.
        /// </summary>
        /// <param name="info">The path info object defining the environment variable and the value to add.</param>
        /// <returns>The created path info object that was stored internally.</returns>
        public PathInfo AddPath(PathInfo info)
        {
            return AddPath(info.PathType, info.Variable, info.Values, info.Key);
        }

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

        #region Private Functions
        /// <summary>
        /// Add a new environment variable manipulation to the definition of the environment module.
        /// </summary>
        /// <param name="pathType">The type of the path manipulation.</param>
        /// <param name="variable">The environment variable to modify.</param>
        /// <param name="value">The new value to use for the manipulation. Multiple entries are separated by the Path separator of the System (like ';' on Windows).</param>
        /// <param name="key">The unique key of the path modification.</param>
        private PathInfo AddPath(PathType pathType, string variable, string value, string key)
        {
            var values = value.Split(Path.PathSeparator).ToList();
            return AddPath(pathType, variable, values, key);
        }

        /// <summary>
        /// Add a new environment variable manipulation to the definition of the environment module.
        /// </summary>
        /// <param name="pathType">The type of the path manipulation.</param>
        /// <param name="variable">The environment variable to modify.</param>
        /// <param name="values">The new values to use for the manipulation.</param>
        /// <param name="key">The unique key of the path modification.</param>
        private PathInfo AddPath(PathType pathType, string variable, List<string> values, string key) {
            string internalKey = $"{pathType.ToString()}_{variable}";
            if (pathType == PathType.APPEND || pathType == PathType.PREPEND)
                internalKey += $"_{key}";  // The append mode does support different values with different keys

            if (!pathInfos.TryGetValue(internalKey, out var info))
            {
                info = new PathInfo(FullName, pathType, variable, values, key);
                info.OnValueChanged += (sender, args) => OnPathChanged?.Invoke(sender, args);
            }
            else
            {
                if (pathType == PathType.SET)
                    info.Values = values;
                else
                    info.Values.AddRange(values);
            }

            pathInfos[internalKey] = info;
            OnPathAdded?.Invoke(info, this);

            return info;
        }
        #endregion
    }
}
