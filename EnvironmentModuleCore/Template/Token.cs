using System.Diagnostics;

namespace EnvironmentModuleCore.Template
{
    [DebuggerDisplay("{TokenType} - {Value}")]
    public class Token
    {
        public TokenType TokenType { get; }

        public string Value { get; set; }

        public Token(TokenType tokenType, string value = null)
        {
            TokenType = tokenType;
            Value = value;
        }
    }
}
