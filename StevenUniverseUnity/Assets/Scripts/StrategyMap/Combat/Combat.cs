using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace SUGame.StrategyMap
{
    public class Combat
    {
        MapCharacter attacker_;
        MapCharacter defender_;

        public Combat( MapCharacter attacker, MapCharacter defender )
        {
            attacker_ = attacker;
            defender_ = defender;
        }
        
        public class Phase
        {
            public void OnPreAttack( MapCharacter attacker, MapCharacter defender, Weapon weapon )
            {

            }

            public void OnAttack()
            {

            }

            public void OnPostAttack()
            {

            }
        }
    }

}