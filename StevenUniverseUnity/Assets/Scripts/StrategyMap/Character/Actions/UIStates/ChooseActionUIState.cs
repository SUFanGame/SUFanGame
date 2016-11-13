using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using StevenUniverse.FanGame.StrategyMap;
using StevenUniverse.FanGame.StrategyMap.UI;

namespace StevenUniverse.FanGame.Util.Logic.States
{
    public class ChooseActionUIState : State
    {
        MapCharacter actor_;

        /// <summary>
        /// The action selected by the player.
        /// </summary>
        CharacterAction action_ = null;

        public ChooseActionUIState( MapCharacter actor )
        {
            actor_ = actor;
        }

        public override void OnEnter()
        {
            base.OnEnter();

            CharacterActionsUI.OnActionSelected_ += OnActionSelected;

            CharacterActionsUI.Show(actor_);
        }

        public override void OnExit()
        {

            CharacterActionsUI.OnActionSelected_ -= OnActionSelected;

            CharacterActionsUI.Hide();

            base.OnExit();
        }

        /// <summary>
        /// Called when the player selects an action from the ActionsUI menu.
        /// </summary>
        void OnActionSelected( CharacterAction action )
        {
            action_ = action;
        }

        public override IEnumerator Tick()
        {
            // Track when the player selects an action from the UI menu.
            if( action_ != null )
            {
                var nextState = action_.GetUIState();
                // Push the action's UI state onto our stack.
                if ( nextState != null )
                {
                    Machine.Push(action_.GetUIState());
                }
                else
                {
                    yield return action_.Execute();
                    actor_.Paused_ = true;
                    Machine.Clear();
                }
                
                action_ = null;
            }
            yield return null;
        }


        //public override IEnumerator Tick()
        //{
        //    while( !stopTicking_ )
        //    {
        //        if (Input.GetButtonDown("Cancel"))
        //        {
        //            Machine.Pop();
        //        }

        //        var action = CharacterActionsUI.SelectedAction;

        //        // Check if a character action was selected.
        //        if (action != null)
        //        {
        //            var state = action.GetUIState();
        //            if (state != null)
        //            {
        //                // If so, we'll push that action's UI state (if any) onto our state machine.
        //                Machine.Push(state);
        //            }
        //            else
        //            {
        //                // If the action has no UI state the default behaviour is to clear the stack.
        //                Machine.Clear();
        //            }

        //        }

        //        yield return null;
        //    }
        //}
    }
}
