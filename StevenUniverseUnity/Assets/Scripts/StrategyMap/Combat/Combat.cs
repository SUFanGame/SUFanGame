using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SUGame.Characters.Skills;

namespace SUGame.StrategyMap
{
    public class Combat
    {
        MapCharacter attacker_;
        public MapCharacter Attacker_ { get { return attacker_; } }

        MapCharacter defender_;
        public MapCharacter Defender_ { get { return defender_; } }

        List<CombatSkill> attackerSkills_ = new List<CombatSkill>();
        List<CombatSkill> defenderSkills_ = new List<CombatSkill>();

        //List<Phase> phases_ = new List<Phase>();

        public Combat( MapCharacter attacker, MapCharacter defender )
        {
            attacker_ = attacker;
            defender_ = defender;
        }

        void Initialize()
        {
            attackerSkills_.Clear();
            attacker_.Data.GetSkills(attackerSkills_);

            defenderSkills_.Clear();
            defender_.Data.GetSkills(defenderSkills_);

        }

        public IEnumerator Resolve()
        {


            yield return null;
        }

        public IEnumerator DoPhaseSkills( Phase phase )
        {
            foreach( var s in attackerSkills_ )
            {
                if( s.Phase_ == phase )
                {
                    s.Execute(this);
                }
            }

            foreach( var s in defenderSkills_ )
            {
                if( s.Phase_ == phase )
                {
                    s.Execute(this);
                }
            }

            yield return null;
        }
        

        public enum Phase
        {
            PRE_COMBAT,
            PRE_ATTACK,
            ON_MISS,
            ON_HIT,
            ON_KILL,
            ON_KILLED,
            POST_ATTACK,
        };
    }

}