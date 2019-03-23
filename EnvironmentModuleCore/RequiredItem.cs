// <copyright file="RequiredItem.cs">
//     Copyright 2019 Marcus Walther
// </copyright>
// <author>Marcus Walther</author>

namespace EnvironmentModuleCore
{
    /// <summary>
    /// This class represents an item (for instance a file) that is required by the module.
    /// </summary>
    public class RequiredItem
    {
        /// <summary>
        /// The constant type representing a file that is required.
        /// </summary>
        public static readonly string TYPE_FILE = "FILE";

        /// <summary>
        /// Initializes a new instance of the RequiredItem class with the given parameters.
        /// </summary>
        /// <param name="itemType">The type of the item that is required.</param>
        /// <param name="value">The value of the item that is required.</param>
        public RequiredItem(string itemType, string value)
        {
            ItemType = itemType;
            Value = value;
        }

        #region Properties
        /// <summary>
        /// Gets or sets the item type value.
        /// </summary>
        public string ItemType { get; set; }

        /// <summary>
        /// Gets or sets the value specifying the required item (for instance a file path).
        /// </summary>
        public string Value { get; set; }
        #endregion
    }
}
