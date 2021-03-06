﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SUGame.StrategyMap;


namespace SUGame.Characters.Effects
{
    /// <summary>
    /// Modify a stat in some way
    /// </summary>
    [CreateAssetMenu(fileName = "Modify Stat", menuName = "Skills/Effects/Modify Stat", order = 9000)]
    public class ModifyStatEffect : SkillEffect
    {
        [SerializeField]
        Stats.PrimaryStat stat_ = Stats.PrimaryStat.STR;

        [SerializeField]
        ValueModifier valueModifier_ = new ValueModifier("Modify Stat");

        public virtual void Execute(Stats stats)
        {
            // Note we have to allocate a new modifier, since the modifier contains unique
            // state ( tick count ) we don't want to share it among different values.
            var mod = new ValueModifier(valueModifier_);
            stats[stat_].AddModifier(mod);
        }



    }
}