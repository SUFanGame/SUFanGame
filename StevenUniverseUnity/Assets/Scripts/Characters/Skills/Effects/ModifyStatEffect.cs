using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SUGame.StrategyMap;

/// <summary>
/// Modify a stat in some way
/// </summary>
[CreateAssetMenu(fileName = "Modify Stat", menuName = "Skills/Effects/Modify Stat", order = 9000)]
public class ModifyStatEffect : CombatEffect
{
    public Stat.Type stat_;

    public Operator operator_ = Operator.ADD;

    public Target target_;

    public StatModifier mod_;

    [ContextMenuItem("PrintResult", "PrintResult")]
    public int operand_ = 0;

    System.Func<int, int, int> operation_ = null;

    public override void Execute(Combat combat)
    {
        var tar = target_ == Target.ATTACKER ? combat.Attacker_ : combat.Defender_;
        //tar.Data.Stats_[stat_] = 
    }

    public enum Operator
    {
        ADD = 0,
        SUBTRACT = 1,
        MULTIPLY = 2,
        DIVIDE = 3
    };

    public enum Target
    {
        ATTACKER,
        DEFENDER,
    };

    void OnEnable()
    {
        OnValidate();
    }

    void OnValidate()
    {
        switch( operator_ )
        {
            case Operator.ADD: operation_ = Add; break;
            case Operator.SUBTRACT: operation_ = Sub; break;
            case Operator.MULTIPLY: operation_ = Mul; break;
            case Operator.DIVIDE: operation_ = Div; break;
        }
    }

    // For testing purposes
    void PrintResult()
    {
        if( operation_ == null )
        {
            Debug.LogError("Delegate is null");
        }
        Debug.Log(operation_(2, operand_));
    }

    static int Mul(int a, int b )
    {
        return a * b;
    }

    static int Add( int a, int b )
    {
        return a + b;
    }

    static int Sub( int a, int b )
    {
        return a - b;
    }

    static int Div( int a, int b )
    {

        return b == 0 ? 0 : Mathf.RoundToInt((float)a / (float)b);
    }


}
