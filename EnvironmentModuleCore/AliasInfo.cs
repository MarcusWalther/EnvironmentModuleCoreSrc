// <copyright file="AliasInfo.cs">
//     Copyright 2019 Marcus Walther
// </copyright>
// <author>Marcus Walther</author>

namespace EnvironmentModuleCore
{
    /// <summary>
    /// This container class stores all information related to an alias definition in the Powershell environment. 
    /// </summary>
    public class AliasInfo
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the AliasInfo class with the given parameters.
        /// </summary>
        /// <param name="name">The name of the alias.</param>
        /// <param name="moduleFullName">The module that defines the alias.</param>
        /// <param name="definition">The definition that is executed on alias invocation.</param>
        /// <param name="description">The additional description of the alias.</param>
        public AliasInfo(string name, string moduleFullName, string definition, string description)
        {
            Name = name;
            ModuleFullName = moduleFullName;
            Definition = definition;
            Description = description;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the definition of the alias that is executed on invocation.
        /// </summary>
        public string Definition { get; protected set; }

        /// <summary>
        /// Gets or sets an additional description that can be displayed.
        /// </summary>
        public string Description { get; protected set; }

        /// <summary>
        /// Gets or sets the name of the module that has specified the alias.
        /// </summary>
        public string ModuleFullName { get; protected set; }

        /// <summary>
        /// Gets or sets the name of the alias.
        /// </summary>
        public string Name { get; protected set; }
        #endregion
    }
}
