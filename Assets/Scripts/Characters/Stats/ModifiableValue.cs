using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SUGame.Util.Collections;
using SUGame.PropertyAttributes;


namespace SUGame.Characters
{
    /// <summary>
    /// A value which can have <seealso cref="ValueModifier"/>'s applied and tracks it's 
    /// current/max state based on those modifiers.
    /// </summary>
    [System.Serializable]
    public class ModifiableValue
    {
        [SerializeField]
        string name_;
        public string Name_ { get { return name_; } }

        [SerializeField]
        int level_;
        /// <summary>
        /// Current "Level" for this stat. Determines the stat's <see cref="BaseValue_"/> which in turn determines
        /// the stat's <see cref="MaxValue_"/>.
        /// </summary>
        public int Level_
        {
            get { return level_; }
            set
            {
                SetBaseValueForLevel(value);
                level_ = value;
            }
        }

        [SerializeField]
        AnimationCurve levelingCurve_;
        /// <summary>
        /// Determines a stat's base value at any given level from 1 to <see cref="Stats.MAX_LEVEL_"/>.
        /// <see cref="GetBaseValueAtLevel(int)"/> can be used to retrieve a value.
        /// </summary>
        public AnimationCurve LevelingCurve_ { get { return levelingCurve_; } }

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


        [Header("Stat values")]
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



        [SerializeField, ReadOnly]
        int maxValue_;

        /// <summary>
        /// The maximum possible value of this stat at it's current level including current modifiers.
        /// <see cref="CurrentValue_"/> is clamped from 0 to this.
        /// </summary>
        public int MaxValue_
        {
            get
            {
                return maxValue_;
            }
        }


        [SerializeField, ReadOnly]
        int baseValueUnmodified_;
        /// <summary>
        /// The base value of this stat before modifiers. Set from <seealso cref="SetBaseValueForLevel(int)"/>
        /// </summary>
        public int BaseValueUnmodified_ { get { return baseValueUnmodified_; } }

        /// <summary>
        /// The base value of this stat at a given level after modifiers.
        /// </summary>
        public int BaseValue_ { get { return baseValueUnmodified_ + baseModValue_; } }

        /// <summary>
        /// Base modifiers are added to the base value before percent bonus is applied.
        /// </summary>
        [Header("Modifier values")]
        [SerializeField, ReadOnly]
        int baseModValue_ = 0;

        // TODO : I kept going back and forth on how best to represent this in the editor (0-1 range, 0-100 range).
        //        It seemed unintuitive for a "Double Stat" effect's percent to be 100.
        /// <summary>
        /// Once base modifiers have been applied this percent value is then applied to the result.
        /// 100 means the value will be unmodified, 200 means it will be doubled (assuming no other percent modifiers are in place).
        /// </summary>
        [SerializeField, ReadOnly]
        int bonusPercentValue_ = 100;

        /// <summary>
        /// Modifier applied at the end of final value calculation.
        /// </summary>
        [SerializeField, ReadOnly]
        int flatModValue_ = 0;

        /// <summary>
        /// Modifiers which are applied to a stat's base value to result in <see cref="MaxValue_"/>.
        /// </summary>
        [Space(10)]
        [SerializeField]
        List<ValueModifier> modifiers_ = new List<ValueModifier>();

        public delegate void ModifierCallback(ModifiableValue value, ValueModifier mod);

        /// <summary>
        /// Callback for when a value modifier is applied to this value.
        /// Currently used by MapCharacter to track when a modifier is added
        /// or removed so GameEvents can be broadcast to relevant modifiers based on their 
        /// <seealso cref="ValueModifier.TickType_"/>
        /// </summary>
        public ModifierCallback onModifierAdded_;
        /// <summary>
        /// Callback for when a value modifier is removed from this value.
        /// </summary>
        public ModifierCallback onModifierRemoved_;



        public ModifiableValue(string name)
        {
            name_ = name;
            levelingCurve_ = AnimationCurve.Linear(0, 0, 1, 1);
            minLevelValue_ = 10;
            maxLevelValue_ = 100;
            SetBaseValueForLevel(1);
        }


        // TODO : Use "priority" on stat modifiers, so they only get replaced by higher priority
        // modifiers?
        /// <summary>
        /// Add the given modifier to this stat if valid. Note if a modifier of this type
        /// (name) already exists, it will be replaced.
        /// </summary>
        public void AddModifier(ValueModifier modifier)
        {
            int i = modifiers_.IndexOf(modifier);

            //Debug.LogFormat("Mod {0} applied to {1}, old Value: {2}. ModValue: {3}", modifier.name_, this.name_, CurrentValue_, modifier.ModValue_);

            if (i < 0)
            {
                modifiers_.Add(modifier);
            }
            else
            {
                modifiers_[i] = modifier;
            }

            RefreshModifiers();
            UpdateValues();

            //Debug.LogFormat("Mod {0} applied to {1}, new Value: {2}", modifier.name_, this.name_, CurrentValue_);

            // Map characters use this to forward game events to modifiers so they will be ticked properly (see ValueModifier.Tick)
            onModifierAdded_.Invoke(this, modifier);
            // When the modifier ticks reach zero it will be removed.
            modifier.onExpired_ += RemoveModifier;
        }

        public void RemoveModifier(ValueModifier mod)
        {
            Debug.LogFormat("Removing modifier {0}", mod.name_);
            int i = modifiers_.IndexOf(mod);
            if (i < 0)
                return;

            modifiers_.RemoveAt(i);
            RefreshModifiers();
            UpdateValues();

            onModifierRemoved_(this, mod);
        }

        /// <summary>
        /// Clears current mod values and rebuilds.
        /// This should be called any time a modification is added or removed.
        /// </summary>
        void RefreshModifiers()
        {
            baseModValue_ = 0;
            flatModValue_ = 0;
            bonusPercentValue_ = 100;

            foreach (var mod in modifiers_)
            {
                switch (mod.modifierType_)
                {
                    case ValueModifier.Type.BASE_MODIFIER:
                        baseModValue_ += mod.ModValue_;
                        break;
                    case ValueModifier.Type.BONUS_PERCENT_MODIFIER:
                        bonusPercentValue_ += (mod.ModValue_ - 100);
                        break;
                    case ValueModifier.Type.FLAT_MODIFIER:
                        flatModValue_ += mod.ModValue_;
                        break;
                }
            }
        }

        /// <summary>
        /// Update the stat's current and max values, taking modifiers into account.
        /// </summary>
        void UpdateValues()
        {
            int diff = MaxValue_ - CurrentValue_;

            int modifiedBaseValue = baseValueUnmodified_ + baseModValue_;
            // Apply bonus percent
            modifiedBaseValue = Mathf.RoundToInt((float)modifiedBaseValue * (float)bonusPercentValue_ / 100f);
            int val = modifiedBaseValue + flatModValue_;
            maxValue_ = Mathf.Clamp(val, 0, int.MaxValue);

            CurrentValue_ = MaxValue_ - diff;
        }

        /// <summary>
        /// Should be called from encompassing monobehaviour.
        /// </summary>
        public void OnValidate()
        {
            // Ensure values remain clamped and update from any editor changes.
            Level_ = level_;
            MinLevelValue_ = minLevelValue_;
            MaxLevelValue_ = maxLevelValue_;
            CurrentValue_ = currentValue_;
            RefreshModifiers();
            UpdateValues();

        }

        /// <summary>
        /// Sets the stat's base value for the given level according to the <seealso cref="LevelingCurve_"/>
        /// </summary>
        /// <param name="level">The level of the value we want.</param>
        void SetBaseValueForLevel(int level)
        {
            baseValueUnmodified_ = GetBaseValueAtLevel(level);
            UpdateValues();
        }

        /// <summary>
        /// Retrieve the base value for this stat at the given level based on the leveling curve and the stat range.
        /// </summary>
        public int GetBaseValueAtLevel(int level)
        {
            // Get our T value from our current level.
            float normalizedLevel = Mathf.InverseLerp(1, Stats.MAX_LEVEL_, level);

            // Find our stat's position on the curve, based on the given level.
            float normalizedCurveVal = LevelingCurve_.Evaluate(normalizedLevel);

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

        public static implicit operator int (ModifiableValue stat)
        {
            return stat.CurrentValue_;
        }

        public static int operator +(ModifiableValue left, int right)
        {
            return left.CurrentValue_ + right;
        }

        public static int operator -(ModifiableValue left, int right)
        {
            return left.CurrentValue_ - right;
        }

        public static int operator +(ModifiableValue left, ModifiableValue right)
        {
            return left.currentValue_ + right.CurrentValue_;
        }

        public static int operator -(ModifiableValue left, ModifiableValue right)
        {
            return left.CurrentValue_ - right.currentValue_;
        }

        public static bool operator ==(ModifiableValue left, ModifiableValue right)
        {
            return left.CurrentValue_ == right.CurrentValue_;
        }

        public static bool operator !=(ModifiableValue left, ModifiableValue right)
        {
            return left.CurrentValue_ != right.CurrentValue_;
        }

        public static bool operator <(ModifiableValue left, ModifiableValue right)
        {
            return left.CurrentValue_ < right.currentValue_;
        }

        public static bool operator >(ModifiableValue left, ModifiableValue right)
        {
            return left.CurrentValue_ > right.CurrentValue_;
        }

        public static bool operator >=(ModifiableValue left, ModifiableValue right)
        {
            return left.CurrentValue_ >= right.CurrentValue_;
        }

        public static bool operator <=(ModifiableValue left, ModifiableValue right)
        {
            return left.CurrentValue_ <= right.CurrentValue_;
        }

        public static bool operator ==(ModifiableValue left, int right)
        {
            return left.CurrentValue_ == right;
        }

        public static bool operator !=(ModifiableValue left, int right)
        {
            return left.CurrentValue_ != right;
        }


        public static bool operator <(ModifiableValue left, int right)
        {
            return left.CurrentValue_ < right;
        }

        public static bool operator >(ModifiableValue left, int right)
        {
            return left.CurrentValue_ > right;
        }

        public static bool operator >=(ModifiableValue left, int right)
        {
            return left.CurrentValue_ >= right;
        }

        public static bool operator <=(ModifiableValue left, int right)
        {
            return left.CurrentValue_ <= right;
        }

        #endregion

        #region comparisoninterfaces
        public int CompareTo(ModifiableValue other)
        {
            return CurrentValue_.CompareTo(other.CurrentValue_);
        }

        public override bool Equals(object obj)
        {
            return obj is ModifiableValue && this.Equals((ModifiableValue)obj);
        }

        public bool Equals(ModifiableValue other)
        {
            return CurrentValue_.Equals(other.CurrentValue_);
        }

        public override int GetHashCode()
        {
            return CurrentValue_.GetHashCode();
        }
        #endregion



    }
}