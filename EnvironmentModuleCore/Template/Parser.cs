using System;
using System.Collections.Generic;
using System.IO;

namespace EnvironmentModuleCore.Template
{
    internal class Parser
    {
        private IEnumerable<Token> ParseCommand(string command)
        {
            var parts = command.Trim().Split(' ');
            foreach (var part in parts)
            {
                if(string.IsNullOrWhiteSpace(part))
                    continue;

                if (part.Equals("for", StringComparison.CurrentCultureIgnoreCase))
                {
                    yield return new Token(TokenType.KEYWORD_FOR);
                    continue;
                }

                if (part.Equals("in", StringComparison.CurrentCultureIgnoreCase))
                {
                    yield return new Token(TokenType.KEYWORD_IN);
                    continue;
                }

                if (part.Equals("end", StringComparison.CurrentCultureIgnoreCase))
                {
                    yield return new Token(TokenType.KEYWORD_END);
                    continue;
                }

                if (part.Equals("if", StringComparison.CurrentCultureIgnoreCase))
                {
                    yield return new Token(TokenType.KEYWORD_IF);
                    continue;
                }

                if (part.Equals("else", StringComparison.CurrentCultureIgnoreCase))
                {
                    yield return new Token(TokenType.KEYWORD_ELSE);
                    continue;
                }

                yield return new Token(TokenType.PARAMETER, part);
            }
        }

        public Template Parse(string content)
        {
            List<Token> tokens = new List<Token>();
            if (string.IsNullOrEmpty(content))
                return new Template(tokens);

            char lastCharacter = '\0';
            int character;
            string currentToken = string.Empty;
            string escapeSequence = null;

            StringReader reader = new StringReader(content);

            while ((character = reader.Read()) != -1)
            {
                bool handled = false;

                char currentCharacter = (char)character;
                if (currentCharacter == '{' && lastCharacter == '{')
                {
                    if (currentToken != "{")
                    {
                        tokens.Add(new Token(TokenType.TEXT, currentToken.TrimEnd('{')));
                    }

                    tokens.Add(new Token(TokenType.COMMAND_BEGIN));
                    handled = true;
                }

                // Detect escape sequences like {%{ or {%%{
                if (currentCharacter == '%' && lastCharacter == '{')
                {
                    escapeSequence = "{%";
                    handled = true;
                }

                // Detect escape sequences like {%{ or {%%{
                if (escapeSequence != null)
                {
                    if(currentCharacter == '%')
                        escapeSequence += '%';
                    else if (currentCharacter == '{')
                        escapeSequence += "{";
                    else
                        escapeSequence = null;
                    handled = true;
                }

                if (currentCharacter == '}' && lastCharacter == '}')
                {
                    if (!string.IsNullOrEmpty(currentToken))
                    {
                        tokens.AddRange(ParseCommand(currentToken.TrimEnd('}')));
                    }

                    tokens.Add(new Token(TokenType.COMMAND_END));
                    handled = true;
                }

                if (!handled)
                {
                    currentToken += currentCharacter;
                }
                else
                {
                    currentToken = string.Empty;
                }

                lastCharacter = currentCharacter;
            }

            if (!string.IsNullOrEmpty(currentToken))
            {
                tokens.Add(new Token(TokenType.TEXT, currentToken.TrimEnd('{')));
            }

            return new Template(tokens);
        }
    }
}
