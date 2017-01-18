using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SUGame.StrategyMap;
using SUGame.StrategyMap.UI.CombatPanelUI;

/// <summary>
/// Used to send animation events back to the UI.
/// </summary>
public class AnimationCallback : MonoBehaviour 
{
    CombatPanelUnit panel_;
    [SerializeField]
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
    /// Current events:
    ///     AttackHit
    ///     AnimComplete
    /// 
    /// Note the int signifies which side the animation is coming from - -1 for left, 1 for right.
    /// </summary>
    public System.Action<string, int> onAnimationEvent_;

    void Awake()
    {
        animator_ = GetComponent<Animator>();
    }
    
    void _AnimationEvent( string eventName )
    {
        if( onAnimationEvent_ != null )
        {
            onAnimationEvent_.Invoke(eventName, side_);
        }
    }

    /// <summary>
    /// An event will be raised when the given animation completes.
    /// </summary>
    public void SignalWhenComplete( string animName )
    {
        StartCoroutine(SignalRoutine(animName));
    }

    IEnumerator SignalRoutine( string animName )
    {
        while( true )
        {
            var state = animator_.GetCurrentAnimatorStateInfo(0);
            if (!state.IsName(animName))
            {
                yield return wait_;
                continue;
            }

            if (state.normalizedTime < 1)
            {
                yield return wait_;
                continue;
            }
            break;
        }

        onAnimationEvent_.Invoke("AnimComplete", side_);
    }
    
    
}
