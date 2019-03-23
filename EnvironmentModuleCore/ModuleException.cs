// <copyright file="ModuleException.cs">
//     Copyright 2019 Marcus Walther
// </copyright>
// <author>Marcus Walther</author>

namespace EnvironmentModuleCore
{
    using System;

    /// <summary>
    /// This exception class can be thrown by various functions of the library.
    /// </summary>
    public class ModuleException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the ModuleException class.
        /// </summary>
        /// <param name="message">The message body of the exception.</param>
        public ModuleException(string message) : base(message)
        {
        }
    }
}
