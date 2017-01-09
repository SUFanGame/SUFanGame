using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SUGame.Util.Collections;

[System.Serializable]
public class Stat
{
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
        }
    }


    [SerializeField]
    int current_;
    /// <summary>
    /// The current value of the stat. Clamps from 0 to max_
    /// </summary>
    public int Current_
    {
        get { return current_; }
        set { current_ = Mathf.Clamp(value,0, max_); }
    }


    [SerializeField]
    int max_;
    /// <summary>
    /// The maximum value of this stat at the current level, AFTER modifiers. <see cref="Current_"/> is 
    /// clamped from 0 to this.
    /// </summary>
    public int Max_
    {
        get
        {
            return max_;
        }
    }


    [SerializeField]
    AnimationCurve levellingCurve_;
    /// <summary>
    /// Determines a stat's base value at any given level from 1 to <see cref="Stats.MAX_LEVEL_"/>.
    /// <see cref="GetBaseValueAtLevel(int)"/> can be used to retrieve a value.
    /// </summary>
    public AnimationCurve LevellingCurve_ { get { return levellingCurve_; } }
    
    /// <summary>
    /// Modifiers which are applied to a state's Max_ value.
    /// </summary>
    [SerializeField]
    ModifiersDict modifiers_ = new ModifiersDict();


    // Not sure this is the best route for modifiers, but should be fine for now.
    // It will be caller's responsibility to remove modifiers as appropriate.
    /// <summary>
    /// Add or change a modifier to this stat. Pass in 0 to
    /// remove a modifier. By default <see cref="Current_"/> will automatically be
    /// set to <see cref="Max_"/> for this stat based on the argument 
    /// <paramref name="currentBecomesMax"/>
    /// </summary>
    /// <param name="name">The name of the modifier.</param>
    /// <param name="value">The value of the modifier.</param>
    /// <param name="currentBecomesMax">If true, the current value will be set to max.</param>
    public void SetModifier( string name, int value, bool currentBecomesMax = true )
    {
        if (value == 0 )
        {
            // If we pass in 0 but the modifier doesn't exist then nothing happens.
            if (!modifiers_.ContainsKey(name))
                return;

            // Otherwise remove the previously added modifier.
            max_ -= modifiers_[name];
            modifiers_.Remove(name);
            return;
        }

        if( modifiers_.ContainsKey(name) )
        {
            max_ -= modifiers_[name];
            max_ += value;
            modifiers_[name] = value;
            return;
        }

        modifiers_[name] = value;
        max_ += value;
    }

    /// <summary>
    /// Sets the stat's max and current values based on the given level and current modifiers.
    /// </summary>
    /// <param name="level">The level of the value we want.</param>
    /// <param name="currentBecomesMax">If set to true the current value will be set to max, otherwise
    /// it will remain unchanged, or lower if max is now below current.</param>
    public void SetValueForLevel( int level, bool currentBecomesMax = false )
    {
        int val = GetBaseValueAtLevel(level);
        foreach( var mod in modifiers_ )
        {
            val += mod.Value;
        }
        max_ = val;
        Current_ = currentBecomesMax ? max_ : current_;
    }

    /// <summary>
    /// Retrieve the value for this stat at the given level based on the levelling curve.
    /// </summary>
    /// <param name="level"></param>
    /// <returns></returns>
    public int GetBaseValueAtLevel( int level )
    {
        // Get our T value from our current level.
        float normalizedLevel = Mathf.InverseLerp( 1, Stats.MAX_LEVEL_, level );

        // Find our stat's position on the curve, based on the given level.
        float normalizedCurveVal = LevellingCurve_.Evaluate(normalizedLevel);

        // Get our actual value from the curve using our stats min-max range.
        return Mathf.RoundToInt(Mathf.Lerp(minLevelValue_, maxLevelValue_, normalizedCurveVal));
    }


    public Stat( int value ) : this( value, value + 1, value) {}

    public Stat( int minLevel, int maxLevel, int current )
    {
        minLevelValue_ = minLevel;
        maxLevelValue_ = maxLevel;
        current_ = current;
        max_ = current;
        levellingCurve_ = AnimationCurve.Linear(0, 0, 1, 1);
    }

    /// <summary>
    /// Returns the stat's value in the range 0-1
    /// </summary>
    public float NormalizedValue_
    {
        get
        {
            return Mathf.InverseLerp(0, Max_, Current_);
        }

    }

    #region operators


    public static implicit operator Stat(int val)
    {
        return new Stat(val);
    }

    public static implicit operator int (Stat stat)
    {
        return stat.Current_;
    }

    public static Stat operator --(Stat value)
    {
        return value.Current_--;
    }

    public static Stat operator ++(Stat value)
    {
        return value.Current_++;
    }

    public static Stat operator +(Stat left, int right)
    {
        return left.Current_ + right;
    }

    public static Stat operator -(Stat left, int right)
    {
        return left.Current_ - right;
    }

    public static Stat operator +(Stat left, Stat right)
    {
        return left.current_ + right.Current_;
    }

    public static Stat operator -(Stat left, Stat right)
    {
        return left.Current_ - right.current_;
    }

    public static bool operator ==(Stat left, Stat right)
    {
        return left.Current_ == right.Current_;
    }

    public static bool operator !=(Stat left, Stat right)
    {
        return left.Current_ != right.Current_;
    }
    
    public static bool operator <(Stat left, Stat right)
    {
        return left.Current_ < right.current_;
    }

    public static bool operator >(Stat left, Stat right)
    {
        return left.Current_ > right.Current_;
    }

    public static bool operator >=(Stat left, Stat right)
    {
        return left.Current_ >= right.Current_;
    }

    public static bool operator <=(Stat left, Stat right)
    {
        return left.Current_ <= right.Current_;
    }

    public static bool operator ==(Stat left, int right)
    {
        return left.Current_ == right;
    }

    public static bool operator !=(Stat left, int right)
    {
        return left.Current_ != right;
    }


    public static bool operator <(Stat left, int right)
    {
        return left.Current_ < right;
    }

    public static bool operator >(Stat left, int right)
    {
        return left.Current_ > right;
    }

    public static bool operator >=(Stat left, int right)
    {
        return left.Current_ >= right;
    }

    public static bool operator <=(Stat left, int right)
    {
        return left.Current_ <= right;
    }

    #endregion

    #region comparisoninterfaces
    public int CompareTo(Stat other)
    {
        return Current_.CompareTo(other.Current_);
    }

    public override bool Equals(object obj)
    {
        return obj is Stat && this.Equals((Stat)obj);
    }

    public bool Equals(Stat other)
    {
        return Current_.Equals(other.Current_);
    }

    public override int GetHashCode()
    {
        return Current_.GetHashCode();
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

    /// <summary>
    /// The total count of all stat types.
    /// </summary>
    public static int TypeCount_ { get { return properStatNames_.Length; } }

    public Dictionary<string,int>.Enumerator ModifiersEnumerator_ { get { return modifiers_.GetEnumerator(); } }

    [System.Serializable]
    class ModifiersDict : SerializableDictionary<string, int> { }
}
