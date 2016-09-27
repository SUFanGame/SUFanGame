using System;
using StevenUniverse.FanGame.Data;
using StevenUniverse.FanGame.Data.DataTypes;
using StevenUniverse.FanGame.Entities;

namespace StevenUniverse.FanGame.Util.Logic
{
    public class OperandToken : Token
    {
    }

    public class OrToken : OperandToken
    {
    }

    public class AndToken : OperandToken
    {
    }

    public class DataToken : Token
    {
        public static DataToken GenerateBool(bool data)
        {
            return new DataToken(new DataBool("temp", data));
        }

        private DataType dataType;

        public DataToken(DataType dataType)
        {
            this.dataType = dataType;
        }

        public DataType DataType { get { return dataType; } }
    }

    public class ComparisonToken : Token
    {
        private string comparison;

        public ComparisonToken(string comparison)
        {
            this.comparison = comparison;
        }

        public bool Evaluate(DataType segment1Data, DataType segment2Data)
        {
            int comparisonValue = segment1Data.CompareTo(segment2Data);
            bool derivedBoolean;
            switch (comparison)
            {
            case "==":
                derivedBoolean = comparisonValue == 0;
                break;
            case "<=":
                derivedBoolean = comparisonValue == 0 || comparisonValue == -1;
                break;
            case ">=":
                derivedBoolean = comparisonValue == 0 || comparisonValue == 1;
                break;
            case "<":
                derivedBoolean = comparisonValue == -1;
                break;
            case ">":
                derivedBoolean = comparisonValue == 1;
                break;
            default:
                throw new Exception("Invalid operator!");
            }

            return derivedBoolean;
        }
    }

    /*
    public class BooleanValueToken : Token
    {
        private bool data;

        public BooleanValueToken(bool data)
        {
            this.data = data;
        }

        public bool Data { get { return data; } }
    }*/

    public class ParenthesisToken : Token
    {
    }

    public class ClosedParenthesisToken : ParenthesisToken
    {
    }

    public class OpenParenthesisToken : ParenthesisToken
    {
    }

    public class NegationToken : Token
    {
    }

    public abstract class Token
    {
    }
}