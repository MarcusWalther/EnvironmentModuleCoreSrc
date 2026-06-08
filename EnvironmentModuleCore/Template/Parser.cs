using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace EnvironmentModuleCore.Template
{
    internal class Parser
    {
        private static readonly Regex NUMBER_REGEX= new Regex(@"^-?\d+(\.\d+)?$");
        private static readonly Regex STRING_REGEX = new Regex("^\".*\"$");
        private static readonly HashSet<string> COMPARATORS = new HashSet<string>{ "!=", "==", "<", ">", "<=", ">=" };

        internal string[] SplitCommand(string command)
        {
            HashSet<char> comparatorChars = new HashSet<char> { '!', '=', '>', '<' };

            StringReader reader = new StringReader(command);

            int character;
            char lastCharacter = '\0';
            string current = string.Empty;
            bool isString = false;
            List<string> result = new List<string>();

            while ((character = reader.Read()) != -1)
            {
                char currentCharacter = (char)character;

                if (currentCharacter == '"')
                {
                    if (isString)
                    {
                        // We found the end of the string
                        current += currentCharacter;
                        result.Add(current);
                        current = string.Empty;
                        isString = false;
                    }
                    else
                    {
                        // We found a string start
                        if (current != string.Empty)
                        {
                            result.Add(current);
                        }

                        current = string.Empty + currentCharacter;
                        isString = true;
                    }

                    lastCharacter = currentCharacter;
                    continue;
                }

                if (isString)
                {
                    current += currentCharacter;
                    lastCharacter = currentCharacter;
                    continue;
                }

                if (currentCharacter == ' ')
                {
                    if (current != string.Empty)
                    {
                        result.Add(current);
                    }

                    current = string.Empty;
                    lastCharacter = currentCharacter;
                    continue;
                }

                if (comparatorChars.Contains(currentCharacter))
                {
                    if (current != string.Empty)
                    {
                        if (comparatorChars.Contains(lastCharacter))
                        {
                            current += currentCharacter;
                        }
                        else
                        {
                            result.Add(current);
                            current = "" + currentCharacter;
                        }
                    }
                    else
                    {
                        current += currentCharacter;
                    }

                    lastCharacter = currentCharacter;
                    continue;
                }

                current += currentCharacter;
                lastCharacter = currentCharacter;
            }

            if(current != string.Empty)
                result.Add(current);

            return result.ToArray();
        }

        internal IEnumerable<Token> ParseCommand(string command)
        {
            var parts = SplitCommand(command.Trim());

            foreach (var part in parts)
            {
                if(string.IsNullOrWhiteSpace(part))
                    continue;

                if (part.Equals("for", StringComparison.CurrentCultureIgnoreCase))
                {
                    yield return new Token(TokenType.KeywordFor);
                    continue;
                }

                if (part.Equals("in", StringComparison.CurrentCultureIgnoreCase))
                {
                    yield return new Token(TokenType.KeywordIn);
                    continue;
                }

                if (part.Equals("end", StringComparison.CurrentCultureIgnoreCase))
                {
                    yield return new Token(TokenType.KeywordEnd);
                    continue;
                }

                if (part.Equals("if", StringComparison.CurrentCultureIgnoreCase))
                {
                    yield return new Token(TokenType.KeywordIf);
                    continue;
                }

                if (part.Equals("else", StringComparison.CurrentCultureIgnoreCase))
                {
                    yield return new Token(TokenType.KeywordElse);
                    continue;
                }

                if (part.Equals("&&", StringComparison.CurrentCultureIgnoreCase))
                {
                    yield return new Token(TokenType.KeywordAnd);
                    continue;
                }

                if (part.Equals("||", StringComparison.CurrentCultureIgnoreCase))
                {
                    yield return new Token(TokenType.KeywordOr);
                    continue;
                }

                if (part.Equals("null", StringComparison.CurrentCultureIgnoreCase))
                {
                    yield return new Token(TokenType.KeywordNull);
                    continue;
                }

                if (COMPARATORS.Contains(part))
                {
                    yield return new Token(TokenType.Comparator, part);
                    continue;
                }

                if (NUMBER_REGEX.IsMatch(part))
                {
                    yield return new Token(TokenType.ConstNumber, part);
                    continue;
                }

                if (STRING_REGEX.IsMatch(part))
                {
                    yield return new Token(TokenType.ConstString, part.Substring(1, part.Length - 2));
                    continue;
                }

                yield return new Token(TokenType.Parameter, part);
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

                        // This is something else, but not the end of the escape sequence
                        currentToken += escapeSequence;
                        escapeSequence = "" + currentCharacter;
                        lastCharacter = currentCharacter;
                        continue;
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
                if(endEscapeSequence == null && currentCharacter == '-' && string.IsNullOrEmpty(currentToken) && tokens.LastOrDefault()?.TokenType == TokenType.CommandBegin) 
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
                        tokens.Add(new Token(TokenType.Text, currentToken.TrimEnd('{')));
                    }

                    tokens.Add(new Token(TokenType.CommandBegin));
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

                        tokens.Add(new Token(TokenType.CommandEnd, "-"));
                    }
                    else
                    {
                        tokens.Add(new Token(TokenType.CommandEnd));
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
                tokens.Add(new Token(TokenType.Text, currentToken.TrimEnd('{')));
            }

            return new Template(tokens);
        }
    }
}
