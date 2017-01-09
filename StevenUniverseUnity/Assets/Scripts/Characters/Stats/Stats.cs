using UnityEngine;
using System.Collections;
using System.Collections.Generic;


// TODO: Figure out how modifiers are going to work...will these stats be completely
// static and all modifications occur separately? Will each stat have it's own mod
// variable to keep track of modifications? Or maybe a List<Mod> so mods can be added
// and removed by value/comparison

// Should each stat have it's own curve? I'm thinking it might be a better idea to have a few presets IE:
// "Proficient" - Levels somewhat faster early 
// "Terrible" - Levels very slowly and reduces stat's max
// "Natural Born" - Increases stats base and max, reaches max faster
// etc. Then each stat can only pick from a limited set, instead of screwing around with curves.

// Regarding variance - right now all characters given the same stats would be identical at any given level. Need to consider
// how randomization will come in. Random chance of "extra" stats on level up? Via direct changes to max/min? (this would cause
// levelling to change a bit and give different start and end results - might be an easy way to do it).

[ExecuteInEditMode]
public class Stats : MonoBehaviour
{
    public const int MAX_LEVEL_ = 100;
    [SerializeField, Range(1, MAX_LEVEL_)]
    int level_ = 1;

    [SerializeField]
    List<Stat> stats_ = null;
    public IList<Stat> Stats_
    {
        get; private set;
    }

    public Stat testStat_;

    /// <summary>
    /// Vitality is multiplied by this to give base HP.
    /// </summary>
    [SerializeField]
    float vitalityMultiplier_ = 1f;

    void Awake()
    {
        Stats_ = stats_.AsReadOnly();

        //// Assign our array and try to copy old values in case we've changed
        //// our stat types around - we don't want to just lose any previously assigned data if we can avoid it.
        //if (stats_ == null || stats_.Count != Stat.TypeCount_)
        //{
        //    var newList = new List<Stat>();
            
        //}
    }

    public void ModifyCurrent(Stat.Type type, int modifier)
    {
        SetCurrent(type, GetCurrent(type) + modifier);
    }

    public void SetCurrent(Stat.Type type, int val)
    {
        if ((int)type >= stats_.Count)
            return;

        stats_[(int)type] = val;

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
        if (stats_ != null)
            UpdateStats();
    }

    void UpdateHP()
    {
        // Set our HP based on our vitality.
        SetCurrent(Stat.Type.HP, (int)(GetCurrent(Stat.Type.VIT) * vitalityMultiplier_ + minHP_));
    }

    // Update stats, ensure they are clamped into reasonable ranges and find their curve values.
    void UpdateStats()
    {
        for (int i = 0; i < stats_.Count; ++i)
        {
            var type = (Stat.Type)i;

            // These stats aren't touched by the editor
            if (type == Stat.Type.HP || type == Stat.Type.XP ||
                type == Stat.Type.DEF)
            {
                continue;
            }

            //if (type == Stat.Type.LVL)
            //{
            //    stats_[i].Max_ = MaxLevel_;
            //    stats_[i].Base_ = MaxLevel_;
            //    stats_[i].Current_ = Mathf.Clamp(stats_[i].Current_, 1, MaxLevel_);

            //    continue;
            //}

            // Use our stat's properties to ensure they are properly clamped.
           // stats_[i].Max_ = stats_[i].Max_;
            //stats_[i].MaxLevelValue_ = stats_[i].MaxLevelValue_;
            
            //// Get our T value from our current level.
            //float normalizedLevel = Mathf.InverseLerp(1, MAX_LEVEL_, level_);

            //// Find our stat's position on the curve, based on our current level.
            //float normalizedCurveVal = stats_[i].LevellingCurve_.Evaluate(normalizedLevel);

            //// Get our actual value from the curve using our stats min-max range.
            //stats_[i].Current_ = (int)Mathf.Lerp(stats_[i].Max_, stats_[i].MaxLevelValue_, normalizedCurveVal);

        }

        UpdateHP();
    }

    public Stat this[Stat.Type t ]
    {
        get { return stats_[(int)t]; }
        set { stats_[(int)t] = value; }
    }
}
