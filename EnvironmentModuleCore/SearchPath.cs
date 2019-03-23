// <copyright file="RequiredItem.cs">
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
        public static readonly string TYPE_DIRECTORY = "DIRECTORY";

        /// <summary>
        /// The constant identifies environment variable search paths.
        /// </summary>
        public static readonly string TYPE_ENVIRONMENT_VARIABLE = "ENVIRONMENT_VARIABLE";

        /// <summary>
        /// Initializes a new instance of the SearchPath class by copying the values from the passed object.
        /// </summary>
        /// <param name="other">The object that is used as data source for the copy operation.</param>
        public SearchPath(SearchPath other) : this(other.Key, other.Type, other.Priority, other.SubFolder, other.IsDefault)
        {
 
        }

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

        public SearchPathInfo ToInfo(string module)
        {
            return new SearchPathInfo(this, module);
        }

        public int CompareTo(object obj)
        {
            if (obj is SearchPath concreteObj && concreteObj.Priority > Priority)
                return 1;

            return -1;
        }

        public override bool Equals(object obj)
        {
            SearchPath other = obj as SearchPath;

            if(other == null)
            {
                return false;
            }

            return Key == other.Key && IsDefault == other.IsDefault && Type == other.Type && Priority == other.Priority && SubFolder == other.SubFolder;
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            return Key.GetHashCode() ^ IsDefault.GetHashCode() ^ Type.GetHashCode() ^ Priority.GetHashCode() ^ SubFolder.GetHashCode();
        }
    }

    public class SearchPathInfo : SearchPath
    {
        public string Module { get; set; }

        public SearchPathInfo(SearchPath baseSearchPath, string module) : base(baseSearchPath)
        {
            Module = module;
        }

        public override string ToString()
        {
            string subPath = string.IsNullOrEmpty(SubFolder) ? "" : $" \\ {SubFolder}";
            return $"{Module} -- {Type}: {Key}{subPath} (Priority: {Priority}, Default: {IsDefault})";
        }
    }
}
