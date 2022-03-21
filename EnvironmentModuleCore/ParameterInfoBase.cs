// <copyright file="ParameterInfo.cs">
//     Copyright 2019 Marcus Walther
// </copyright>
// <author>Marcus Walther</author>

namespace EnvironmentModuleCore
{
    /// <summary>
    /// This class represents a parameter that is provided by an environment module.
    /// </summary>
    // ReSharper disable once UnusedMember.Global
    public class ParameterInfoBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the ParameterInfo class with the given parameters.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="value">The value of the parameter.</param>
        /// <param name="isUserDefined">True if the value was defined manually by the user.</param>
        /// <param name="virtualEnvironment">The name of the virtual environment the parameter belongs to. Default is null.</param>
        public ParameterInfoBase(string name, string value, bool isUserDefined, string virtualEnvironment)
        {
            Name = name;
            Value = value;
            IsUserDefined = isUserDefined;
            VirtualEnvironment = virtualEnvironment;
        } 
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the unique name of the parameter.
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// Gets or sets the virtual environment of the parameter.
        /// </summary>
        public string VirtualEnvironment { get; protected set; }

        /// <summary>
        /// Gets or sets the value of the parameter.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the values was defined manually by the user or not.
        /// </summary>
        public bool IsUserDefined { get; set; }
        #endregion
    }
}
