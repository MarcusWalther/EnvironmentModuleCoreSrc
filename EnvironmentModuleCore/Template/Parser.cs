using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
            string endEscapeSequence = null;

            StringReader reader = new StringReader(content);

            while ((character = reader.Read()) != -1)
            {
                bool handled = false;
                char currentCharacter = (char)character;

                // Check if we are in an escaped section
                if (endEscapeSequence != null)
                {
                    if (currentCharacter == '}')
                    {
                        if (escapeSequence == null)
                        {
                            // This may be the start of the escape sequence end
                            escapeSequence = "" + currentCharacter;
                            lastCharacter = currentCharacter;
                            continue;
                        }

                        if (endEscapeSequence == escapeSequence + '}')
                        {
                            // We found the end of the escape sequence
                            lastCharacter = currentCharacter;
                            endEscapeSequence = null;
                            escapeSequence = null;
                            continue;
                        }
                        else
                        {
                            // This is something else, but not the end of the escape sequence
                            currentToken += escapeSequence;
                            escapeSequence = "" + currentCharacter;
                            lastCharacter = currentCharacter;
                            continue;
                        }
                    }

                    if (currentCharacter == '%')
                    {
                        if (escapeSequence != null)
                        {
                            escapeSequence += currentCharacter;
                            lastCharacter = currentCharacter;
                            continue;
                        }
                    }
                }

                // Detect trim command sequence for a command begin
                if(endEscapeSequence == null && currentCharacter == '-' && string.IsNullOrEmpty(currentToken) && tokens.LastOrDefault()?.TokenType == TokenType.COMMAND_BEGIN) 
                {
                    tokens.Last().Value += currentCharacter;
                    lastCharacter = currentCharacter;
                    continue;
                }
                
                // Detect command begin
                if (endEscapeSequence == null && currentCharacter == '{' && lastCharacter == '{')
                {
                    if (currentToken != "{")
                    {
                        tokens.Add(new Token(TokenType.TEXT, currentToken.TrimEnd('{')));
                    }

                    tokens.Add(new Token(TokenType.COMMAND_BEGIN));
                    handled = true;
                }

                // Detect command end
                if (endEscapeSequence == null && currentCharacter == '}' && lastCharacter == '}')
                {
                    if (!string.IsNullOrEmpty(currentToken))
                    {
                        tokens.AddRange(ParseCommand(currentToken.TrimEnd('}')));
                    }

                    var lastToken = tokens.LastOrDefault();
                    if (lastToken?.Value != null && lastToken.Value.EndsWith("-"))
                    {
                        lastToken.Value = lastToken.Value.Substring(0, lastToken.Value.Length - 1);
                        if (lastToken.Value == "")
                            tokens.RemoveAt(tokens.Count - 1);

                        tokens.Add(new Token(TokenType.COMMAND_END, "-"));
                    }
                    else
                    {
                        tokens.Add(new Token(TokenType.COMMAND_END));
                    }

                    handled = true;
                }

                // Detect escape sequences like {%{ or {%%{
                if (endEscapeSequence == null && currentCharacter == '%' && lastCharacter == '{')
                {
                    escapeSequence = "" + lastCharacter + currentCharacter;
                    lastCharacter = currentCharacter;
                    currentToken = currentToken.Substring(0, currentToken.Length - 1);
                    continue;
                }

                if (endEscapeSequence == null && escapeSequence != null)
                {
                    if(currentCharacter == '%')
                        escapeSequence += currentCharacter;
                    else if (currentCharacter == '{')
                    {
                        // Detected end of escape sequence
                        escapeSequence += currentCharacter;
                        endEscapeSequence = escapeSequence.Replace('{', '}');
                        escapeSequence = null;
                    }
                    else
                    {
                        // We found something like {%%a that is not a valid escape sequence
                        currentToken += escapeSequence + currentCharacter;
                        escapeSequence = null;
                    }

                    lastCharacter = currentCharacter;
                    continue;
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
