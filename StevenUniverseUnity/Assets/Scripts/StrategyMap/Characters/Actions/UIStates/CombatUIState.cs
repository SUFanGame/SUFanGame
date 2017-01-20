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

            CombatPanel.HidePortraits();

            CombatPanel.ShowTerrains();

            var combat = new Combat(attacker_, defender_);
            combat.SetUICallbacks(CombatPanel.Instance);

            // Not using the Combat Panel for any particular reason, we just need a gameobject to
            // run the coroutine.
            CombatPanel.Instance.StartCoroutine( ResolveCombat(combat) );
            //CombatPanel.BeginAttackAnimations(attacker_.transform.position, defender_.transform.position);
        }

        IEnumerator ResolveCombat( Combat combat )
        {
            yield return combat.Resolve();
            OnAttacksComplete();
        }

        public override void OnExit()
        {
            base.OnExit();

            CombatPanel.OnAttackAnimsComplete_ -= OnAttacksComplete;
            CombatPanel.Clear();

        }

        void OnAttacksComplete()
        {
            //Debug.Log("Attacks completed");
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
                attacker_.Paused_ = true;
            }


            yield return base.Tick();
        }

        public override void OnCancelInput(StrategyPlayer player)
        {
            animsComplete_ = true;
            //CombatPanel.StopAnimations();
        }

    }
}