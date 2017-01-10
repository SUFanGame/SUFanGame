using UnityEngine;
using System.Collections;
using System.Collections.Generic;



/// <summary>
/// Stats component for characters (or anything really).
/// Note that stats are maintained in an lsit to make it easy to reference them programmatically.
/// 
/// </summary>
[ExecuteInEditMode]
public class Stats : MonoBehaviour
{
    public const int MAX_LEVEL_ = 100;
    [SerializeField, Range(1, MAX_LEVEL_)]
    int level_ = 1;
    public int Level_ { get { return level_; } }

    [SerializeField]
    List<Stat> stats_ = new List<Stat>();
    public IList<Stat> Stats_
    {
        get; private set;
    }

    /// <summary>
    /// Vitality is multiplied by this to give base HP.
    /// </summary>
    [SerializeField]
    float vitalityMultiplier_ = 1f;

    void Awake()
    {
        Stats_ = stats_.AsReadOnly();
        OnValidate();
    }

    public void ModifyCurrent(Stat.Type type, int modifier)
    {
        SetCurrent(type, GetCurrent(type) + modifier);
    }

    public void SetCurrent(Stat.Type type, int val)
    {
        if ((int)type >= stats_.Count)
            return;

        stats_[(int)type].CurrentValue_ = val;

        if (type == Stat.Type.VIT)
        {
            UpdateHP();
        }
    }

    /// <summary>
    /// Returns the value of a stat at the current level.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public int GetCurrent(Stat.Type type)
    {
        if( (int)type >= stats_.Count )
            return 0;
        return stats_[(int)type];
    }

   // public const int MaxLevel_ = 15;

    const int minHP_ = 6;


    public void OnValidate()
    {
        foreach (var s in stats_)
        {
            s.Level_ = level_;
            s.OnValidate();
        }
    }

    void UpdateHP()
    {
        // Set our HP based on our vitality.
        SetCurrent(Stat.Type.HP, (int)(GetCurrent(Stat.Type.VIT) * vitalityMultiplier_ + minHP_));
    }

    public Stat this[Stat.Type t ]
    {
        get { return stats_[(int)t]; }
        set { stats_[(int)t] = value; }
    }

    //[ContextMenu("Print Results")]
    //void Print()
    //{
    //    if (stats_ == null)
    //        return;

    //    foreach( var s in stats_ )
    //    {
    //        s.Print();
    //    }
    //}


    [ContextMenu("RefreshModifiers")]
    void RefreshModifiers()
    {
        if (stats_ == null)
            return;

        foreach (var s in stats_)
        {
            //s.RefreshModifiers();
        }
    }
}
