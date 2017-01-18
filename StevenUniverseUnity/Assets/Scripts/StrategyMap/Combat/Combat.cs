using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SUGame.Characters.Skills;
using SUGame.Util.Common;
using SUGame.Util.Logic.States;
using SUGame.StrategyMap.Characters.Actions.UIStates;
using SUGame.Util;

namespace SUGame.StrategyMap
{
    // TODO: Might want to do a bunch of pooling, probably not necessary
   
    
    /// <summary>
    /// Initializes and resolves a combat encounter between two map characters.
    /// </summary>
    public class Combat
    {
        MapCharacter attacker_;
        public MapCharacter Attacker_ { get { return attacker_; } }

        MapCharacter defender_;
        public MapCharacter Defender_ { get { return defender_; } }

        StateMachine uiStateMachine_;

        bool cancelled_ = false;

        
        // Combat consists of rounds where units take turns attacking eachother.
        // Often the attack order or number of attacks can vary. 
        // All attack rounds will be determined before combat begins,
        // then they will occur in order. This should allow us to account for skills that 
        // change combat order, as well as allow combat to be instantly resolved if the 
        // user cancels during combat animations.
        List<AttackRound> rounds_ = new List<AttackRound>();

        // RE: Combat events
        // it's not possible for combat to send messages at specific times since
        // some event times (ie: when a "physical" hit occurs during an animation)
        // are tied to each unit's individual animation.

        // Combat events are generated into a queue as combat is processed,
        // then the UI animation can process that queue in order to play the 
        // combat out visually.
        // Of course this means that combat has already ended by the time the queue is
        // populated. We'll have to account for that on the UI side or maybe tweak this a bit so
        // things happen "concurrently" between this and UI. Not sure yet which would be easier.
        List<CombatEvent> combatEvents_ = new List<CombatEvent>();
        public IList<CombatEvent> CombatEvents_ { get; private set; }

        public Combat(MapCharacter attacker, MapCharacter defender)
        {
            attacker_ = attacker;
            defender_ = defender;
            CombatEvents_ = combatEvents_.AsReadOnly();
        }

        /// <summary>
        /// Initialize a typical round of combat.
        /// </summary>
        public void Initialize()
        {
            AddAttack(attacker_, defender_);
            AddAttack(defender_, attacker_);
        }

        public void ClearAttacks()
        {
            rounds_.Clear();
        }

        public void AddAttack(MapCharacter attacker, MapCharacter defender)
        {
            var weapon = attacker.Data.EquippedWeapon_;
            if (weapon == null)
            {
                Debug.LogWarningFormat("Attempting to intiate with {0} but their equipped weapon is null", attacker.name);
                return;
            }

            IntVector2 attackerPos = (IntVector2)attacker.GridPosition;
            IntVector2 defenderPos = (IntVector2)defender.GridPosition;

            // Ensure the target is within range.
            if (IntVector2.ManhattanDistance(attackerPos, defenderPos) > weapon.Range_)
            {
                Debug.LogFormat("Attempted to add an attack in combat, but {0} is out of range of {1}", attacker.name, defender.name);
                return;
            }

            rounds_.Add(new AttackRound(attacker, defender));
        }


        /// <summary>
        /// Resolve this combat encounter. Once this is called the combat will resolve
        /// instantly and <seealso cref="CombatEvents_"/> should be processed by the Combat UI
        /// to replay the events.
        /// </summary>
        public void Resolve()
        {
            DoPhaseSkills(GameEvent.EVT_PRE_COMBAT, attacker_);
            DoPhaseSkills(GameEvent.EVT_PRE_COMBAT, defender_);

            foreach (var round in rounds_)
            {
                if (cancelled_)
                    break;

                DoAttack(round);
            }

            DoPhaseSkills(GameEvent.EVT_POST_COMBAT, attacker_);
            DoPhaseSkills(GameEvent.EVT_POST_COMBAT, defender_);

            // Signal for combat to end.
            combatEvents_.Add(new CombatEvent(CombatEventType.END, 0));

            ClearAttacks();

           // yield return null;
        }

        public static int GetDamage( MapCharacter att, MapCharacter defender, Weapon weapon )
        {
            int weaponDmg = weapon.Damage_;
            int str = att.Data.Stats_[Stats.PrimaryStat.STR];
            int def = defender.Data.Stats_[Stats.PrimaryStat.DEF];
            return Mathf.Clamp(((weaponDmg + str) - def), 0, int.MaxValue);
        }

        public static int GetHitChance( MapCharacter att, MapCharacter def, Weapon weapon )
        {
            int halfLuck = Mathf.RoundToInt(def.Data.Stats_[Stats.PrimaryStat.LCK] / 2f);
            return Mathf.Clamp(att.Data.Stats_.GetHitChance(weapon) - halfLuck, 0,  100);
        }

        public static int GetCritChance( MapCharacter att, MapCharacter def, Weapon weapon )
        {
            return Mathf.Clamp(att.Data.Stats_.GetCritChance(weapon) - def.Data.Stats_[Stats.PrimaryStat.LCK], 0, int.MaxValue);
        }

        /// <summary>
        /// Go through an attack round for a single unit.
        /// </summary>
        void DoAttack(AttackRound round)
        {
            //Debug.Log("Doing attack round");
            // Note we have to use round.attacker/round.defender instead of the Combat member variables
            // defender_ and attacker_. 
            DoPhaseSkills(GameEvent.EVT_PRE_ATTACK, round.attacker_);
            
            var weapon = round.attacker_.Data.EquippedWeapon_;
            var attackerStats = round.attacker_.Data.Stats_;
            var defenderStats = round.defender_.Data.Stats_;

            int hitChance = GetHitChance(round.attacker_, round.defender_, weapon);

            int critChance = GetCritChance(round.attacker_, round.defender_, weapon);

            bool hit = RandUtil.Chance100(hitChance);
            int critMod = RandUtil.Chance100(critChance) ? 2 : 1;
            int damage = GetDamage(round.attacker_, round.defender_, weapon) * critMod;



            if (hit)
            {
                string critMessage = critMod == 1 ? "" : "It was a critical hit!";
                // We hit!
                Debug.LogFormat("{0} hit {1} for {2} damage with a hit chance of {3}. {4}",
                    round.attacker_.name,
                    round.defender_.name,
                    damage,
                    hitChance,
                    critMessage
                    );
                defenderStats[Stats.PrimaryStat.HP].CurrentValue_ -= damage;
                DoPhaseSkills(GameEvent.EVT_DEALT_DAMAGE, round.attacker_);
                DoPhaseSkills(GameEvent.EVT_TOOK_DAMAGE, round.defender_);

                var evtType = critMod == 1 ? CombatEventType.HIT : CombatEventType.CRIT;
                combatEvents_.Add(new CombatEvent(evtType, Side(round.attacker_), damage));

            }
            else
            {
                // We missed...
                Debug.LogFormat("{0} missed {1} with a hit chance of {2}.", round.attacker_.name, round.defender_.name, hitChance);
                DoPhaseSkills(GameEvent.EVT_MISS, round.attacker_);
                combatEvents_.Add(new CombatEvent(CombatEventType.MISS, Side(round.attacker_)));
            }

            DoPhaseSkills(GameEvent.EVT_POST_ATTACK, round.attacker_);
        }

        public void DoPhaseSkills(GameEvent phase, MapCharacter character)
        {

            // Tick game event for this character.
            if (character.EventCallbacks_[(int)phase] != null)
                character.EventCallbacks_[(int)phase]();

            if (character.Data.CombatSkills_ == null)
            {
                //Debug.LogFormat("{0}'s Combat skills is null", character.name);
                //yield break;
                return;
            }

            //Debug.LogFormat("{0} has some combat skills", character.name);
            foreach (var s in character.Data.CombatSkills_)
            {
                if (s.TriggerEvent_ == phase)
                {
                    Debug.LogFormat("Executing {0}'s skill {1} in phase {2}", attacker_.name, s.name, phase);

                    combatEvents_.Add(new CombatEvent( CombatEventType.SKILL, Side(character), s, phase )) ;

                    s.Execute(this);
                }
            }

        }

        /// <summary>
        /// Which side of the combat animation the character is on ( left or right ).
        /// </summary>
        int Side(MapCharacter c)
        {
            return c == attacker_ ? 0 : 1;
        }
        
        struct AttackRound
        {
            public MapCharacter attacker_;
            public MapCharacter defender_;
            public AttackRound( MapCharacter a, MapCharacter d )
            {
                attacker_ = a;
                defender_ = d;
            }
        };

        /// <summary>
        /// Event data used for processing events in UI animations
        /// </summary>
        public class CombatEvent
        {
            public CombatEventType type_;
            public int side_;
            public int dmg_;
            public CombatSkill skill_;
            public GameEvent event_;

            public CombatEvent(CombatEventType t, int side ) { type_ = t; side_ = side; }
            public CombatEvent( CombatEventType t, int side, int damage ) : this( t, side )
            {
                dmg_ = damage;
            }
            public CombatEvent( CombatEventType t, int side, CombatSkill s, GameEvent e ) : this( t, side )
            {
                skill_ = s;
                event_ = e;
            }
        };
        
        /// <summary>
        /// Events that occured during combat, used during UI animations.
        /// </summary>
        public enum CombatEventType
        {
            HIT,
            CRIT,
            MISS,
            DIED,
            SKILL,
            END
        };

    }

}