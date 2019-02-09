namespace EnvironmentModules
{
    using System.Collections.Generic;
    using System.Linq;

    public class EnvironmentModule : EnvironmentModuleInfo
    {
        private Dictionary<string, EnvironmentModulePathInfo> pathInfos;

        #region Properties
        /// <summary>
        /// The path to the root directory of the module.
        /// </summary>
        public string ModuleRoot { get; private set; }
        /// <summary>
        /// A reference counter indicating that is decreased when the module is removed and increased when loaded.
        /// </summary>
        public int ReferenceCounter { get; set; }
        /// <summary>
        /// A collection of paths that are added to the environment variables when the module is loaded. The values
        /// are removed from the environment-variable when unload is called.
        /// </summary>
        public HashSet<EnvironmentModulePathInfo> Paths { get { return new HashSet<EnvironmentModulePathInfo>(pathInfos.Values); } }
        /// <summary>
        /// A collection of paths that are appended to the environment variables when the module is loaded. The values
        /// are removed from the environment-variable when unload is called.
        /// </summary>
        public HashSet<EnvironmentModulePathInfo> AppendPaths { get { return new HashSet<EnvironmentModulePathInfo>(pathInfos.Values.Where(x => x.PathType == EnvironmentModulePathType.APPEND)); } }
        /// <summary>
        /// A collection of paths that are prepended to the environment variables when the module is loaded. The values
        /// are removed from the environment-variable when unload is called.
        /// </summary>
        public HashSet<EnvironmentModulePathInfo> PrependPaths { get { return new HashSet<EnvironmentModulePathInfo>(pathInfos.Values.Where(x => x.PathType == EnvironmentModulePathType.PREPEND)); } }
        /// <summary>
        /// A collection of paths that are set to the environment variables when the module is loaded. The values
        /// are removed from the environment-variable when unload is called.
        /// </summary>
        public HashSet<EnvironmentModulePathInfo> SetPaths { get { return new HashSet<EnvironmentModulePathInfo>(pathInfos.Values.Where(x => x.PathType == EnvironmentModulePathType.SET)); } }
        /// <summary>
        /// A collection of aliases (dictionary-keys) that are set if the module is loaded. The aliases
        /// are deleted if unload is called. The value represents the command and an optional description.
        /// </summary>
        public Dictionary<string, EnvironmentModuleAliasInfo> Aliases { get; protected set; }
        /// <summary>
        /// A collection of aliases (dictionary-keys) that are set if the module is loaded. The aliases
        /// are deleted if unload is called. The value represents the command.
        /// </summary>
        public Dictionary<string, EnvironmentModuleFunctionInfo> Functions { get; protected set; }

        /// <summary>
        /// This value indicates if the module was loaded by the user or as dependency of another module.
        /// </summary>
        public bool IsLoadedDirectly { get; set; }
        /// <summary>
        /// Gets the module that has triggered the loading of the module. The value is null if the module was loaded directly by the user.
        /// </summary>
        public EnvironmentModuleInfo SourceModule { get; protected set; }
        #endregion

        #region Constructors
        public EnvironmentModule(
            EnvironmentModuleInfo baseModule,
            string moduleRoot,
            bool isLoadedDirectly = true,
            EnvironmentModuleInfo sourceModule = null) : 
            base(baseModule)
        {
            ModuleRoot = moduleRoot;
            IsLoadedDirectly = isLoadedDirectly;
            SourceModule = sourceModule;
            pathInfos = new Dictionary<string, EnvironmentModulePathInfo>();
            Aliases = new Dictionary<string, EnvironmentModuleAliasInfo>();
            Functions = new Dictionary<string, EnvironmentModuleFunctionInfo>();
        }
        #endregion

        protected void AddPath(EnvironmentModulePathType pathType, string variable, string value)
        {
            string key = $"{pathType.ToString()}_{variable}";
            EnvironmentModulePathInfo info;
            if(!pathInfos.TryGetValue(key, out info))
            {
                info = new EnvironmentModulePathInfo(FullName, pathType, variable, new List<string>() { value });
            }
            else
            {
                info.Values.Add(value);
            }

            pathInfos[key] = info;
        }

        public void AddPrependPath(string envVar, string path)
        {
            AddPath(EnvironmentModulePathType.PREPEND, envVar, path);
        }

        public void AddAppendPath(string envVar, string path)
        {
            AddPath(EnvironmentModulePathType.APPEND, envVar, path);
        }

        public void AddSetPath(string envVar, string path)
        {
            AddPath(EnvironmentModulePathType.SET, envVar, path);
        }

        public void AddAlias(string aliasName, string command, string description="")
        {
            Aliases[aliasName] = new EnvironmentModuleAliasInfo(aliasName, FullName, command, description);
        }

        public void AddFunction(string functionName, System.Management.Automation.ScriptBlock content)
        {
            Functions[functionName] = new EnvironmentModuleFunctionInfo(functionName, FullName, content);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is EnvironmentModule))
                return false;

            EnvironmentModule em = (EnvironmentModule) obj;
            return (Name == em.Name) && (Version == em.Version) && (Architecture == em.Architecture);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}