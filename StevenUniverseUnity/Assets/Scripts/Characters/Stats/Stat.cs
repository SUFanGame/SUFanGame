using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SUGame.Util.Collections;
using SUGame.PropertyAttributes;

[System.Serializable]
public class Stat
{
    [SerializeField]
    string name_;
    public string Name_ { get { return name_; } }

    [SerializeField]
    int level_;
    /// <summary>
    /// Current "Level" for this stat. Determine's the stat's <see cref="BaseValue_"/> which in turn determines
    /// the stat's <see cref="MaxValue_"/>.
    /// </summary>
    public int Level_
    {
        get { return level_; }
        set
        {
            if (value == level_)
                return;
            SetBaseValueForLevel(value);
            level_ = value;
        }
    }

    [SerializeField]
    AnimationCurve levellingCurve_;
    /// <summary>
    /// Determines a stat's base value at any given level from 1 to <see cref="Stats.MAX_LEVEL_"/>.
    /// <see cref="GetBaseValueAtLevel(int)"/> can be used to retrieve a value.
    /// </summary>
    public AnimationCurve LevellingCurve_ { get { return levellingCurve_; } }

    [Header("Range")]
    [SerializeField]
    int minLevelValue_;
    /// <summary>
    /// The value of this stat at level 1, before modifiers.
    /// </summary>
    public int MinLevelValue_
    {
        get { return minLevelValue_; }
        set
        {
            minLevelValue_ = Mathf.Clamp(value, 0, int.MaxValue);
            MaxLevelValue_ = maxLevelValue_;
            SetBaseValueForLevel(level_);
        }
    }


    [SerializeField]
    int maxLevelValue_;
    /// <summary>
    /// The value of the stat at max level, before modifiers.
    /// </summary>
    public int MaxLevelValue_
    {
        get { return maxLevelValue_; }
        set
        {
            maxLevelValue_ = Mathf.Clamp(value, minLevelValue_, int.MaxValue);
            SetBaseValueForLevel(level_);
        }
    }


    [Header("Current/Max")]
    [SerializeField]
    int currentValue_;
    /// <summary>
    /// The current value of the stat. Clamped from 0 to <see cref="MaxValue_"/>
    /// </summary>
    public int CurrentValue_
    {
        get { return currentValue_; }
        set { currentValue_ = Mathf.Clamp(value, 0, MaxValue_); }
    }



    // Editor only
    [SerializeField, ReadOnly]
    int maxValue_;

    /// <summary>
    /// The maximum possible value of this stat including current modifiers.
    /// <see cref="CurrentValue_"/> is clamped from 0 to this.
    /// </summary>
    public int MaxValue_
    {
        get
        {
            int val = baseValue_ + modValue_;
            return Mathf.Clamp(val, 0, int.MaxValue);
        }
    }



    [Header("Base Value")]
    [SerializeField, ReadOnly]
    int baseValue_;
    /// <summary>
    /// The base value of this stat at a given level. This can be set from <see cref="SetBaseValueForLevel(int)"/>
    /// </summary>
    public int BaseValue_ { get { return baseValue_; } }

    /// <summary>
    /// The value added to <see cref="BaseValue_"/> to determine <see cref="MaxValue_"/>.
    /// Note this is not a direct sum of modifiers, but result of each modifier being applied in sequence.
    /// This means that if any modifier is removed, all modifications AFTER the removed modification must
    /// be reapplied, since their results might be different.
    /// </summary>
    [SerializeField, ReadOnly]
    int modValue_ = 0;
   


    /// <summary>
    /// Modifiers which are applied to a stat's base value to result in <see cref="MaxValue_"/>.
    /// </summary>
    //[SerializeField]
    //ModifiersDict modifiers_ = new ModifiersDict();
    [Space(10)]
    [SerializeField]
    List<StatModifier> modifiers_ = new List<StatModifier>();

    public void Initialize(string name, int minValue, int maxValue, int level, int current)
    {
        name_ = name;
        minLevelValue_ = minValue;
        maxLevelValue_ = maxValue;
        SetBaseValueForLevel(level);
        currentValue_ = MaxValue_;
    }

    // TODO : Rather than replace modifiers of the same type we could make them additive in some way,
    // so maybe some stack or some don't. Rather than replace exising, we get new modifier value
    // and add it to existing? Then there would be no easy way to undo only one of the modifiers...
    /// <summary>
    /// Add the given modifier to this stat if valid. Note if a modifier of this type
    /// (name) already exists, this will only be applied if it's a higher priority.
    /// </summary>
    public void AddModifier( StatModifier modifier )
    {
        // Get index of existing modifiers of this type if any - note modifiers
        // are equated by name only, no other factor is considered for equality.
        int index = modifiers_.IndexOf(modifier);
        if( index == -1 )
        {
            // If there are none, apply our new modifier.
            modifiers_[index] = modifier;
            modValue_ += modifiers_[index].ModValue_;
        }
        else
        {
            // If there is one, check if our priority is higher
            // Not sure about this priority system.
            var existing = modifiers_[index];
            //if (existing.priority_ < modifier.priority_)
            {
                // If so, remove our old modifier and apply this new one
                modifier.UpdateValue(this);
                modifiers_[index] = modifier;
            }
            // Otherwise do nothing
        }

    }

    /// <summary>
    /// This should be called any time a modification is added or removed since modifiers 
    /// are applied in a sequence.
    /// </summary>
    void RefreshModifiers()
    {
        // If we want each mod to only refer to the original stat's values, this is how.
        //float t = Mathf.InverseLerp(0, MaxValue_, CurrentValue_);
        // Debug.LogFormat("From: {0}, To: {1}, T: {2}, Curr: {3}", 0, MaxValue_, t, CurrentValue_);
        int diff = MaxValue_ - CurrentValue_;

        modValue_ = 0;

        foreach ( var m in modifiers_ )
        {
            m.UpdateValue(this);
            // If we want to immediately apply each mod in sequence, this is how
            //float t = Mathf.InverseLerp(0, MaxValue_, CurrentValue_);
            modValue_ += m.ModValue_;
            
            //Debug.LogFormat("Name: {0}, Current: {1}, Max: {2}, Mod: {3}, T: {4}", m.name_, CurrentValue_, MaxValue_, m.ModValue_, t);

            //CurrentValue_ = Mathf.CeilToInt(Mathf.Lerp(0, MaxValue_, t));
        }

        CurrentValue_ = MaxValue_ - diff;
        //CurrentValue_ = LerpCurrent(t);
        //Debug.LogFormat("Current after mods: {0}", CurrentValue_);

        //Debug.LogFormat("Stat after modifiers: Base: {0}, Current: {1}, Max: {2}, Mod: {3}", BaseValue_, CurrentValue_, MaxValue_, modValue_);
        //currentValue_ = oldVal;
    }

    /// <summary>
    /// Sets the stat's base value for the given level.
    /// </summary>
    /// <param name="level">The level of the value we want.</param>
    void SetBaseValueForLevel( int level )
    {
        int diff = MaxValue_ - CurrentValue_;
        //float t = Mathf.InverseLerp(0, MaxValue_, CurrentValue_);
        baseValue_ = GetBaseValueAtLevel(level);

        currentValue_ = MaxValue_ - diff;
        //CurrentValue_ = LerpCurrent(t);
        RefreshModifiers();
    }

    int LerpCurrent( float t )
    {
        return Mathf.RoundToInt(Mathf.Lerp(0, MaxValue_, t));
    }

    /// <summary>
    /// Retrieve the base value for this stat at the given level based on the levelling curve and the stat range.
    /// </summary>
    public int GetBaseValueAtLevel( int level )
    {
        // Get our T value from our current level.
        float normalizedLevel = Mathf.InverseLerp( 1, Stats.MAX_LEVEL_, level );

        // Find our stat's position on the curve, based on the given level.
        float normalizedCurveVal = LevellingCurve_.Evaluate(normalizedLevel);

        // Get our actual value from the curve using our stats min-max range.
        return Mathf.RoundToInt(Mathf.Lerp(minLevelValue_, maxLevelValue_, normalizedCurveVal));
    }

    /// <summary>
    /// Returns the stat's value in the range 0-1
    /// </summary>
    public float NormalizedValue_
    {
        get
        {
            return Mathf.InverseLerp(0, MaxValue_, CurrentValue_);
        }

    }

    #region operators

    public static implicit operator int (Stat stat)
    {
        return stat.CurrentValue_;
    }

    

    public static int operator +(Stat left, int right)
    {
        return left.CurrentValue_ + right;
    }

    public static int operator -(Stat left, int right)
    {
        return left.CurrentValue_ - right;
    }

    public static int operator +(Stat left, Stat right)
    {
        return left.currentValue_ + right.CurrentValue_;
    }

    public static int operator -(Stat left, Stat right)
    {
        return left.CurrentValue_ - right.currentValue_;
    }

    public static bool operator ==(Stat left, Stat right)
    {
        return left.CurrentValue_ == right.CurrentValue_;
    }

    public static bool operator !=(Stat left, Stat right)
    {
        return left.CurrentValue_ != right.CurrentValue_;
    }
    
    public static bool operator <(Stat left, Stat right)
    {
        return left.CurrentValue_ < right.currentValue_;
    }

    public static bool operator >(Stat left, Stat right)
    {
        return left.CurrentValue_ > right.CurrentValue_;
    }

    public static bool operator >=(Stat left, Stat right)
    {
        return left.CurrentValue_ >= right.CurrentValue_;
    }

    public static bool operator <=(Stat left, Stat right)
    {
        return left.CurrentValue_ <= right.CurrentValue_;
    }

    public static bool operator ==(Stat left, int right)
    {
        return left.CurrentValue_ == right;
    }

    public static bool operator !=(Stat left, int right)
    {
        return left.CurrentValue_ != right;
    }


    public static bool operator <(Stat left, int right)
    {
        return left.CurrentValue_ < right;
    }

    public static bool operator >(Stat left, int right)
    {
        return left.CurrentValue_ > right;
    }

    public static bool operator >=(Stat left, int right)
    {
        return left.CurrentValue_ >= right;
    }

    public static bool operator <=(Stat left, int right)
    {
        return left.CurrentValue_ <= right;
    }

    #endregion

    #region comparisoninterfaces
    public int CompareTo(Stat other)
    {
        return CurrentValue_.CompareTo(other.CurrentValue_);
    }

    public override bool Equals(object obj)
    {
        return obj is Stat && this.Equals((Stat)obj);
    }

    public bool Equals(Stat other)
    {
        return CurrentValue_.Equals(other.CurrentValue_);
    }

    public override int GetHashCode()
    {
        return CurrentValue_.GetHashCode();
    }
    #endregion


    public enum Type
    {
        //LVL = 0,
        HP = 1,
        STR = 2,
        DEF = 3,
        VIT = 4,
        SPD = 5,
        XP = 6,
    };

    /// <summary>
    /// Get the proper name for the given stat.
    /// </summary>
    public static string GetProperName_( Type type )
    {
        return properStatNames_[(int)type];
    }
    
    public static readonly string[] properStatNames_ = new string[]
    {
        //"Level",
        "Hit Points",
        "Strength",
        "Defense",
        "Vitality",
        "Speed",
        "Experience",
    };

    //public void Print()
    //{
    //    var val = GetBaseValueAtLevel(1);
    //    int originalVal = currentValue_;
    //    modValue_ = 0;
    //    foreach (var mod in modifiers_)
    //    {
    //        Debug.LogFormat("Before mod Base: {0}, Current: {1}, Max: {2}", BaseValue_, CurrentValue_, MaxValue_);
    //        mod.UpdateValue(this);
    //        float t = Mathf.InverseLerp(0, MaxValue_, CurrentValue_);
    //        modValue_ += mod.ModValue_;
    //        CurrentValue_ = Mathf.FloorToInt(Mathf.Lerp(0, MaxValue_, t));
    //        Debug.LogFormat("Modding stat by {0} {1} {2} ({3})", mod.leftOperand_, mod.operator_, mod.rightOperand_, mod.ModValue_ );
    //    }
    //    Debug.LogFormat("Base: {0}, Current: {1}, Max: {2}, ModValue: {3}", BaseValue_, CurrentValue_, MaxValue_, modValue_);
    //    modValue_ = 0;
    //    currentValue_ = originalVal;
    //}

    /// <summary>
    /// The total count of all stat types.
    /// </summary>
    public static int TypeCount_ { get { return properStatNames_.Length; } }

    public void OnValidate()
    {
        MinLevelValue_ = minLevelValue_;
        MaxLevelValue_ = maxLevelValue_;
        Level_ = level_;
        CurrentValue_ = currentValue_;

        // Editor only
        maxValue_ = MaxValue_;

    }

   // public Dictionary<string,StatModifier>.Enumerator ModifiersEnumerator_ { get { return modifiers_.GetEnumerator(); } }

    //[System.Serializable]
    //class ModifiersDict : SerializableDictionary<string, StatModifier> { }
}
