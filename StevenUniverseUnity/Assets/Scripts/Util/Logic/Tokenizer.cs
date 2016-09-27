using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using StevenUniverse.FanGame.Data;
using StevenUniverse.FanGame.Data.DataTypes;
using StevenUniverse.FanGame.Entities;
using StevenUniverse.FanGame.Extensions;

namespace StevenUniverse.FanGame.Util.Logic
{
    public class Tokenizer
    {
        private readonly StringReader _reader;
        private string _text;

        public Tokenizer(string text)
        {
            _text = text;
            _reader = new StringReader(text);
        }

        public IEnumerable<Token> Tokenize()
        {
            var tokens = new List<Token>();
            while (_reader.Peek() != -1)
            {
                while (Char.IsWhiteSpace((char) _reader.Peek()))
                {
                    _reader.Read();
                }

                if (_reader.Peek() == -1)
                    break;

                var c = (char) _reader.Peek();
                switch (c)
                {
                    case '!':
                        tokens.Add(new NegationToken());
                        _reader.Read();
                        break;
                    case '(':
                        tokens.Add(new OpenParenthesisToken());
                        _reader.Read();
                        break;
                    case ')':
                        tokens.Add(new ClosedParenthesisToken());
                        _reader.Read();
                        break;
                    default:
                        if (CharIsValidSymbol(c))
                        {
                            var token = ParseKeyword();
                            tokens.Add(token);
                        }
                        else
                        {
                            var remainingText = _reader.ReadToEnd() ?? string.Empty;
                            throw new Exception(string.Format("Unknown grammar found at position {0} : '{1}'",
                                _text.Length - remainingText.Length, remainingText));
                        }
                        break;
                }
            }
            return tokens;
        }

        private bool CharIsValidSymbol(char c)
        {
            return Char.IsLetter(c) || Char.IsDigit(c) || c == '.' || c == '<' || c == '>' || c == '=';
        }

        private Token ParseKeyword()
        {
            var text = new StringBuilder();
            while (CharIsValidSymbol((char) _reader.Peek()))
            {
                text.Append((char) _reader.Read());
            }

            var potentialKeyword = text.ToString().ToLower();

            switch (potentialKeyword)
            {
            case "true":
                return DataToken.GenerateBool(true);
            case "false":
                return DataToken.GenerateBool(false);
            case "and":
                return new AndToken();
            case "or":
                return new OrToken();
            default:

                switch (potentialKeyword)
                {
                case "==": return new ComparisonToken("==");
                case "<=": return new ComparisonToken("<=");
                case ">=": return new ComparisonToken(">=");
                case "<": return new ComparisonToken("<");
                case ">": return new ComparisonToken(">");
                }

                if (potentialKeyword.IndexOfAny(new char[] { '<', '>', '=' }) != -1)
                {
                    throw new Exception("Potential keyword contained unexpected symbol!");
                }

                return new DataToken(GetDataFromString(potentialKeyword));
            }
        }

        private DataType GetDataFromString(string segmentString)
        {
            if (segmentString.Contains("."))
            {
                string[] splitSegment1 = segmentString.Split('.');

                if (splitSegment1.Length != 2)
                {
                    throw new Exception(string.Format("Invalid segment: '{0}'. variable must be split into 2 segments by an period ex: 'dataGroup.dataBool'", segmentString));
                }

                string segmentDataGroupName = splitSegment1[0];
                string segmentDataName = splitSegment1[1];

                return GameController.Instance.CurrentPlayer.SourcePlayer.SavedData.GetDataOfAnyType(segmentDataGroupName, segmentDataName);
            }
            else
            {
                bool possibleBool;
                int possibleInt;

                if (bool.TryParse(segmentString, out possibleBool))
                {
                    return new DataBool("temp", possibleBool);
                }
                else if (int.TryParse(segmentString, out possibleInt))
                {
                    return new DataInt("temp", possibleInt);
                }
                else
                {
                    throw new Exception(string.Format("Failed to parse inline datatype from '{0}'.", segmentString));
                }
            }
        }
    }
}