using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SUGame.Util.Logic.States;
using SUGame.StrategyMap.UI.CombatPanelUI;
using SUGame.StrategyMap.Players;

namespace SUGame.StrategyMap.Characters.Actions.UIStates
{
    public class CombatUIState : State
    {
        MapCharacter attacker_;
        MapCharacter defender_;
       // System.Func<MapCharacter, IEnumerator> actionCallback_;

        bool animsComplete_ = false;

        public CombatUIState( 
            MapCharacter attacker, 
            MapCharacter defender
            //System.Func<MapCharacter, IEnumerator> actionCallback 
            )
        {
            attacker_ = attacker;
            defender_ = defender;
            //actionCallback_ = actionCallback;
        }

        public override void OnEnter()
        {
            base.OnEnter();

            CombatPanel.OnAttackAnimsComplete_ += OnAttacksComplete;

            CombatPanel.HidePortraits();

            CombatPanel.BeginAttackAnimations(attacker_.transform.position, defender_.transform.position);
        }

        public override void OnExit()
        {
            base.OnExit();

            CombatPanel.OnAttackAnimsComplete_ -= OnAttacksComplete;
            CombatPanel.Clear();

        }

        void OnAttacksComplete()
        {
            animsComplete_ = true;
        }

        public override IEnumerator Tick()
        {
            if( animsComplete_ )
            {
                animsComplete_ = false;

                OnExit();
                //yield return actionCallback_.Invoke(defender_);
                Machine.Clear();
            }


            yield return base.Tick();
        }

        public override void OnCancelInput(StrategyPlayer player)
        {
            animsComplete_ = true;
            CombatPanel.StopAnimations();
        }

    }
}