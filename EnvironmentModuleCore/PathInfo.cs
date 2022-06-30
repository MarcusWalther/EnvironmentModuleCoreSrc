// <copyright file="PathInfo.cs">
//     Copyright 2019 Marcus Walther
// </copyright>
// <author>Marcus Walther</author>

using System;
using System.IO;
using System.Linq;

namespace EnvironmentModuleCore
{
    using System.Collections.Generic;

    /// <summary>
    /// This enumeration types are valid for environment variable modifications.
    /// </summary>
    public enum PathType
    {
        /// <summary>
        /// An unknown path type.
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        UNKNOWN,

        /// <summary>
        /// An environment variable that is appended to the already existing value.
        /// </summary>
        APPEND,

        /// <summary>
        /// An environment variable that is prepended to the already existing value.
        /// </summary>
        PREPEND,

        /// <summary>
        /// An environment variable that overwrites the already existing value.
        /// </summary>
        SET
    }

    /// <summary>
    /// The event class used if an previously defined path was updated.
    /// </summary>
    public class PathUpdateEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the values that were previously assigned to the path info.
        /// </summary>
        public List<string> OldValues { get; }

        public PathUpdateEventArgs(List<string> oldValues)
        {
            OldValues = oldValues;
        }
    }

    /// <summary>
    /// This class represents a environment variable modification performed by an environment module.
    /// </summary>
    public class PathInfo
    {
        public delegate void PathUpdateHandler(PathInfo sender, PathUpdateEventArgs e);
        public event PathUpdateHandler OnValueChanged;

        /// <summary>
        /// Initializes a new instance of the PathInfo class with the given parameters.
        /// </summary>
        /// <param name="moduleFullName">The environment module that performs the environment modification.</param>
        /// <param name="pathType">The type of the path modification.</param>
        /// <param name="variable">The environment variable to modify.</param>
        /// <param name="values">The new values to handle.</param>
        /// <param name="key">A unique key that can be used to identify and change the value during runtime.</param>
        public PathInfo(string moduleFullName, PathType pathType, string variable, List<string> values = null, string key = null)
        {
            ModuleFullName = moduleFullName;
            PathType = pathType;
            Variable = variable;

            if (values == null)
            {
                values = new List<string>();
            }

            Values = values;
            Key = key;
        }

        #region Properties
        /// <summary>
        /// Gets or sets the module name that performs the environment modification.
        /// </summary>
        public string ModuleFullName { get; protected set; }

        /// <summary>
        /// Gets the path type that specifies the kind of modification.
        /// </summary>
        public PathType PathType { get; }

        /// <summary>
        /// Gets the environment variable that is modified.
        /// </summary>
        public string Variable { get; }

        /// <summary>
        /// Gets the optional key of the object. The key can be used to identify and change the value dynamically.
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// Gets or sets the values that are used by the modification.
        /// </summary>
        public List<string> Values { get; internal set; }
        #endregion

        /// <summary>
        /// Compare the path info with another. Two path infos are equal, if the have the same type and modify the same variable.
        /// </summary>
        /// <param name="obj">The object to compare with this object.</param>
        /// <returns>True if the objects are equal.</returns>
        public override bool Equals(object obj)
        {
            PathInfo other = obj as PathInfo;

            if (other == null)
            {
                return false;
            }

            return other.Variable == Variable && other.PathType == PathType && other.Key == Key;
        }

        /// <summary>
        /// Get the hash code identifying this object.
        /// </summary>
        /// <returns>The created hash code.</returns>
        public override int GetHashCode()
        {
            int result = PathType.GetHashCode() ^ Variable.GetHashCode();
            if (!string.IsNullOrEmpty(Key))
                result ^= Key.GetHashCode();

            return result;
        }

        /// <summary>
        /// Change the values of the path and invoke the OnValuesChanged event.
        /// </summary>
        /// <param name="newValues">The new path values to use.</param>
        public void ChangeValues(IEnumerable<string> newValues)
        {
            ChangeValues(newValues.ToArray());
        }

        /// <summary>
        /// Change the values of the path and invoke the OnValuesChanged event.
        /// </summary>
        /// <param name="newValue">The new path value to use.</param>
        public void ChangeValues(string newValue)
        {
            ChangeValues(new[] { newValue });
        }

        /// <summary>
        /// Change the values of the path and invoke the OnValuesChanged event.
        /// </summary>
        /// <param name="newValues">The new path values to use.</param>
        public void ChangeValues(string[] newValues)
        {
            List<string> valueCopy = new List<string>(Values);
            Values.Clear();
            Values.AddRange(newValues);
            OnValueChanged?.Invoke(this, new PathUpdateEventArgs(valueCopy));
        }

        public override string ToString()
        {
            string identifier = Variable;
            if (!string.IsNullOrEmpty(Key))
                identifier += $" ({Key})";

            return $"{identifier} = {string.Join(Path.PathSeparator.ToString(), Values)} [{PathType}] ({ModuleFullName})";
        }
    }
}
