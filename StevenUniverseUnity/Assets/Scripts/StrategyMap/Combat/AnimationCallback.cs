using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SUGame.StrategyMap;
using SUGame.StrategyMap.UI.CombatPanelUI;
namespace SUGame.StrategyMap
{
    /// <summary>
    /// Receives animation events from the combat sprite and forwards them
    /// to the Combat Panel via <seealso cref="onAnimationEvent_"/>. 
    /// </summary>
    public class AnimationCallback : MonoBehaviour
    {
        CombatPanelUnit panel_;
        Animator animator_;

        WaitForSeconds wait_ = new WaitForSeconds(.1f);

        /// <summary>
        /// Which side is the animation coming from? -1 for left, 1 for right.
        /// This seems pretty unintuitive but allows us to directly use the argument in
        /// calculations (like determining which way to shake the screen on hits for example)
        /// </summary>
        [SerializeField]
        int side_;

        /// <summary>
        /// A callback which can be hooked into to respond to combat animation events.
        /// Note the int signifies which side the animation is coming from: -1 for left, 1 for right.
        /// </summary>
        public System.Action<CombatAnimEvent, int> onAnimationEvent_;

        void Awake()
        {
            animator_ = GetComponent<Animator>();
        }

        /// <summary>
        /// Animations should target these functions to generate animation events at runtime.
        /// </summary>
        /// <param name="evt"></param>
        void _AnimationEvent(CombatAnimEvent evt)
        {
            if (onAnimationEvent_ != null)
            {
                onAnimationEvent_(evt, side_);
            }
        }

        /// <summary>
        /// An event will be raised when the given animation completes.
        /// </summary>
        public void SignalWhenComplete(string animName)
        {
            //Debug.Log("Calling startcoroutine");
            StartCoroutine(SignalRoutine(animName));
        }


        /// <summary>
        /// Monitor the current animation and signal when it completes.
        /// </summary>
        /// <param name="animName"></param>
        /// <returns></returns>
        IEnumerator SignalRoutine(string animName)
        {
            // Must allow the animator to be ticked by the engine after calling animator_.Play
            // Only then will get proper state information.
            yield return null;
            //Debug.Log("Signal routine has been called");
            while (true)
            {
                var state = animator_.GetCurrentAnimatorStateInfo(0);
                if (!state.IsName(animName))
                {
                    //Debug.LogFormat("Waiting until current anim is {0}.", animName );
                    yield return wait_;
                    continue;
                }

                if (state.normalizedTime < 1)
                {
                    //Debug.LogFormat("Waiting until normalizedTime >= 1. Current: {0}", state.normalizedTime);
                    yield return wait_;
                    continue;
                }

                break;
            }
        
            //Debug.Log("SIGNAL ROUTINE COMPLETE");
            onAnimationEvent_(CombatAnimEvent.ANIM_COMPLETE, side_);
        }


    }
}