// <copyright file="SearchPath.cs">
//     Copyright 2019 Marcus Walther
// </copyright>
// <author>Marcus Walther</author>

// ReSharper disable NonReadonlyMemberInGetHashCode
namespace EnvironmentModuleCore
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// This class represents a search path that can be used as folder candidate. The objects of this class are serialized and stored in the configuration.
    /// </summary>
    [DataContract]
    public class SearchPath : IComparable
    {
        /// <summary>
        /// This constant identifies directory search paths.
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        public static readonly string TYPE_DIRECTORY = "DIRECTORY";

        /// <summary>
        /// The constant identifies environment variable search paths.
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        public static readonly string TYPE_ENVIRONMENT_VARIABLE = "ENVIRONMENT_VARIABLE";

        /// <summary>
        /// Initializes a new instance of the SearchPath class by copying the values from the passed object.
        /// </summary>
        /// <param name="other">The object that is used as data source for the copy operation.</param>
        public SearchPath(SearchPath other) : this(other.Key, other.Type, other.Priority, other.SubFolder, other.IsDefault)
        {
        }

        /// <summary>
        /// Initializes a new instance of the SearchPath class using the given parameters.
        /// </summary>
        /// <param name="key">The key specifying the folder, environment variable etc. to consider.</param>
        /// <param name="type">The type of the search path.</param>
        /// <param name="priority">The priority. A higher value means a higher priority.</param>
        /// <param name="subFolder">The subfolder to consider in the path that was identified by the key.</param>
        /// <param name="isDefault">True if the value is a default value specified by the description files.</param>
        public SearchPath(string key, string type, int priority, string subFolder, bool isDefault)
        {
            Key = key;
            Type = type;
            Priority = priority;
            SubFolder = subFolder;
            IsDefault = isDefault;
        }

        #region Properties
        /// <summary>
        /// Gets or sets the key (for instance directory or environment variable) of the search path.
        /// </summary>
        [DataMember]
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the search path is defined statically by the description file or by the user.
        /// </summary>
        [DataMember]
        public bool IsDefault { get; set; }

        /// <summary>
        /// Gets or sets the type identifying the search path.
        /// </summary>
        [DataMember]
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the priority of the search path. A higher number gives a higher priority.
        /// </summary>
        [DataMember]
        public int Priority { get; set; }

        /// <summary>
        /// Gets or sets the subfolder that should be considered for the search.
        /// </summary>
        [DataMember]
        public string SubFolder { get; set; } 
        #endregion

        /// <summary>
        /// Convert the given search path object to a search path info object.
        /// </summary>
        /// <param name="module">The module that uses the info object.</param>
        /// <returns>The created search path info object.</returns>
        // ReSharper disable once UnusedMember.Global
        public SearchPathInfo ToInfo(string module)
        {
            return new SearchPathInfo(this, module);
        }

        /// <summary>
        /// Compare the given object to another search path, considering the priority.
        /// </summary>
        /// <param name="obj">The object that should be used for the comparison.</param>
        /// <returns>1 if the priority of the object is higher than the priority of the argument. Otherwise -1.</returns>
        public int CompareTo(object obj)
        {
            if (obj is SearchPath concreteObj && concreteObj.Priority > Priority)
            {
                return 1;
            }

            return -1;
        }

        /// <summary>
        /// Compare the given object with this search path instance. The objects are equal if they have the same key, isDefault, type, priority and sub folder.
        /// </summary>
        /// <param name="obj">The object that should be used for the comparison.</param>
        /// <returns>True if the objects are equal.</returns>
        public override bool Equals(object obj)
        {
            SearchPath other = obj as SearchPath;

            if (other == null)
            {
                return false;
            }

            return Key == other.Key && IsDefault == other.IsDefault && Type == other.Type && Priority == other.Priority && SubFolder == other.SubFolder;
        }

        /// <summary>
        /// Calculates the hash code of the object.
        /// </summary>
        /// <returns>The created hash code.</returns>
        public override int GetHashCode()
        {
            return Key.GetHashCode() ^ IsDefault.GetHashCode() ^ Type.GetHashCode() ^ Priority.GetHashCode() ^ SubFolder.GetHashCode();
        }
    }
}
