// <copyright file="DependencyInfo.cs">
//     Copyright 2025 Marcus Walther
// </copyright>
// <author>Marcus Walther</author>

namespace EnvironmentModuleCore
{
    /// <summary>
    /// This class represents a dependency to another environment module.
    /// </summary>
    public class DependencyInfo
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the DependencyInfo class with the given parameters.
        /// </summary>
        /// <param name="moduleFullName">The full name of the dependency environment module.</param>
        /// <param name="isOptional">True if the dependency is not mandatory.</param>
        /// <param name="priority">The priority of the module dependency, only relevant for module merging.</param>
        public DependencyInfo(string moduleFullName, bool isOptional = false, int priority = 0)
        {
            ModuleFullName = moduleFullName;
            IsOptional = isOptional;
            Priority = priority;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether the dependency is optional or mandatory.
        /// </summary>
        public bool IsOptional { get; set; }

        /// <summary>
        /// Gets or sets the name of the dependency environment module.
        /// </summary>
        public string ModuleFullName { get; protected set; }

        /// <summary>
        /// Gets or sets the priority of the dependency. It is relevant for the module merging.
        /// </summary>
        public int Priority { get; set; }
        #endregion
    }
}
