using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using StevenUniverse.FanGame.StrategyMap;
using StevenUniverse.FanGame.StrategyMap.Players;

namespace StevenUniverse.FanGame.Util.Logic.States
{
    /// <summary>
    /// A simple state machine, currently used for progression through the character selection/action UI.
    /// States are kept on a stack. Top state is considered "active" and is ticked each frame.
    /// States can pop themselves off the stack, push new states or replace themselves with new states.
    /// </summary>
    [System.Serializable]
    public class State
    {
        [System.NonSerialized]
        private StateMachine machine_;
        public StateMachine Machine
        {
            get { return machine_; }
            set
            {
                machine_ = value;
            }
        }

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
                    if (value == true)
                        OnPaused();
                    else
                        OnPaused();
                }
                paused_ = value;
            }
        }

        protected bool stopTicking_ = false;
        
        public virtual void OnEnter()
        {
            Debug.LogFormat("Entering {0}", GetType().Name);
        }

        /// <summary>
        /// What to do if the previous state is cancelled (via player input ) and this one becomes active again. 
        /// Note this is not what should happen if THIS STATE gets cancelled.
        /// </summary>
        public virtual void OnUndo()
        {

        }

        public virtual void OnExit()
        {
            stopTicking_ = true;
            Debug.LogFormat("Exiting {0}", GetType().Name);
        }
        
        /// <summary>
        /// Called once per frame by the state machine on whichever state is on top of the stack.
        /// This is expected to process player input and perform actions each frame.
        /// </summary>
        public virtual IEnumerator Tick()
        {
            yield return null;
        }

        /// <summary>
        /// Optional function to handle input from the player. 
        /// Input will be forwarded from the state machine to whichever state is currently active.
        /// </summary>
        public virtual void OnCharacterSelected( StrategyPlayer player, MapCharacter character )
        {

        }

        /// <summary>
        /// Optional function to handle when a player selects a point on the map. 
        /// Input will be forwarded from the state machine to whichever state is currently active.
        /// </summary>
        public virtual void OnPointSelected( StrategyPlayer player, Node point )
        {

        }

        /// <summary>
        /// Optional function to handle accept inputs. 
        /// Input will be forwarded from the state machine to whichever state is currently active.
        /// </summary>
        /// <param name="player"></param>
        public virtual void OnAcceptInput( StrategyPlayer player )
        {

        }

        /// <summary>
        /// Optional function to handle cancel inputs.
        /// Input will be forwarded from the state machine to whichever state is currently active.
        /// </summary>
        /// <param name="player"></param>
        public virtual void OnCancelInput( StrategyPlayer player )
        {
            Machine.Pop();
        }

        public virtual void OnPaused()
        {
            string name = GetType().Name;
            Debug.LogFormat("Pausing {0}", name);
        }

        public virtual void OnUnpaused()
        {
            string name = GetType().Name;
            Debug.LogFormat("Unpausing {0}", name);
        }

    }
}
