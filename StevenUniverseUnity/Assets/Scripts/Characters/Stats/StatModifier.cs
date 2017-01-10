using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SUGame.PropertyAttributes;
using SUGame.StrategyMap;
using UnityEngine.Events;

/// <summary>
/// A modifier for a stat. Modifiers are equated by name and compared by priority.
/// Modifiers are applied in sequential order using the specified operation.
/// NOTE: The modifier value will be calculated on the spot and the result will be ADDED
/// to the current value of the stat.
/// </summary>
[System.Serializable]
public class StatModifier : System.IEquatable<StatModifier>, System.IComparable<StatModifier>
{
    public string name_;
    public int priority_ = 0;
    [Header("Value to Add")]
    public LeftOperand leftOperand_ = LeftOperand.CURRENT_VALUE;
    public Operator operator_ = Operator.PLUS;
    public float rightOperand_ = 0;
    System.Func<int, float, int> operation_;
    [Space(10)]


    [SerializeField,ReadOnly]
    int modValue_ = 0;
    /// <summary>
    /// The amount by which the stat is modified.
    /// </summary>
    public int ModValue_ { get { return modValue_; } }

    /// <summary>
    /// Sets ModValue based on our current operation.
    /// </summary>
    /// <param name="stat"></param>
    public void UpdateValue( Stat stat )
    {
        switch (operator_)
        {
            case Operator.PLUS: operation_ = Add; break;
            case Operator.MINUS: operation_ = Sub; break;
            case Operator.MULTIPLIED_BY: operation_ = Mul; break;
            case Operator.DIVIDED_BY: operation_ = Div; break;
        }

        int leftOperand = 0;
        switch( leftOperand_ )
        {
            case LeftOperand.BASE_VALUE: leftOperand = stat.BaseValue_; break;
            case LeftOperand.CURRENT_VALUE: leftOperand = stat.CurrentValue_; break;
            case LeftOperand.MAX_VALUE: leftOperand = stat.MaxValue_; break;
        }
        // The value to add to Stat.Currnt
        modValue_ = operation_(leftOperand, rightOperand_) - leftOperand;
    }

    public enum Operator
    {
        PLUS = 0,
        MINUS = 1,
        MULTIPLIED_BY = 2,
        DIVIDED_BY = 3
    };

    /// <summary>
    /// Determines which value to use for the left operand
    /// </summary>
    public enum LeftOperand
    {
        CURRENT_VALUE,
        MAX_VALUE,
        BASE_VALUE,
    }

    static int Mul(int value, float operand)
    {
        return Mathf.RoundToInt((float)value * operand);
    }

    static int Add(int value, float operand)
    {
        return value + Mathf.RoundToInt(operand);
    }

    static int Sub(int value, float operand)
    {
        return value - Mathf.RoundToInt(operand);
    }

    static int Div(int value, float operand)
    {
        return operand == 0 ? 0 : Mathf.RoundToInt((float)value / operand);
    }

    public bool Equals(StatModifier other)
    {
        return name_.Equals(other.name_);
    }

    public override bool Equals( object other )
    {
        var o = other as StatModifier;
        if (o == null)
            return false;
        return name_.Equals(o);
    }

    public override int GetHashCode()
    {
        return name_.GetHashCode();
    }
    
    public int CompareTo(StatModifier other)
    {
        // Higher priority modifiers get applied first
        return -priority_.CompareTo(other.priority_);
    }
}
