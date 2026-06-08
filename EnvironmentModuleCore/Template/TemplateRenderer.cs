using System;
using System.Collections;
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

        private object GetVariable(string fullName, object model, Dictionary<string, object> localVariables, bool mandatory = true)
        {
            foreach (var name in fullName.Split('.'))
            {
                if (model == null)
                    return null;

                if (localVariables != null)
                {
                    var result = GetVariable(name, localVariables, false);
                    if (result != null)
                    {
                        model = result;
                        continue;
                    }
                }

                if (model == null)
                    throw new ArgumentException($"The parameter '{name}' was not found");

                // Handle dictionary models directly
                if (model is Dictionary<string, object> dictionaryModel)
                {
                    model = GetVariable(name, dictionaryModel, mandatory);
                    continue;
                }

                // Handle expando objects by treating them as dictionaries
                if (model is ExpandoObject expandoModel)
                {
                    model = GetVariable(name, expandoModel, mandatory);
                    continue;
                }

                var member = model.GetType().GetProperty(name);
                if (member == null)
                    throw new ArgumentException($"The parameter '{name}' was not found");

                model = member.GetValue(model);
            }

            return model;
        }

        public bool Compare(Token left, Token comparatorToken, Token right, object model, Dictionary<string, object> localVariables)
        {
            object leftValue = left.Value;
            object rightValue = right.Value;

            if (left.TokenType == TokenType.Parameter)
            {
                leftValue = GetVariable(left.Value, model, localVariables);
            }

            if (right.TokenType == TokenType.Parameter)
            {
                rightValue = GetVariable(right.Value, model, localVariables);
            }

            switch (comparatorToken.Value)
            {
                case "==":
                    return leftValue == rightValue;
                case "!=":
                    return leftValue != rightValue;
                case ">":
                    return String.Compare(leftValue.ToString(), rightValue.ToString(), StringComparison.Ordinal) > 0;
                case "<":
                    return String.Compare(leftValue.ToString(), rightValue.ToString(), StringComparison.Ordinal) < 0;
                case ">=":
                    return String.Compare(leftValue.ToString(), rightValue.ToString(), StringComparison.Ordinal) >= 0;
                case "<=":
                    return String.Compare(leftValue.ToString(), rightValue.ToString(), StringComparison.Ordinal) <= 0;
                default:
                    throw new InvalidOperationException($"Unsupported comparator '{comparatorToken.Value}' in condition");
            }
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
                    case TokenType.KeywordFor:
                        depth++;
                        break;
                    case TokenType.KeywordEnd:
                        depth--;
                        if(depth < 0)
                            return;
                        break;
                    case TokenType.KeywordIf:
                        depth++;
                        break;
                    case TokenType.KeywordElse:
                        if (depth <= 0)
                            return;
                        break;
                    case TokenType.KeywordIn:
                    case TokenType.Text:
                    case TokenType.Parameter:
                    case TokenType.CommandBegin:
                    case TokenType.CommandEnd:
                        break;
                    default:
                        throw new InvalidOperationException($"Unexpected token type: {enumerator.Current.TokenType}");
                }
            }
        }

        private (List<Token>, Token, List<Token>) SplitTokenStream(List<Token> conditionTokens)
        {
            int tokenPosition = 0;
            int tokenPriority = 0;

            int i = 0;
            foreach (var token in conditionTokens)
            {
                if (token.TokenType == TokenType.KeywordOr && tokenPriority == 0)
                {
                    tokenPriority = 1;
                    tokenPosition = i;
                    continue;
                }

                if (token.TokenType == TokenType.KeywordAnd)
                {
                    tokenPosition = i;
                    break; // There is not a higher priority token than 'and', so we can stop the search
                }

                i++;
            }

            return (conditionTokens.GetRange(0, tokenPosition), conditionTokens[tokenPosition], conditionTokens.GetRange(tokenPosition + 1, conditionTokens.Count - tokenPosition - 1));
        }

        private bool EvaluateCondition(List<Token> conditionTokens, object model, Dictionary<string, object> localVariables)
        {
            if (conditionTokens.Count == 0)
                return false;
            
            if (conditionTokens.Count == 1)
            {
                var firstToken = conditionTokens[0];
                if (firstToken.TokenType != TokenType.Parameter)
                    throw new InvalidOperationException($"Unable to evaluate the parameter {firstToken.Value} to bool");

                var value = GetVariable(firstToken.Value, model, localVariables);
                // A value that is null
                if (value == null)
                    return false;

                // A boolean value that is false
                if (value is bool boolValue && !boolValue)
                    return false;

                // Some value that is not null
                return true;
            }

            if (conditionTokens.Count == 3)
            {
                var firstToken = conditionTokens[0];
                var comparatorToken = conditionTokens[1];
                var secondToken = conditionTokens[2];

                if (comparatorToken.TokenType != TokenType.Comparator)
                {
                    throw new InvalidOperationException($"Unable to evaluate the condition {string.Join(" ", conditionTokens)} to bool");
                }

                return Compare(firstToken, comparatorToken, secondToken, model, localVariables);
            }

            var (leftTokens, combinationToken, rightTokens) = SplitTokenStream(conditionTokens);

            switch (combinationToken.TokenType)
            {
                case TokenType.KeywordAnd:
                    return EvaluateCondition(leftTokens, model, localVariables) && EvaluateCondition(rightTokens, model, localVariables);
                case TokenType.KeywordOr:
                    return EvaluateCondition(leftTokens, model, localVariables) || EvaluateCondition(rightTokens, model, localVariables);
                default:
                    throw new InvalidOperationException($"Unexpected token type '{combinationToken.TokenType}' in condition");
            }
        }

        private string HandleIfExpression(ref List<Token>.Enumerator enumerator, object model, Dictionary<string, object> localVariables, int depth)
        {
            // Check the expression
            if (!enumerator.MoveNext() || enumerator.Current?.TokenType != TokenType.Parameter)
                throw new InvalidOperationException("Expected a parameter after 'if' keyword");

            List<Token> condition = new List<Token>();
            while (enumerator.Current.TokenType != TokenType.CommandEnd)
            {
                condition.Add(enumerator.Current);
                if (!enumerator.MoveNext())
                    break;
            }

            bool conditionResult = EvaluateCondition(condition, model, localVariables);
            if (!conditionResult)
            {
                MoveToEnd(ref enumerator);

                if (enumerator.Current?.TokenType == TokenType.KeywordElse)
                {
                    enumerator.MoveNext();
                    return Render(ref enumerator, model, depth + 1, localVariables);
                }

                return string.Empty;
            }

            string result = Render(ref enumerator, model, depth + 1, localVariables);
            if(enumerator.Current?.TokenType == TokenType.KeywordElse)
                MoveToEnd(ref enumerator);

            return result;
        }

        private string HandleForExpression(ref List<Token>.Enumerator enumerator, object model, Dictionary<string, object> localVariables, int depth)
        {
            // Check the expression
            if (!enumerator.MoveNext() || enumerator.Current?.TokenType != TokenType.Parameter)
                throw new InvalidOperationException("Expected a parameter after 'if' keyword");

            if (enumerator.Current?.TokenType != TokenType.Parameter) 
                throw new InvalidOperationException("Expected a parameter name at the begin of the for-loop");


            var parameterName = enumerator.Current.Value;

            if (!enumerator.MoveNext() || enumerator.Current?.TokenType != TokenType.KeywordIn)
                throw new InvalidOperationException("Expected the 'in' keyword in the for-loop");

            if (!enumerator.MoveNext() || enumerator.Current?.TokenType != TokenType.Parameter)
                throw new InvalidOperationException("Expected a parameter after the 'in' keyword of the for-loop");

            var collection = enumerator.Current.Value;

            if (!enumerator.MoveNext())
                throw new InvalidOperationException("Expected a body of the for-loop");

            var enumerable = GetVariable(collection, model, localVariables, false) as IEnumerable;

            if (enumerable == null)
            {
                
                MoveToEnd(ref enumerator);
                return "";
            }

            string result = string.Empty;
            var items = enumerable.Cast<object>().ToList();
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

                Dictionary<string, object> forInfo = new Dictionary<string, object>
                {
                    {"index", i},
                    {"rindex", length - i - 1},
                    {"first", i == 0},
                    {"last", i == length - 1},
                    {"even", i % 2 == 0},
                    {"odd", i % 2 == 1}
                };

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
            bool trimStart = false;

            while (enumerator.MoveNext())
            {
                if (enumerator.Current == null)
                    break;

                string tmpResult;
                switch (enumerator.Current.TokenType)
                {
                    case TokenType.Text:
                        tmpResult = enumerator.Current.Value;
                        if (trimStart)
                            tmpResult = tmpResult.TrimStart();

                        trimStart = false;
                        result += tmpResult;
                        break;
                    case TokenType.CommandBegin:
                        if (enumerator.Current.Value == "-")
                        {
                            result = result.TrimEnd();
                        }
                        break;
                    case TokenType.CommandEnd:
                        trimStart = enumerator.Current.Value == "-";
                        break;
                    case TokenType.Parameter:
                        tmpResult = VariableValueToText(GetVariable(enumerator.Current.Value, model, localVariables));

                        if (trimStart)
                            tmpResult = tmpResult.TrimStart();

                        trimStart = false;
                        result += tmpResult;
                        break;
                    case TokenType.KeywordIf:
                        tmpResult = HandleIfExpression(ref enumerator, model, localVariables, depth);

                        if (trimStart)
                            tmpResult = tmpResult.TrimStart();

                        trimStart = false;
                        result += tmpResult;
                        break;
                    case TokenType.KeywordFor:
                        tmpResult = HandleForExpression(ref enumerator, model, localVariables, depth);

                        if (trimStart)
                            tmpResult = tmpResult.TrimStart();

                        trimStart = false;
                        result += tmpResult;
                        break;
                    case TokenType.KeywordEnd:
                        if (depth == 0)
                            throw new InvalidOperationException("Unexpected 'end' keyword without a matching 'if' or 'for'");
                        return result;
                    case TokenType.KeywordElse:
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
