using UnityEngine;
using System.Collections;
using System.Collections.Generic;



namespace SUGame.Characters
{
    /// <summary>
    /// Stats for characters.
    /// Note that stats are maintained in a list to make it easy to reference them programmatically.
    /// </summary>
    //[ExecuteInEditMode]
    [System.Serializable]
    public class Stats
    {
        public const int MAX_LEVEL_ = 100;
        [SerializeField, Range(1, MAX_LEVEL_)]
        int level_ = 1;
        public int Level_ { get { return level_; } }

        [SerializeField]
        List<ModifiableValue> primaryStats_ = new List<ModifiableValue>();
        /// <summary>
        /// List of primary stats for this character, where each index of the list maps to
        /// <see cref="PrimaryStat"/>
        /// </summary>
        public IList<ModifiableValue> PrimaryStats_
        {
            get; private set;
        }

        //[SerializeField]
        //List<ModifiableValue> derivedStats_ = new List<ModifiableValue>();
        //public IList<ModifiableValue> DerivedStats_
        //{
        //    get; private set;
        //}

        /// <summary>
        /// Returns the character's hit chance with the given weapon.
        /// </summary>
        public int GetHitChance(Weapon weapon)
        {
            int halfLuck = Mathf.RoundToInt(this[PrimaryStat.LCK] / 2f);
            int doubleSkill = this[PrimaryStat.SKL] * 2;
            int weaponMod = weapon == null ? 0 : weapon.Accuracy_;
            return Mathf.Clamp(weaponMod + doubleSkill + halfLuck, 0, int.MaxValue);
        }

        public int GetCritChance(Weapon weapon)
        {
            // FE Formula: Crit Chance = Weapon Crit + Skill / 2 + Bond Bonus + Class Crit - Enemy Luck
            int halfSkill = Mathf.RoundToInt(this[PrimaryStat.SKL] / 2f);
            int weaponMod = weapon == null ? 0 : weapon.CritChance_;
            return Mathf.Clamp(weaponMod + halfSkill, 0, int.MaxValue);
        }

        [SerializeField]
        int experience_ = 0;
        public int Experience_ { get { return experience_; } }


        public Stats()
        {
            OnValidate();
        }

        public void Awake()
        {
            PrimaryStats_ = primaryStats_.AsReadOnly();
            //DerivedStats_ = derivedStats_.AsReadOnly();
            OnValidate();
        }

        /// <summary>
        /// Should be called from encompassing monobehaviour
        /// </summary>
        public void OnValidate()
        {

            // Ensure our stat list matches our enum
            if (primaryStats_ == null || primaryStats_.Count != Stats.PrimaryStatCount_)
            {
                primaryStats_ = new List<ModifiableValue>();
                foreach (var sName in primaryStatNames_)
                {
                    primaryStats_.Add(new ModifiableValue(sName));
                }
            }

            //if( derivedStats_ == null || derivedStats_.Count != derivedStatNames_.Length )
            //{
            //    derivedStats_ = new List<ModifiableValue>();
            //    foreach( var sName in derivedStatNames_ )
            //    {
            //        derivedStats_.Add(new ModifiableValue(sName));
            //    }
            //}

            foreach (var s in primaryStats_)
            {
                s.Level_ = level_;
                s.OnValidate();
            }
        }

        public ModifiableValue this[PrimaryStat t]
        {
            get { return primaryStats_[(int)t]; }
            set { primaryStats_[(int)t] = value; }
        }

        //public ModifiableValue this[Derived t ]
        //{
        //    get { return derivedStats_[(int)t]; }
        //    set { derivedStats_[(int)t] = value; }
        //}

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
            if (primaryStats_ == null)
                return;

            foreach (var s in primaryStats_)
            {
                s.Level_ = level_;
            }
        }


        public enum PrimaryStat
        {
            //LVL = 0,
            HP = 0,
            STR = 1,
            MAG = 2,
            SKL = 3,
            SPD = 4,
            LCK = 5,
            DEF = 6,
            RES = 7,
            //ATK = 9,  <-- These are derived from other stats, but we still want to use modifiers with them
            //   So we can still keep them as ModifiableValues but they should be separate from "proper" stats
            //HIT = 10,
            //CRT = 11,
            //AVO = 12,
            //EXP = 13,
        };

        //public enum Derived
        //{
        //    ATK = 8,
        //    HIT = 9,
        //    CRT = 10,
        //    AVO = 11,
        //};

        /// <summary>
        /// Get the proper name for the given stat.
        /// </summary>
        public static string GetPrimaryStatName(PrimaryStat type)
        {
            return primaryStatNames_[(int)type];
        }

        public static readonly string[] primaryStatNames_ = new string[]
        {
        //"Level",
        "Hit Points", // 1
        "Strength",   // 2
        "Magic Power",      // 3
        "Skill",      // 4
        "Speed",      // 5
        "Luck",       // 6
        "Defense",    // 7
        "Resistance", // 8
                      //"Attack Power", // 9 
                      //"Hit Chance",   // 10
                      //"Critical Chance", // 11
                      //"Dodge Change",    // 12
        };

        //public static readonly string[] derivedStatNames_ = new string[]
        //{
        //    "Attack Power", // 0
        //    "Hit Chance",   // 1
        //    "Critical Chance", // 2
        //    "Dodge Change",    // 3
        //};

        /// <summary>
        /// The total count of all stat types.
        /// </summary>
        public static int PrimaryStatCount_ { get { return primaryStatNames_.Length; } }
    }

}