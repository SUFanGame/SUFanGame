using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SUGame.Util.Logic.States;
using SUGame.StrategyMap;
using SUGame.StrategyMap.Players;
using System;

namespace SUGame.StrategyMap.Characters.Actions.UIStates
{


    public class ChooseTargetUIState : State
    {
        MapCharacter actor_;
        //public TargetType TargetType { get; private set; }
        public TargetProperties TargetProperties { get; private set; }
        //public CharacterAction Action_ { get; private set; }
        /// <summary>
        /// List of valid targets for this action (if any)
        /// </summary>
        public IList<MapCharacter> ValidTargets_ { get; private set; }

        // Selected target.
        MapCharacter Target_ { get; set; }

        /// <summary>
        /// Callback to execute the action once a target has been selected.
        /// </summary>
        //System.Func<MapCharacter, IEnumerator> ActionCallback_;
        public Type nextState_; //State type to be instantiated later

        /// <summary>
        /// State for choosing the target of an action. This will read the given AttackProperties
        /// and provide the appropriate UI for the player to choose a target, whether it be an ally, an enemy, terrain, or the unit itself.
        /// </summary>
        public ChooseTargetUIState(
            MapCharacter actor,
            //CharacterAction action,
            TargetProperties targetProperties,
            //System.Func<MapCharacter, IEnumerator> actionCallback,
            Type nextState,
            IList<MapCharacter> validTargets = null)
        {
            actor_ = actor;
            TargetProperties = targetProperties;
            //Action_ = action;
            ValidTargets_ = validTargets;
            nextState_ = nextState;
            //ActionCallback_ = actionCallback;
            //this.TargetType = targetType;
            if( actor == null )
            {
                Debug.Log("Actor is null in choose target constructor");
            }

        }

        public override void OnEnter()
        {
            base.OnEnter();

            var targetType = TargetProperties.type_;

            if ((targetType & TargetType.ENEMY) == TargetType.ENEMY)
            {
                //Debug.LogFormat("This action targets enemies");
            }

            if ((targetType & TargetType.ALLY) == TargetType.ALLY)
            {
                //Debug.LogFormat("This action targets allies");
            }

            if ((targetType & TargetType.SELF) == TargetType.SELF)
            {
                //Debug.LogFormat("This action targets self");
            }

            // TODO : Change this so if we're targeting an ally it colors them blue.
            if (ValidTargets_ != null)
            {
                for (int i = 0; i < ValidTargets_.Count; ++i)
                {
                    var sprite = ValidTargets_[i].GetComponentInChildren<SpriteRenderer>();
                    sprite.color = Color.red;
                }
            }
        }

        public override void OnExit()
        {
            base.OnExit();


            if (ValidTargets_ != null)
            {
                for (int i = 0; i < ValidTargets_.Count; ++i)
                {
                    var sprite = ValidTargets_[i].GetComponentInChildren<SpriteRenderer>();
                    if (sprite.color == Color.red)
                        sprite.color = Color.white;
                }
            }
        }

        public override IEnumerator Tick()
        {

            if( Target_ != null )
            {
                OnExit();
                //Debug.LogFormat("Actor null when calling new ConfirmTarget from ChooseTarget {0}", actor_ == null);
                // This statement assumes that the only states that'll get passed only need actor and target as params
                Machine.Push((State)System.Activator.CreateInstance(nextState_, new System.Object[] { actor_, Target_ }));
                //Debug.Log("Passed along next state");
                Target_ = null;
            }

            yield return null;
            //if (Target != null)
            //{
            //    yield return ActionCallback(Target);

            //    OnExit();

            //    Machine.Clear();
            //}

            //return base.Tick();
        }

        // Note this only accounts for abilities that target characters...what about an ability
        // that targets a point? That could be a separate state maybe?
        public override void OnCharacterSelected(StrategyPlayer player, MapCharacter target)
        {
            if (ValidTargets_ == null)
            {
                //Debug.LogErrorFormat("Valid Targets was null, ensure action \"{0}\" is properly assigning ValidTargets array", Action_.UIName);
            }
            if (ValidTargets_.Contains(target))
            {
                Target_ = target;
            }
        }

        // override 


    }
}