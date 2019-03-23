// <copyright file="EnvironmentModule.cs">
//     Copyright 2019 Marcus Walther
// </copyright>
// <author>Marcus Walther</author>

// ReSharper disable UnusedMember.Global
namespace EnvironmentModuleCore
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// This class represents a loaded environment module in the environment. It holds references to all information given by the user or the description file.
    /// </summary>
    public class EnvironmentModule : EnvironmentModuleInfo
    {
        #region Private Fields
        /// <summary>
        /// All path info objects that can be accesses using the Properties "AppendPaths", "Paths" and "PrependPaths".
        /// </summary>
        private readonly Dictionary<string, PathInfo> pathInfos;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the EnvironmentModule class with the given parameters.
        /// </summary>
        /// <param name="baseModule">The info object of the module.</param>
        /// <param name="isLoadedDirectly">True if the module was loaded directly by the user. False if the module was loaded as dependency of another module.</param>
        /// <param name="sourceModule">The module that has triggered the loading. Should be set if isLoadedDirectly is false.</param>
        public EnvironmentModule(
            EnvironmentModuleInfo baseModule,
            bool isLoadedDirectly = true,
            EnvironmentModuleInfo sourceModule = null) : 
            base(baseModule)
        {
            IsLoadedDirectly = isLoadedDirectly;
            SourceModule = sourceModule;
            pathInfos = new Dictionary<string, PathInfo>();
            Aliases = new Dictionary<string, AliasInfo>();
            Functions = new Dictionary<string, FunctionInfo>();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the collection of aliases (dictionary-keys) that are set if the module is loaded. The aliases
        /// are deleted if unload is called. The value represents the command and an optional description.
        /// </summary>
        public Dictionary<string, AliasInfo> Aliases { get; protected set; }

        /// <summary>
        /// Gets a collection of paths that are appended to the environment variables when the module is loaded. The values
        /// are removed from the environment when unload is called.
        /// </summary>
        public HashSet<PathInfo> AppendPaths
        {
            get { return new HashSet<PathInfo>(pathInfos.Values.Where(x => x.PathType == PathType.APPEND)); }
        }

        /// <summary>
        /// Gets or sets a collection of aliases (dictionary-keys) that are set if the module is loaded. The aliases
        /// are deleted if unload is called. The value represents the command.
        /// </summary>
        public Dictionary<string, FunctionInfo> Functions { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether the module was loaded by the user or as dependency of another module.
        /// </summary>
        public bool IsLoadedDirectly { get; set; }

        /// <summary>
        /// Gets a collection of paths that are added to the environment variables when the module is loaded. The values
        /// are removed from the environment-variable when unload is called.
        /// </summary>
        public HashSet<PathInfo> Paths => new HashSet<PathInfo>(pathInfos.Values);

        /// <summary>
        /// Gets a collection of paths that are prepended to the environment variables when the module is loaded. The values
        /// are removed from the environment-variable when unload is called.
        /// </summary>
        public HashSet<PathInfo> PrependPaths
        {
            get { return new HashSet<PathInfo>(pathInfos.Values.Where(x => x.PathType == PathType.PREPEND)); }
        }

        /// <summary>
        /// Gets or sets the reference counter that is decreased when the module is removed and increased when loaded or referenced as dependency.
        /// </summary>
        public int ReferenceCounter { get; set; }

        /// <summary>
        /// Gets a collection of paths that are set to the environment variables when the module is loaded. The values
        /// are removed from the environment-variable when unload is called.
        /// </summary>
        public HashSet<PathInfo> SetPaths
        {
            get { return new HashSet<PathInfo>(pathInfos.Values.Where(x => x.PathType == PathType.SET)); }
        }

        /// <summary>
        /// Gets or sets the module that has triggered the loading of the module. The value is null if the module was loaded directly by the user.
        /// </summary>
        public EnvironmentModuleInfo SourceModule { get; protected set; }
        #endregion

        #region Public Functions
        /// <summary>
        /// Add a prepend environment variable manipulation to the definition.
        /// </summary>
        /// <param name="envVar">The environment variable to modify.</param>
        /// <param name="path">The value to prepend.</param>
        public void AddPrependPath(string envVar, string path)
        {
            AddPath(PathType.PREPEND, envVar, path);
        }

        /// <summary>
        /// Add a append environment variable manipulation to the definition.
        /// </summary>
        /// <param name="envVar">The environment variable to modify.</param>
        /// <param name="path">The value to append.</param>
        public void AddAppendPath(string envVar, string path)
        {
            AddPath(PathType.APPEND, envVar, path);
        }

        /// <summary>
        /// Add a environment variable manipulation to the definition that overwrites or defines an environment variable.
        /// </summary>
        /// <param name="envVar">The environment variable to modify.</param>
        /// <param name="path">The value to set.</param>
        public void AddSetPath(string envVar, string path)
        {
            AddPath(PathType.SET, envVar, path);
        }

        /// <summary>
        /// Add a new alias definition to the environment module.
        /// </summary>
        /// <param name="aliasName">The name of the alias to define.</param>
        /// <param name="command">The command that should be executed on invocation.</param>
        /// <param name="description">An additional description that can be displayed for the user.</param>
        public void AddAlias(string aliasName, string command, string description = "")
        {
            Aliases[aliasName] = new AliasInfo(aliasName, FullName, command, description);
        }

        /// <summary>
        /// Add a new function definition to the environment module.
        /// </summary>
        /// <param name="functionName">The name of the function to define.</param>
        /// <param name="content">The function content. Usually this is a ScriptBlock. The object must provide an Invoke function.</param>
        public void AddFunction(string functionName, object content)
        {
            Functions[functionName] = new FunctionInfo(functionName, FullName, content);
        }
        #endregion

        #region Protected Functions
        /// <summary>
        /// Add a new environment variable manipulation to the definition of the environment module.
        /// </summary>
        /// <param name="pathType">The type of the path manipulation.</param>
        /// <param name="variable">The environment variable to modify.</param>
        /// <param name="value">The new value to use for the manipulation.</param>
        protected void AddPath(PathType pathType, string variable, string value)
        {
            string key = $"{pathType.ToString()}_{variable}";
            if (!pathInfos.TryGetValue(key, out var info))
            {
                info = new PathInfo(FullName, pathType, variable, new List<string>() { value });
            }
            else
            {
                info.Values.Add(value);
            }

            pathInfos[key] = info;
        }
        #endregion
    }
}