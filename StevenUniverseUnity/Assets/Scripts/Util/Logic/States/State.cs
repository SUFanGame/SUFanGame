using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace StevenUniverse.FanGame.Util.Logic.States
{
    public class State
    {
        protected StateMachine machine_;

        public State( StateMachine machine )
        {
            machine_ = machine;
        }

        /// <summary>
        /// This will be polled by the state machine after ticking. If true, states below
        /// this one on the stack will not be ticked.
        /// </summary>
        public virtual bool HaltTicking { get; protected set; }

        /// <summary>
        /// This will be polled by the state machine after reading Input. If true, states below
        /// this one on the stack will not receieve input.
        /// </summary>
        public virtual bool HaltInput { get; protected set; }


        private bool paused_ = false;
        public virtual bool Paused_
        {
            get
            {
                return paused_;
            }
            set
            {
                if( value != paused_ )
                {
                    string name = GetType().Name;
                    if (value == false)
                        Debug.LogFormat("Unpausing {0}", name);
                    else
                        Debug.LogFormat("Pausing {0}", name);
                }
                paused_ = value;
            }
        }

        /// <summary>
        /// Read input from unity.
        /// </summary>
        public virtual void HandleInput()
        {

        }
        
        public virtual void OnEnter()
        {
            Debug.LogFormat("Entering {0}", GetType().Name);
        }

        public virtual void OnExit()
        {
            Debug.LogFormat("Exiting {0}", GetType().Name);
        }
        
        /// <summary>
        /// If the state above this on the stack (if any) hasn't halted ticking
        /// this will be called once per frame by the state machine.
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerator Tick()
        {
            if (Paused_)
                yield break;
        }


    }
}
