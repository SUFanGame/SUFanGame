using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SUGame.Util.Logic.States;
using SUGame.StrategyMap.UI.ConfirmTargetUI;
using SUGame.StrategyMap.Players;

namespace SUGame.StrategyMap.Characters.Actions.UIStates
{
    public class ConfirmTargetUIState : State
    {
        MapCharacter actor_;
        MapCharacter target_;
        System.Func<MapCharacter, IEnumerator> actionCallback_;

        bool accepted_ = false;

        public ConfirmTargetUIState(MapCharacter actor, MapCharacter target, System.Func<MapCharacter,IEnumerator> actionCallback ) : base()
        {
            actor_ = actor;
            target_ = target;
            actionCallback_ = actionCallback;
        }

        public override void OnEnter()
        {
            base.OnEnter();


            ConfirmTargetPanel.SetAttacker(actor_);
            ConfirmTargetPanel.SetDefender(target_);
            ConfirmTargetPanel.Toggle();
        }

        public override void OnExit()
        {
            base.OnExit();

            ConfirmTargetPanel.Toggle();
        }

        public override void OnAcceptInput(StrategyPlayer player)
        {
            base.OnAcceptInput(player);

            accepted_ = true;
        }

        public override IEnumerator Tick()
        {
            if( accepted_ )
            {
                OnExit();
                //Debug.Log("Accepted input");
                yield return actionCallback_.Invoke(target_);
                Machine.Clear();
            }

            yield return null;
        }
    }
}
