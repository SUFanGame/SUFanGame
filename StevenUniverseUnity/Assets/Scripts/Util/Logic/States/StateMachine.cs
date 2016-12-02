using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SUGame.StrategyMap;
using SUGame.StrategyMap.Players;
using SUGame.StrategyMap.Characters.Actions.UIStates;

namespace SUGame.Util.Logic.States
{
    [System.Serializable]
    public class StateMachine
    {
        [SerializeField]
        List<State> stack_ = new List<State>();

        public int StateCount { get { return stack_.Count; } }

        /// <summary>
        /// The player to whom this state machine belongs.
        /// </summary>
        public HumanPlayer Player { get; private set; }

        public State TopState
        {
            get
            {
                int count = stack_.Count;
                return count == 0 ? null : stack_[count - 1];
            }
        }

        public StateMachine( HumanPlayer player )
        {
            Player = player;
            Push(new StrategyMapIdleUIState());
        }

        /// <summary>
        /// Ticks the state at the top of the stack.
        /// </summary>
        public IEnumerator TickRoutine()
        {
            while( true )
            {
                int topIndex = stack_.Count - 1;
                
                if( stack_.Count > 0 && !stack_[topIndex].Paused_ )
                {
                    // Tick the top state if it's active
                    yield return stack_[topIndex].Tick();
                }

                if( stack_.Count == 0 )
                {
                    Push(new StrategyMapIdleUIState());
                }

                yield return null;

                //if ( stack_.Count == 0 || stack_[topIndex].Paused_ )
                //    yield return null;
                //else
                //    yield return stack_[topIndex].Tick();
            }
        }

        /// <summary>
        /// Pop the top state from the top of the stack and unpause the state below it (if any)
        /// </summary>
        public void Pop()
        {
            int topIndex = stack_.Count - 1;
            var topState = stack_[topIndex];
            stack_.RemoveAt(topIndex);

            //topState.OnStack = false;
            topState.OnExit();

            if (stack_.Count == 0)
                return;

            stack_[stack_.Count - 1].Paused_ = false;
        }

        /// <summary>
        /// Push the given state onto the state machine. The state will be ticked each frame unless it's paused.
        /// </summary>
        public void Push( State state )
        {
            state.Machine = this;

            int topIndex = stack_.Count - 1;

            if( stack_.Count != 0 )
            {
                stack_[topIndex].Paused_ = true;
            }

            stack_.Add(state);

            state.OnEnter();
        }

        /// <summary>
        /// Replaces the top state without touching state below it.
        /// </summary>
        /// <param name="newState">The new state that will replace the current top state.</param>
        public void ReplaceTop( State newState )
        {
            int topIndex = stack_.Count - 1;
            var oldTop = stack_[topIndex];
            
            stack_.RemoveAt(topIndex);

            //oldTop.OnStack = false;
            oldTop.OnExit();

            stack_.Add(newState);

            newState.OnEnter();
        }

        /// <summary>
        /// Reset the state of the stack.
        /// </summary>
        public void Clear()
        {
            stack_.Clear();
        }

        #region InputHandlers
        public void OnAccept()
        {
            var topState = TopState;

            if (topState != null)
                topState.OnAcceptInput(Player);
        }

        public void OnCancel()
        {
            var topState = TopState;

            if(topState != null )
                topState.OnCancelInput(Player);
        }

        public void OnUnitSelected( MapCharacter character )
        {
            var topState = TopState;

            if (topState != null)
                topState.OnCharacterSelected(Player, character);
        }

        public void OnPointSelected( Node node )
        {
            var topState = TopState;

            if (topState != null)
                topState.OnPointSelected(Player, node);
        }
        #endregion
    }
}
