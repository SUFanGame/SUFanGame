﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SUGame.StrategyMap;
using SUGame.StrategyMap.UI.CombatPanelUI;

namespace SUGame.Characters.Skills
{
    /// <summary>
    /// Skills which trigger in combat.
    /// </summary>
    public abstract class CombatSkill : Skill
    {
        [SerializeField]
        GameEvent triggerEvent_;
        public GameEvent TriggerEvent_ { get { return triggerEvent_; } }

        [SerializeField]
        bool showLabel_;
        /// <summary>
        /// Whether or not this skill will show it's label when it occurs in combat.
        /// </summary>
        public bool ShowLabel_ { get { return showLabel_; } }
        

        /// <summary>
        /// Optional coroutine to do something UI related during combat encounter animations.
        /// </summary>
        public virtual IEnumerator UIRoutine(CombatPanelUnit uiPanel)
        {
            yield return null;
        }

        //public abstract void Execute();

        public virtual void Execute(Combat combat)
        {
        }

        // [SerializeField]
        // protected List<CombatEffect> effects_ = new List<CombatEffect>();
    }
}
