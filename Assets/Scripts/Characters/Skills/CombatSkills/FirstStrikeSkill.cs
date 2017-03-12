using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SUGame.Characters.Skills;
using SUGame.StrategyMap;

namespace SUGame.Characters.Skills
{
    [CreateAssetMenu(fileName = "First Strike", menuName = "Skills/Combat/First Strike", order = 9001)]
    public class FirstStrikeSkill : CombatSkill
    {
        protected override void OnExecute(Combat combat)
        {
            combat.ClearAttacks();
            combat.AddAttack(combat.Defender_, combat.Attacker_);
            combat.AddAttack(combat.Attacker_, combat.Defender_);
        }
    }
}