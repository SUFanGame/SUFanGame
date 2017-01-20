using UnityEngine;

namespace SUGame.Characters
{


    /// <summary>
    /// A modifier which can be applied to a <seealso cref="ModifiableValue"/>.
    /// Modifiers are equated by name only.
    /// FinalValue = ( (baseValue + baseModifiers) * (Percent modifiers added up)) + (Flat modifiers added up)
    /// </summary>
    [System.Serializable]
    public class ValueModifier : System.IEquatable<ValueModifier>, System.IComparable<ValueModifier>
    {
        public string name_;
        // Old, still may want to use this in some way? IE: To override certain modifiers of the same "type" (name)
        //public int priority_ = 0;

        /// <summary>
        /// The type of this modifier. Determines in what way it is applied to the value,
        /// see <seealso cref="Type"/>
        /// </summary>
        public Type modifierType_ = Type.BASE_MODIFIER;

        [SerializeField]
        int modValue_ = 0;
        /// <summary>
        /// The final amount by which the value is modified.
        /// </summary>
        public int ModValue_ { get { return modValue_; } }


        [SerializeField]
        GameEvent tickType_ = GameEvent.EVT_PRE_COMBAT;
        /// <summary>
        /// Determines which game event causes this modifier to tick.
        /// See <seealso cref="TicksRemaining_"/>
        /// </summary>
        public GameEvent TickType_ { get { return tickType_; } }


        [SerializeField]
        int ticksRemaining_ = 1;
        /// <summary>
        /// When this reaches 0 the modifier will be removed from the
        /// value.
        /// </summary>
        public int TicksRemaining_ { get { return ticksRemaining_; } }

        /// <summary>
        /// Callback invoked when <seealso cref="TicksRemaining_"/> reaches 0.
        /// <seealso cref="ModifiableValue"/>s use this to remove modifiers that have ticked to 0.
        /// </summary>
        public System.Action<ValueModifier> onExpired_;

        public ValueModifier(string name)
        {
            name_ = name;
        }

        public ValueModifier(string name, Type type) : this(name)
        {
            modifierType_ = type;
        }

        public ValueModifier(string name, Type type, ModifiableValue stat) : this(name, type)
        {
        }

        public ValueModifier(ValueModifier other)
        {
            name_ = other.name_;
            modifierType_ = other.modifierType_;
            modValue_ = other.modValue_;
            ticksRemaining_ = other.ticksRemaining_;
            tickType_ = other.tickType_;
        }

        /// <summary>
        /// Tick the modifier, decrementing it's ticks remaining.
        /// When <seealso cref="TicksRemaining_"/> reaches 0, the modifier
        /// will expire and be removed from the value it's modifying via <seealso cref="onExpired_"/>,
        /// see <seealso cref="ModifiableValue.AddModifier(ValueModifier)"/>.
        /// This must be called from an outside source, as there is no inbuilt functionality
        /// for ticking modifiers.
        /// </summary>
        public void Tick()
        {
            //Debug.Log("Ticking " + name_);
            ticksRemaining_--;
            if (ticksRemaining_ <= 0)
            {
                //Debug.LogFormat("{0} has expiered", name_);
                onExpired_(this);
            }
        }

        public bool Equals(ValueModifier other)
        {
            return other != null && !string.IsNullOrEmpty(other.name_) && name_.Equals(other.name_);
        }

        public override bool Equals(object other)
        {
            var o = other as ValueModifier;
            if (o == null)
                return false;
            return name_.Equals(o);
        }

        public override int GetHashCode()
        {
            return name_.GetHashCode();
        }

        // Only useful if priority is used in some way
        public int CompareTo(ValueModifier other)
        {
            // Modifiers are compared by type then by priority.
            //int type = (int)modifierType_;
            //int otherType = (int)other.modifierType_;
            //if (type == otherType)
            //    return priority_.CompareTo(other.priority_);
            //return type.CompareTo(otherType);
            return 0;
        }

        /// <summary>
        /// The type of modifier.
        /// <seealso cref="BASE_MODIFIER"/>s are added to the base value BEFORE percent bonuses.
        /// All <seealso cref="BONUS_PERCENT_MODIFIER"/>s are added together than applied to the modified base value.
        /// Finally <seealso cref="FLAT_MODIFIER"/>s are added at the end, only affecting the final value. 
        /// </summary>
        public enum Type
        {
            BASE_MODIFIER,
            BONUS_PERCENT_MODIFIER,
            FLAT_MODIFIER,
        };

    }
}