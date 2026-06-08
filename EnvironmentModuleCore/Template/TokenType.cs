namespace EnvironmentModuleCore.Template
{
    /// <summary>
    /// An enumeration defining the different token types that can be found in a template file.
    /// </summary>
    public enum TokenType
    {
        /// <summary>
        /// A constant text.
        /// </summary>
        Text,
        /// <summary>
        /// The begin of a command "{{".
        /// </summary>
        CommandBegin,
        /// <summary>
        /// The end of a command "}}".
        /// </summary>
        CommandEnd,
        /// <summary>
        /// The statement "for".
        /// </summary>
        KeywordFor,
        /// <summary>
        /// The keyword "in".
        /// </summary>
        KeywordIn,
        KeywordEnd,
        KeywordIf,
        KeywordElse,
        /// <summary>
        /// The and keyword "&&".
        /// </summary>
        KeywordAnd,
        /// <summary>
        /// The or keyword "||".
        /// </summary>
        KeywordOr,
        /// <summary>
        /// A parameter like "var".
        /// </summary>
        Parameter,
        /// <summary>
        /// A comparator like "==" or "!=".
        /// </summary>
        Comparator,
        /// <summary>
        /// A constant string like "test".
        /// </summary>
        ConstString,
        /// <summary>
        /// A constant number like "4.6".
        /// </summary>
        ConstNumber,
        /// <summary>
        /// The keyword "null".
        /// </summary>
        KeywordNull
    }
}
