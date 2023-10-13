// <copyright file="EnvironmentModule.cs">
//     Copyright 2019 Marcus Walther
// </copyright>
// <author>Marcus Walther</author>

// ReSharper disable UnusedMember.Global
namespace EnvironmentModuleCore
{
    using System.Collections.Generic;

    /// <summary>
    /// This class represents a loaded environment module in the environment. It holds references to all information given by the user or the description file.
    /// </summary>
    public class EnvironmentModule : EnvironmentModuleInfo
    {
        public delegate void FunctionAddedHandler(FunctionInfo sender, EnvironmentModule module);
        public delegate void AliasAddedHandler(AliasInfo sender, EnvironmentModule module);
        public delegate void LoadedHandler(EnvironmentModule module);
        public event FunctionAddedHandler OnFunctionAdded;
        public event AliasAddedHandler OnAliasAdded;
        public event LoadedHandler OnLoaded;
        public event LoadedHandler OnUnloaded;

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
        /// Gets or sets a collection of aliases (dictionary-keys) that are set if the module is loaded. The aliases
        /// are deleted if unload is called. The value represents the command.
        /// </summary>
        public Dictionary<string, FunctionInfo> Functions { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether the module was loaded by the user or as dependency of another module.
        /// </summary>
        public bool IsLoadedDirectly { get; set; }

        /// <summary>
        /// Gets or sets the reference counter that is decreased when the module is removed and increased when loaded or referenced as dependency.
        /// </summary>
        public int ReferenceCounter { get; set; }

        /// <summary>
        /// Gets or sets the module that has triggered the loading of the module. The value is null if the module was loaded directly by the user.
        /// </summary>
        public EnvironmentModuleInfo SourceModule { get; protected set; }
        #endregion

        #region Public Functions
        /// <summary>
        /// Add a new alias definition to the environment module.
        /// </summary>
        /// <param name="aliasName">The name of the alias to define.</param>
        /// <param name="command">The command that should be executed on invocation.</param>
        /// <param name="description">An additional description that can be displayed for the user.</param>
        public void AddAlias(string aliasName, string command, string description = "")
        {
            AliasInfo alias = new AliasInfo(aliasName, FullName, command, description);
            Aliases[aliasName] = alias;
            OnAliasAdded?.Invoke(alias, this);
        }

        /// <summary>
        /// Add a new function definition to the environment module.
        /// </summary>
        /// <param name="functionName">The name of the function to define.</param>
        /// <param name="content">The function content. Usually this is a ScriptBlock. The object must provide an Invoke function.</param>
        public void AddFunction(string functionName, object content)
        {
            FunctionInfo function = new FunctionInfo(functionName, FullName, content);
            Functions[functionName] = function;
            OnFunctionAdded?.Invoke(function, this);
        }

        /// <summary>
        /// The function that indicates that the loading process was completed. It will trigger the OnLoaded event.
        /// </summary>
        public void FullyLoaded()
        {
            OnLoaded?.Invoke(this);
        }

        /// <summary>
        /// The function that indicates that the unloading process was completed. It will trigger the OnUnloaded event.
        /// </summary>
        public void FullyUnloaded()
        {
            OnUnloaded?.Invoke(this);
            OnLoaded = null;
            OnUnloaded = null;
        }
        #endregion
    }
}