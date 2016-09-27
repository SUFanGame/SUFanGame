using System;
using System.Collections.Generic;
using StevenUniverse.FanGame.Data;
using StevenUniverse.FanGame.Data.DataTypes;
using StevenUniverse.FanGame.Entities;

namespace StevenUniverse.FanGame.Util.Logic
{
    // Expression         := [ "!" ] <Boolean> { <BooleanOperator> <Boolean> } ...
    // Boolean            := <BooleanConstant> | <Expression> | "(" <Expression> ")"
    // BooleanOperator    := "And" | "Or" 
    // BooleanConstant    := "True" | "False"
    public class Parser
    {
        private readonly IEnumerator<Token> _tokens;

        public Parser(IEnumerable<Token> tokens)
        {
            _tokens = tokens.GetEnumerator();
            _tokens.MoveNext();
        }

        public DataType Parse()
        {
            while (_tokens.Current != null)
            {
                bool isNegated = _tokens.Current is NegationToken;
                if (isNegated)
                {
                    _tokens.MoveNext();
                }

                DataType currentDataType = ParseDataType();
                DataBool currentDataBool = currentDataType as DataBool;

                if (isNegated)
                {
                    if (currentDataBool != null)
                    {
                        currentDataBool.Data = !currentDataBool.Data;
                    }
                    else
                    {
                        throw new Exception("Cannot negate non-DataBool DataType!");
                    }
                }

                if (_tokens.Current is ComparisonToken)
                {
                    while (_tokens.Current is ComparisonToken)
                    {
                        ComparisonToken comparison = (ComparisonToken)_tokens.Current;
                        if (!_tokens.MoveNext())
                        {
                            throw new Exception("Missing expression after comparison");
                        }

                        DataType nextDataType = ParseDataType();

                        currentDataType = new DataBool("temp", comparison.Evaluate(currentDataType, nextDataType));
                    }
                }
                else if (_tokens.Current is OperandToken)
                {
                    while (_tokens.Current is OperandToken)
                    {
                        Token operand = _tokens.Current;
                        if (!_tokens.MoveNext())
                        {
                            throw new Exception("Missing expression after operand");
                        }

                        DataType nextDataType = ParseDataType();
                        DataBool nextDataBool = nextDataType as DataBool;

                        if (currentDataBool != null && nextDataBool != null)
                        {
                            if (operand is AndToken)
                            {
                                currentDataType = new DataBool("temp", currentDataBool.Data && nextDataBool.Data);
                            }
                            else
                            {
                                currentDataType = new DataBool("temp", currentDataBool.Data || nextDataBool.Data);
                            }
                        }
                        else
                        {
                            throw new Exception("Cannot perform boolean operation on non-booleans!");
                        }
                    }
                }
                /*
                else
                {
                    throw new Exception("Invalid token type!");
                }
                */

                return currentDataType;
            }

            throw new Exception("Empty expression");
        }

        private DataType ParseDataType()
        {
            if (_tokens.Current is DataToken)
            {
                DataToken current = (DataToken)_tokens.Current;
                DataType currentDataType = current.DataType;

                _tokens.MoveNext();

                return currentDataType;
            }
            if (_tokens.Current is OpenParenthesisToken)
            {
                _tokens.MoveNext();

                var expInPars = Parse();

                if (!(_tokens.Current is ClosedParenthesisToken))
                    throw new Exception("Expecting Closing Parenthesis");

                _tokens.MoveNext();

                return expInPars;
            }
            if (_tokens.Current is ClosedParenthesisToken)
                throw new Exception("Unexpected Closed Parenthesis");

            // since its not a BooleanConstant or Expression in parenthesis, it must be a expression again
            var val = Parse();
            return val;
        }
    }
}