using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace StevenUniverse.FanGame.Util.Logic.States
{
    public class State
    {
        public virtual bool Paused_ { get; set; }

        public virtual void OnEnter()
        {

        }

        public virtual void OnExit()
        {

        }
        
        public virtual IEnumerator Tick()
        {
            if (Paused_)
                yield break;
        }
    }
}
