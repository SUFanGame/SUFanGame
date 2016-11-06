using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace StevenUniverse.FanGame.Util.Logic.States
{
    public class StateMachine
    {
        List<State> stack_ = new List<State>();

        public int StateCount { get { return stack_.Count; } }

        /// <summary>
        /// Ticks the state at the top of the stack.
        /// </summary>
        public IEnumerator TickRoutine()
        {
            while( true )
            {
                int topIndex = stack_.Count - 1;

                if ( stack_.Count == 0 || stack_[topIndex].Paused_ )
                    yield return null;
                else
                    yield return stack_[topIndex].Tick();
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


    }
}
