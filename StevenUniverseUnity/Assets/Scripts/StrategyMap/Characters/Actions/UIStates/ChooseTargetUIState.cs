using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SUGame.Util.Logic.States;
using SUGame.StrategyMap;
using SUGame.StrategyMap.Players;

namespace SUGame.StrategyMap.Characters.Actions.UIStates
{


    public class ChooseTargetUIState : State
    {
        public MapCharacter Actor { get; private set; }
        //public TargetType TargetType { get; private set; }
        public TargetProperties TargetProperties { get; private set; }
        public CharacterAction Action { get; private set; }
        /// <summary>
        /// List of valid targets for this action (if any)
        /// </summary>
        public IList<MapCharacter> ValidTargets { get; private set; }

        // Selected target.
        MapCharacter Target { get; set; }

        /// <summary>
        /// Callback to execute the action once a target has been selected.
        /// </summary>
        System.Func<MapCharacter, IEnumerator> ActionCallback;

        /// <summary>
        /// State for choosing the target of an action. This will read the given AttackProperties
        /// and provide the appropriate UI for the player to choose a target, whether it be an ally, an enemy, terrain, or the unit itself.
        /// </summary>
        public ChooseTargetUIState(
            MapCharacter actor,
            CharacterAction action,
            TargetProperties targetProperties,
            System.Func<MapCharacter, IEnumerator> actionCallback,
            IList<MapCharacter> validTargets = null)
        {
            Actor = actor;
            TargetProperties = targetProperties;
            Action = action;
            ValidTargets = validTargets;
            ActionCallback = actionCallback;
            //this.TargetType = targetType;

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
            if (ValidTargets != null)
            {
                for (int i = 0; i < ValidTargets.Count; ++i)
                {
                    var sprite = ValidTargets[i].GetComponentInChildren<SpriteRenderer>();
                    sprite.color = Color.red;
                }
            }
        }

        public override void OnExit()
        {
            base.OnExit();


            if (ValidTargets != null)
            {
                for (int i = 0; i < ValidTargets.Count; ++i)
                {
                    var sprite = ValidTargets[i].GetComponentInChildren<SpriteRenderer>();
                    if (sprite.color == Color.red)
                        sprite.color = Color.white;
                }
            }
        }

        public override IEnumerator Tick()
        {

            if( Target != null )
            {
                OnExit();
                Machine.Push(new ConfirmTargetUIState(Actor, Target, ActionCallback));
                Target = null;
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
            if (ValidTargets == null)
            {
                Debug.LogErrorFormat("Valid Targets was null, ensure action \"{0}\" is properly assigning ValidTargets array", Action.UIName);
            }
            if (ValidTargets.Contains(target))
            {
                Target = target;
            }
        }

        // override 


    }
}