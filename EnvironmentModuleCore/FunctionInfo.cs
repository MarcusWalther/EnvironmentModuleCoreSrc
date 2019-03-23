// <copyright file="FunctionInfo.cs">
//     Copyright 2019 Marcus Walther
// </copyright>
// <author>Marcus Walther</author>

namespace EnvironmentModuleCore
{
    /// <summary>
    /// This class represents a function provided by an environment module.
    /// </summary>
    public class FunctionInfo
    {
        /// <summary>
        /// Initializes a new instance of the FunctionInfo class with the given parameters.
        /// </summary>
        /// <param name="name">The name of the function.</param>
        /// <param name="moduleFullName">The name of the module that provides the function.</param>
        /// <param name="definition">The definition of the function, must provide an Invoke function.</param>
        public FunctionInfo(string name, string moduleFullName, object definition)
        {
            Name = name;
            ModuleFullName = moduleFullName;
            Definition = definition;
        }

        #region Properties
        /// <summary>
        /// Gets or sets the function definition. The object is usually a ScriptBlock and must provide the Invoke function.
        /// </summary>
        public object Definition { get; protected set; }

        /// <summary>
        /// Gets or sets the module name that provides the function.
        /// </summary>
        public string ModuleFullName { get; protected set; }

        /// <summary>
        /// Gets or sets the name of the function.
        /// </summary>
        public string Name { get; protected set; }
        #endregion
    }
}
