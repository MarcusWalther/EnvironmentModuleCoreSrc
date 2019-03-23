// <copyright file="SearchPathInfo.cs">
//     Copyright 2019 Marcus Walther
// </copyright>
// <author>Marcus Walther</author>

namespace EnvironmentModuleCore
{
    /// <summary>
    /// This class represents a search path object that is uses by a concrete environment module.
    /// </summary>
    public class SearchPathInfo : SearchPath
    {
        /// <summary>
        /// Initializes a new instance of the SearchPathInfo class with the given parameters.
        /// </summary>
        /// <param name="baseSearchPath">The base parameter to use.</param>
        /// <param name="module">The module that uses the search path.</param>
        public SearchPathInfo(SearchPath baseSearchPath, string module) : base(baseSearchPath)
        {
            Module = module;
        }

        /// <summary>
        /// Gets or sets the module that uses the search path.
        /// </summary>
        public string Module { get; set; }

        /// <summary>
        /// Convert the object to a human readable string.
        /// </summary>
        /// <returns>The human readable string.</returns>
        public override string ToString()
        {
            string subPath = string.IsNullOrEmpty(SubFolder) ? string.Empty : $" \\ {SubFolder}";
            return $"{Module} -- {Type}: {Key}{subPath} (Priority: {Priority}, Default: {IsDefault})";
        }
    }
}
