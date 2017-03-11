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

        Combat currentlyRunningCombat_ = null;

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

            //var cg = CombatPanel.Instance.GetComponent<CanvasGroup>();
            //cg.alpha = 1;

            // TODO: Currently looks pretty as you just see the portraits slide away if the UI
            // initiates combat
            CombatPanel.Initialize(attacker_, defender_);

            CombatPanel.HidePortraits();

            CombatPanel.ShowPanel();

            CombatPanel.ShowTerrains();

            animsComplete_ = false;

            currentlyRunningCombat_ = new Combat(attacker_, defender_);
            currentlyRunningCombat_.SetUICallbacks(CombatPanel.Instance);

            // Not using the Combat Panel for any particular reason, we just need a gameobject to
            // run the coroutine.
            CombatPanel.Instance.StartCoroutine( ResolveCombat(currentlyRunningCombat_) );
            //CombatPanel.BeginAttackAnimations(attacker_.transform.position, defender_.transform.position);
        }

        IEnumerator ResolveCombat( Combat combat )
        {
            yield return combat.Resolve();
            OnAttacksComplete();
        }

        public override void OnExit()
        {
            Debug.Log("Exiting combat state");
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
            currentlyRunningCombat_.UnhookUI();
            animsComplete_ = true;
            //CombatPanel.StopAnimations();
        }

        /// <summary>
        /// Wait until the combat state completes.
        /// </summary>
        public IEnumerator WaitForAnimations()
        {
            var wait = new WaitForSeconds(.01f);
            while(true)
            {
                if( animsComplete_ == false )
                {
                    yield return wait;
                    continue;
                }
                yield break;
            }
        }

    }
}