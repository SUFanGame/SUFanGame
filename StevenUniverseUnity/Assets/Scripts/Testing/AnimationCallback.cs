using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SUGame.StrategyMap;
using SUGame.StrategyMap.UI.CombatPanelUI;

public class AnimationCallback : MonoBehaviour 
{
    CombatPanelUnit panel_;
    [SerializeField]
    Animator target_;

    /// <summary>
    /// Which side is the animation coming from? -1 for left, 1 for right.
    /// This seems pretty unintuitive but allows us to directly use the argument in
    /// calculations (like determining which way to shake the screen on hits for example)
    /// </summary>
    [SerializeField]
    int side_;

    /// <summary>
    /// A callback which can be hooked into to respong to combat animation events.
    /// Current events:
    ///     AttackHit
    /// 
    /// Note the int signified which side the animation is coming from - -1 for left, 1 for right.
    /// </summary>
    public System.Action<string, int> onAnimationEvent_;


    void _AnimationEvent( string eventName )
    {
        if( onAnimationEvent_ != null )
        {
            onAnimationEvent_.Invoke(eventName, side_);
        }
    }

    void TargetHit()
    {
    }
    
}
