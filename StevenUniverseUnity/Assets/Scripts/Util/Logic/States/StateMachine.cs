using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace StevenUniverse.FanGame.Util.Logic.States
{
    public class StateMachine
    {
        List<State> stack_ = new List<State>();

        /// <summary>
        /// States are ticked in order from top to bottom. If a State.Tick returns false then lower states will not receive
        /// ticks.
        /// </summary>
        public IEnumerator Tick()
        {
            for( int i = stack_.Count - 1; i >= 0; --i )
            {
                var state = stack_[i];

                yield return state.Tick();
            }

            yield return null;
        }
    }
}
