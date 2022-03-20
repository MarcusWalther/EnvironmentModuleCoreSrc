﻿// <copyright file="ParameterInfo.cs">
//     Copyright 2019 Marcus Walther
// </copyright>
// <author>Marcus Walther</author>

namespace EnvironmentModuleCore
{
    /// <summary>
    /// This class represents a parameter that is provided by an environment module.
    /// </summary>
    // ReSharper disable once UnusedMember.Global
    public class ParameterInfo : ParameterInfoBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the ParameterInfo class with the given parameters.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="moduleFullName">The module providing the parameter.</param>
        /// <param name="value">The value of the parameter.</param>
        /// <param name="isUserDefined">True if the value was defined manually by the user.</param>
        public ParameterInfo(string name, string moduleFullName, string value, bool isUserDefined) : base(name, value, isUserDefined)
        {
            ModuleFullName = moduleFullName;
        } 
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the module name that has defined the value of the parameter.
        /// </summary>
        public string ModuleFullName { get; set; }
        #endregion
    }
}
