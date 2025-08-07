// <copyright file="ParameterInfo.cs">
//     Copyright 2019 Marcus Walther
// </copyright>
// <author>Marcus Walther</author>

using System.Runtime.Serialization;

namespace EnvironmentModuleCore
{
    /// <summary>
    /// A helper class that is used to identify the concrete version of a module.
    /// </summary>
    [DataContract]
    public class VersionInfo
    {
        /// <summary>
        /// The type specifier that is used if the version is read from a dll or exe file (Windows).
        /// </summary>
        public static readonly string TYPE_FILE_VERSION = "FILE_VERSION";

        /// <summary>
        /// The type specifier that is used if the version is read from a text file.
        /// </summary>
        public static readonly string TYPE_REGEX_FILE_VERSION = "REGEX_FILE_VERSION";

        /// <summary>
        /// The type specifier that is used if the version is parsed from a file name.
        /// </summary>
        public static readonly string TYPE_REGEX_FILE_NAME_VERSION = "REGEX_FILE_NAME_VERSION";

        /// <summary>
        /// The type specifier that is used if the version is specified as constant value.
        /// </summary>
        public static readonly string TYPE_CONSTANT_VERSION = "CONSTANT_VERSION";

        /// <summary>
        /// Gets or sets the type of the version information object.
        /// </summary>
        [DataMember]
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the file that contains the version information. The path may contain placeholders.
        /// </summary>
        [DataMember]
        public string File { get; set; }

        /// <summary>
        /// Gets or sets the value of the version information object. The value depends on the specified type.
        /// </summary>
        [DataMember]
        public string Value { get; set; }

        public VersionInfo(string type, string file = null, string value = null)
        {
            Type = type;
            File = file;
            Value = value;
        }
    }
}
