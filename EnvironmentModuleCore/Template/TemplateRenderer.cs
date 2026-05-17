using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace EnvironmentModuleCore.Template
{
    internal class TemplateRenderer
    {
        private object GetVariable(string name, IDictionary<string, object> dictionaryModel, bool mandatory = true)
        {
            string variable = name;
            string subPath = null;
            int dotIndex = name.IndexOf('.');

            if (dotIndex > 0)
            {
                variable = name.Substring(0, dotIndex);
                subPath = name.Substring(dotIndex + 1);
            }

            if (dictionaryModel.TryGetValue(variable, out var dictResult))
            {
                if (subPath != null)
                    return GetVariable(subPath, dictResult, null);

                return dictResult;
            }

            if(mandatory)
                throw new ArgumentException($"The parameter '{name}' was not found");

            return null;
        }

        private object GetVariable(string name, object model, Dictionary<string, object> localVariables)
        {
            if (localVariables != null)
            {
                object result = GetVariable(name, localVariables, false);
                if (result != null)
                    return result;
            }

            if (model == null)
                throw new ArgumentException($"The parameter '{name}' was not found");

            // Handle dictionary models directly
            if (model is Dictionary<string, object> dictionaryModel)
            {
                return GetVariable(name, dictionaryModel);
            }

            // Handle expando objects by treating them as dictionaries
            if (model is ExpandoObject expandoModel)
            {
                return GetVariable(name, expandoModel);
            }

            var member = model.GetType().GetProperty(name);
            if (member == null)
                throw new ArgumentException($"The parameter '{name}' was not found");

            return member.GetValue(model);
        }

        private void MoveToEnd(ref List<Token>.Enumerator enumerator)
        {
            int depth = 0;
            while (enumerator.MoveNext())
            {
                if (enumerator.Current == null)
                    break;

                switch (enumerator.Current.TokenType)
                {
                    case TokenType.KEYWORD_FOR:
                        depth++;
                        break;
                    case TokenType.KEYWORD_END:
                        depth--;
                        if(depth < 0)
                            return;
                        break;
                    case TokenType.KEYWORD_IF:
                        depth++;
                        break;
                    case TokenType.KEYWORD_ELSE:
                        if (depth <= 0)
                            return;
                        break;
                    case TokenType.KEYWORD_IN:
                    case TokenType.TEXT:
                    case TokenType.PARAMETER:
                    case TokenType.COMMAND_BEGIN:
                    case TokenType.COMMAND_END:
                        break;
                    default:
                        throw new InvalidOperationException($"Unexpected token type: {enumerator.Current.TokenType}");
                }
            }
        }

        private string HandleIfExpression(ref List<Token>.Enumerator enumerator, object model, Dictionary<string, object> localVariables, int depth)
        {
            // Check the expression
            if (!enumerator.MoveNext() || enumerator.Current?.TokenType != TokenType.PARAMETER)
                throw new InvalidOperationException("Expected a parameter after 'if' keyword");

            var value = GetVariable(enumerator.Current.Value, model, localVariables);
            if (value == null || (value is bool boolValue && !boolValue))
            {
                MoveToEnd(ref enumerator);

                if (enumerator.Current?.TokenType == TokenType.KEYWORD_ELSE)
                {
                    enumerator.MoveNext();
                    return Render(ref enumerator, model, depth + 1, localVariables);
                }

                return string.Empty;
            }

            string result = Render(ref enumerator, model, depth + 1, localVariables);
            if(enumerator.Current?.TokenType == TokenType.KEYWORD_ELSE)
                MoveToEnd(ref enumerator);

            return result;
        }

        private string HandleForExpression(ref List<Token>.Enumerator enumerator, object model, Dictionary<string, object> localVariables, int depth)
        {
            // Check the expression
            if (!enumerator.MoveNext() || enumerator.Current?.TokenType != TokenType.PARAMETER)
                throw new InvalidOperationException("Expected a parameter after 'if' keyword");

            if (enumerator.Current?.TokenType != TokenType.PARAMETER) 
                throw new InvalidOperationException("Expected a parameter name at the begin of the for-loop");


            var parameterName = enumerator.Current.Value;

            if (!enumerator.MoveNext() || enumerator.Current?.TokenType != TokenType.KEYWORD_IN)
                throw new InvalidOperationException("Expected the 'in' keyword in the for-loop");

            if (!enumerator.MoveNext() || enumerator.Current?.TokenType != TokenType.PARAMETER)
                throw new InvalidOperationException("Expected a parameter after the 'in' keyword of the for-loop");

            var collection = enumerator.Current.Value;

            if (!enumerator.MoveNext())
                throw new InvalidOperationException("Expected a body of the for-loop");

            var enumerable = GetVariable(collection, model, localVariables) as IEnumerable<object>;

            if(enumerable == null)
                throw new InvalidOperationException($"The parameter '{collection}' is not enumerable");

            string result = string.Empty;
            var items = enumerable.ToList();
            int length = items.Count;
            int i = 0;

            // Store the old variables
            Dictionary<string, object> stack = new Dictionary<string, object>();
            string[] variablesToStore = { parameterName, "for" };

            foreach (var variable in variablesToStore)
            {
                if(localVariables.TryGetValue(variable, out var value))
                    stack.Add(variable, value);
            }

            foreach (var item in items)
            {
                var start = enumerator;
                localVariables[parameterName] = item;

                dynamic forInfo = new ExpandoObject();
                forInfo.index = i;
                forInfo.rindex = length - i - 1;
                forInfo.first = i == 0;
                forInfo.last = i == length - 1;
                forInfo.even = i % 2 == 0;
                forInfo.odd = i % 2 == 1;
                localVariables["for"] = forInfo;

                result += Render(ref start, model, depth + 1, localVariables);
                i++;
            }

            foreach (var variable in variablesToStore)
            {
                if (stack.TryGetValue(variable, out var oldValue))
                    localVariables[variable] = oldValue;
                else
                    localVariables.Remove(variable);
            }

            MoveToEnd(ref enumerator);

            return result;
        }

        private string VariableValueToText(object value)
        {
            if(value == null)
                return string.Empty;

            if (value is bool bValue)
            {
                // For compliance to Scriban, we convert boolean values to "true" or "false" in lowercase
                return bValue.ToString().ToLower();
            }

            return value.ToString();
        }

        private string Render(ref List<Token>.Enumerator enumerator, object model, int depth, Dictionary<string, object> localVariables)
        {
            string result = string.Empty;
            string tmpResult = string.Empty;
            bool trimStart = false;

            while (enumerator.MoveNext())
            {
                if (enumerator.Current == null)
                    break;

                switch (enumerator.Current.TokenType)
                {
                    case TokenType.TEXT:
                        tmpResult = enumerator.Current.Value;
                        if (trimStart)
                            tmpResult = tmpResult.TrimStart();

                        trimStart = false;
                        result += tmpResult;
                        break;
                    case TokenType.COMMAND_BEGIN:
                        if (enumerator.Current.Value == "-")
                        {
                            result = result.TrimEnd();
                        }
                        break;
                    case TokenType.COMMAND_END:
                        trimStart = enumerator.Current.Value == "-";
                        break;
                    case TokenType.PARAMETER:
                        tmpResult = VariableValueToText(GetVariable(enumerator.Current.Value, model, localVariables));

                        if (trimStart)
                            tmpResult = tmpResult.TrimStart();

                        trimStart = false;
                        result += tmpResult;
                        break;
                    case TokenType.KEYWORD_IF:
                        tmpResult = HandleIfExpression(ref enumerator, model, localVariables, depth);

                        if (trimStart)
                            tmpResult = tmpResult.TrimStart();

                        trimStart = false;
                        result += tmpResult;
                        break;
                    case TokenType.KEYWORD_FOR:
                        tmpResult = HandleForExpression(ref enumerator, model, localVariables, depth);

                        if (trimStart)
                            tmpResult = tmpResult.TrimStart();

                        trimStart = false;
                        result += tmpResult;
                        break;
                    case TokenType.KEYWORD_END:
                        if (depth == 0)
                            throw new InvalidOperationException("Unexpected 'end' keyword without a matching 'if' or 'for'");
                        return result;
                    case TokenType.KEYWORD_ELSE:
                        if (depth == 0)
                            throw new InvalidOperationException("Unexpected 'else' keyword without a matching 'if'");
                        return result;
                    default:
                        throw new InvalidOperationException($"Unexpected token type: {enumerator.Current.TokenType}");
                }
            }

            return result;
        }

        public string Render(Template template, object model = null)
        {
            var enumerator = template.Tokens.GetEnumerator();
            return Render(ref enumerator, model, 0, new Dictionary<string, object>());
        }
    }
}
