using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SUGame.Characters.Skills;
using SUGame.Util.Common;
using SUGame.Util.Logic.States;
using SUGame.StrategyMap.Characters.Actions.UIStates;
using SUGame.Util;
using SUGame.StrategyMap.UI.CombatPanelUI;
using SUGame.Characters;

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


        public delegate IEnumerator UIWaitForEventFunc(CombatAnimEvent evt);
        public delegate void UIAnimCallback(string anim, int side);
        public delegate IEnumerator UIHandleSkill(CombatSkill skill, int side);

        /// <summary>
        /// This coroutine can be yielded to in order to wait for certain
        /// animation events to occur. It's intended for control to returned to
        /// the combat function once the given combat event is triggered from the UI.
        /// </summary>
        UIWaitForEventFunc uiWaitForEvent_;

        /// <summary>
        /// This action should be called to trigger certain animations on the UI side.
        /// </summary>
        UIAnimCallback uiPlayAnimation_;

        /// <summary>
        /// This coroutine can be yielded to to allow the UI to handle a to
        /// handle a triggered skill in some way, IE: Showing a label for the skill.
        /// </summary>
        UIHandleSkill uiHandleSkill_;

        bool UIActive_ { get { return uiHandleSkill_ != null && uiPlayAnimation_ != null && uiWaitForEvent_ != null; } }

        
        // Combat consists of rounds where units take turns attacking eachother.
        // Often the attack order or number of attacks can vary. 
        // All attack rounds will be determined before combat begins,
        // then they will occur in order. This should allow us to account for skills that 
        // change combat order, as well as allow combat to be instantly resolved if the 
        // user cancels during combat animations.
        List<AttackRound> rounds_ = new List<AttackRound>();

        public Combat(
            MapCharacter attacker, 
            MapCharacter defender, 
            UIWaitForEventFunc uiAnimEventRoutine = null,
            UIAnimCallback uiAnimCallback = null,
            UIHandleSkill uiHandleSkill = null )
        {
            attacker_ = attacker;
            defender_ = defender;
            uiWaitForEvent_ += uiAnimEventRoutine;
            uiPlayAnimation_ = uiAnimCallback;
            uiHandleSkill_ = uiHandleSkill;
            Initialize();
        }

        public void SetUICallbacks(CombatPanel panel)
        {
            uiWaitForEvent_ += CombatPanel.WaitForAnimEvent;
            uiPlayAnimation_ += CombatPanel.PlayAnim;
            uiHandleSkill_ += CombatPanel.HandleSkillTrigger;
        }

        /// <summary>
        /// Initialize a typical round of combat.
        /// </summary>
        void Initialize()
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
        public IEnumerator Resolve()
        {
            //Debug.Log("Resolving combat");
            yield return DoPhaseSkills(GameEvent.EVT_PRE_COMBAT, attacker_);
            yield return DoPhaseSkills(GameEvent.EVT_PRE_COMBAT, defender_);

            foreach (var round in rounds_)
            {
                if (cancelled_)
                    break;

                yield return DoAttack(round);
            }

            yield return DoPhaseSkills(GameEvent.EVT_POST_COMBAT, attacker_);
            yield return DoPhaseSkills(GameEvent.EVT_POST_COMBAT, defender_);
            

            ClearAttacks();

            yield return null;
        }

        /// <summary>
        /// Insta-kills for use in cutscenes
        /// </summary>
        /// <returns></returns>
        public IEnumerator QuickResolve()
        {
            
            var defenderStats = defender_.Data.Stats_;

            if (uiPlayAnimation_ == null && uiWaitForEvent_ != null ||
                uiPlayAnimation_ != null && uiWaitForEvent_ == null)
            {
                Debug.LogErrorFormat("Error in combat routine, ui callbacks must either both be null or both be valid to function properly");
            }


            if (UIActive_)
            {
                // Play the attack animation and wait for our pre attack event
                uiPlayAnimation_("Attack", 0);
                yield return uiWaitForEvent_(CombatAnimEvent.PRE_ATTACK);
            }

            int damage = 80;

            if (UIActive_)
            {
                // Wait for the attack hit event
                yield return uiWaitForEvent_(CombatAnimEvent.ATTACK_HIT);
            }
            
            Debug.Log("Instakill!");
            defenderStats[Stats.PrimaryStat.HP].CurrentValue_ -= damage;
            Debug.LogFormat("{0} was killed!", defender_.name);
            
            if (UIActive_)
            {
                uiPlayAnimation_("Killed", 1);
            }

            if (UIActive_)
            {
                // Wait for our attack animation to complete
                yield return uiWaitForEvent_(CombatAnimEvent.ANIM_COMPLETE);
            }

            //yield return null;
        }

        public static int GetDamage( MapCharacter att, MapCharacter defender, Weapon weapon )
        {
            int weaponDmg = weapon == null ? 0 : weapon.Damage_;
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
        IEnumerator DoAttack(AttackRound round)
        {
            if( !round.attacker_.IsAlive() )
            {
                yield break;
            }

            //Debug.Log("Doing attack round");
            // Note we have to use round.attacker/round.defender instead of the Combat member variables
            // defender_ and attacker_. 
            int attackerSide = round.attacker_ == attacker_ ? 0 : 1;
            int defenderSide = 1 - attackerSide;

            if( uiPlayAnimation_ == null && uiWaitForEvent_ != null ||
                uiPlayAnimation_ != null && uiWaitForEvent_ == null )
            {
                Debug.LogErrorFormat("Error in combat routine, ui callbacks must either both be null or both be valid to function properly");
            }
            

            if ( UIActive_ )
            {
                // Play the attack animation and wait for our pre attack event
                uiPlayAnimation_("Attack", attackerSide);
                yield return uiWaitForEvent_(CombatAnimEvent.PRE_ATTACK);
            }

            yield return DoPhaseSkills(GameEvent.EVT_PRE_ATTACK, round.attacker_);

            var weapon = round.attacker_.Data.EquippedWeapon_;
            var defenderStats = round.defender_.Data.Stats_;

            int hitChance = GetHitChance(round.attacker_, round.defender_, weapon);

            int critChance = GetCritChance(round.attacker_, round.defender_, weapon);

            bool hit = RandUtil.Chance100(hitChance);
            int critMod = RandUtil.Chance100(critChance) ? 2 : 1;
            int damage = GetDamage(round.attacker_, round.defender_, weapon) * critMod;


            if (UIActive_)
            {
                // Wait for the attack hit event
                yield return uiWaitForEvent_(CombatAnimEvent.ATTACK_HIT);
            }
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

                yield return DoPhaseSkills(GameEvent.EVT_DEALT_DAMAGE, round.attacker_);
                yield return DoPhaseSkills(GameEvent.EVT_TOOK_DAMAGE, round.defender_);

                if (!round.defender_.IsAlive())
                {
                    Debug.LogFormat("{0} was killed!", round.defender_.name);
                    yield return DoPhaseSkills(GameEvent.EVT_KILLED, round.defender_);
                    if( UIActive_ )
                    {
                        uiPlayAnimation_("Killed", defenderSide);
                    }
                }
                

            }
            else
            {
                // We missed...
                Debug.LogFormat("{0} missed {1} with a hit chance of {2}.", round.attacker_.name, round.defender_.name, hitChance);
                yield return DoPhaseSkills(GameEvent.EVT_MISS, round.attacker_);
            }

            if( UIActive_ )
            {
                // Wait for our attack animation to complete
                yield return uiWaitForEvent_(CombatAnimEvent.ANIM_COMPLETE);
            }

            yield return DoPhaseSkills(GameEvent.EVT_POST_ATTACK, round.attacker_);

        }

        public IEnumerator DoPhaseSkills(GameEvent phase, MapCharacter character)
        {
            bool uiActive = UIActive_;
            //Debug.LogFormat("Doing phase {0}", phase);
            // Tick game event for this character.
            if (character.EventCallbacks_[(int)phase] != null)
                character.EventCallbacks_[(int)phase]();

            if (character.Data.CombatSkills_ == null)
            {
                //Debug.LogFormat("{0}'s Combat skills is null", character.name);
                yield break;
                //return;
            }
           
            //Debug.LogFormat("{0} has some combat skills", character.name);
            foreach (var s in character.Data.CombatSkills_)
            {
                if (s.TriggerEvent_ == phase)
                {
                    Debug.LogFormat("Executing {0}'s skill {1} in phase {2}", character.name, s.name, phase);

                    s.Execute(this);
                    if(uiActive)
                    {
                        yield return uiHandleSkill_(s, GetSide(character));
                    }
                }
            }

            //yield return new WaitForSeconds(.5f);
            //Debug.Log("Waiting in " + phase);
        }

        /// <summary>
        /// Which side of the combat animation the character is on ( left or right ).
        /// </summary>
        int GetSide(MapCharacter c)
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

        /// <summary>
        /// Clear the ui delegates
        /// </summary>
        public void UnhookUI()
        {
            uiWaitForEvent_ = null;
            uiPlayAnimation_ = null;
            uiHandleSkill_ = null;
        }

    }

}