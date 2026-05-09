namespace rec_file_lib.Query;

internal static class RecSelectionExpressionParser
{
    public static RecSelectionExpression? Parse(string? expression)
    {
        if (string.IsNullOrWhiteSpace(expression))
        {
            return null;
        }

        var parser = new Parser(expression);
        return parser.ParseExpression();
    }

    private sealed class Parser
    {
        private readonly string _expression;
        private int _position;
        private int _nextBindingId;

        public Parser(string expression)
        {
            _expression = expression;
            _nextBindingId = 0;
        }

        public RecSelectionExpression ParseExpression()
        {
            SkipWhitespace();
            var parsed = ParseOrExpression();
            SkipWhitespace();

            if (!IsEnd)
            {
                throw InvalidExpression();
            }

            return parsed;
        }

        private RecSelectionExpression ParseOrExpression()
        {
            var left = ParseAndExpression();

            while (true)
            {
                SkipWhitespace();
                if (!TryConsume("||"))
                {
                    return left;
                }

                var right = ParseAndExpression();
                left = new RecSelectionBinaryExpression(left, RecSelectionBinaryOperator.Or, right);
            }
        }

        private RecSelectionExpression ParseAndExpression()
        {
            var left = ParseUnaryExpression();

            while (true)
            {
                SkipWhitespace();
                if (!TryConsume("&&"))
                {
                    return left;
                }

                var right = ParseUnaryExpression();
                left = new RecSelectionBinaryExpression(left, RecSelectionBinaryOperator.And, right);
            }
        }

        private RecSelectionExpression ParseUnaryExpression()
        {
            SkipWhitespace();

            if (TryConsume("!"))
            {
                var operand = ParseUnaryExpression();
                return new RecSelectionUnaryExpression(RecSelectionUnaryOperator.Not, operand);
            }

            return ParsePrimaryExpression();
        }

        private RecSelectionExpression ParsePrimaryExpression()
        {
            SkipWhitespace();

            if (TryConsume("("))
            {
                var inner = ParseOrExpression();
                SkipWhitespace();
                if (!TryConsume(")"))
                {
                    throw InvalidExpression();
                }

                return inner;
            }

            return ParseComparisonExpression();
        }

        private RecSelectionExpression ParseComparisonExpression()
        {
            SkipWhitespace();
            var fieldNameStart = _position;

            while (!IsEnd)
            {
                if (PeekStartsWith("!=")
                    || PeekStartsWith("<=")
                    || PeekStartsWith(">=")
                    || PeekStartsWith("=")
                    || PeekStartsWith("~")
                    || PeekStartsWith("<")
                    || PeekStartsWith(">"))
                {
                    break;
                }

                if (PeekStartsWith("&&") || PeekStartsWith("||") || Current == ')')
                {
                    throw InvalidExpression();
                }

                _position++;
            }

            if (IsEnd)
            {
                throw InvalidExpression();
            }

            var fieldName = _expression[fieldNameStart.._position].Trim();
            if (string.IsNullOrWhiteSpace(fieldName))
            {
                throw InvalidExpression();
            }

            RecSelectionExpressionOperator op;
            if (TryConsume("!="))
            {
                op = RecSelectionExpressionOperator.NotEquals;
            }
            else if (TryConsume("<="))
            {
                op = RecSelectionExpressionOperator.LessThanOrEqual;
            }
            else if (TryConsume(">="))
            {
                op = RecSelectionExpressionOperator.GreaterThanOrEqual;
            }
            else if (TryConsume("="))
            {
                op = RecSelectionExpressionOperator.Equals;
            }
            else if (TryConsume("~"))
            {
                op = RecSelectionExpressionOperator.Contains;
            }
            else if (TryConsume("<"))
            {
                op = RecSelectionExpressionOperator.LessThan;
            }
            else if (TryConsume(">"))
            {
                op = RecSelectionExpressionOperator.GreaterThan;
            }
            else
            {
                throw InvalidExpression();
            }

            SkipWhitespace();
            var valueToken = ReadValueToken();
            var value = ParseValue(valueToken);

            return new RecSelectionComparisonExpression(_nextBindingId++, fieldName, op, value);
        }

        private string ReadValueToken()
        {
            if (IsEnd)
            {
                throw InvalidExpression();
            }

            if (Current == '"')
            {
                var start = _position;
                _position++;

                while (!IsEnd && Current != '"')
                {
                    _position++;
                }

                if (IsEnd)
                {
                    throw InvalidExpression();
                }

                _position++;
                return _expression[start.._position];
            }

            var tokenStart = _position;

            while (!IsEnd)
            {
                if (char.IsWhiteSpace(Current) || PeekStartsWith("&&") || PeekStartsWith("||") || Current == ')')
                {
                    break;
                }

                _position++;
            }

            if (_position == tokenStart)
            {
                throw InvalidExpression();
            }

            return _expression[tokenStart.._position];
        }

        private string ParseValue(string token)
        {
            if (token.Length >= 2 && token[0] == '"' && token[^1] == '"')
            {
                return token[1..^1];
            }

            if (token.Contains(' ', StringComparison.Ordinal))
            {
                throw InvalidExpression();
            }

            return token;
        }

        private bool TryConsume(string token)
        {
            if (!PeekStartsWith(token))
            {
                return false;
            }

            _position += token.Length;
            return true;
        }

        private bool PeekStartsWith(string token)
        {
            if (_position + token.Length > _expression.Length)
            {
                return false;
            }

            return string.Compare(_expression, _position, token, 0, token.Length, StringComparison.Ordinal) == 0;
        }

        private void SkipWhitespace()
        {
            while (!IsEnd && char.IsWhiteSpace(Current))
            {
                _position++;
            }
        }

        private bool IsEnd => _position >= _expression.Length;

        private char Current => _expression[_position];

        private FormatException InvalidExpression()
        {
            return new FormatException($"Invalid selection expression: '{_expression}'.");
        }
    }
}
