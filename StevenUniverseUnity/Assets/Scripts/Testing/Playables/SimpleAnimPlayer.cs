using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Experimental.Director;
using UnityEngine.EventSystems;

public class SimpleAnimPlayer : MonoBehaviour, IPointerDownHandler
{
    Animator animator_;
    RuntimeAnimatorController runtimeController_;
    public AnimationClip clipToPlay_;
    public AnimationClip otherClip_;
    public PlayState playState_;
    public float speed_ = 1f;

    AnimationClipPlayable playable_;
    AnimationClipPlayable secondPlayable_;

    AnimatorControllerPlayable c_;

    Animation anim_;

    void Awake()
    {
        animator_ = GetComponent<Animator>();
        anim_ = GetComponent<Animation>();
        
        runtimeController_ = GetComponent<RuntimeAnimatorController>();
        c_ = AnimatorControllerPlayable.Create(runtimeController_);
        
        //animator_.Play(c_);
        //playable_ = AnimationClipPlayable.Create(clipToPlay_);
        //secondPlayable_ = AnimationClipPlayable.Create(otherClip_);
        //c_= AnimatorControllerPlayable.Create()
        //animator_.Play(playable_);
        //animator_.Play(secondPlayable_);
        OnValidate();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        var id = eventData.button == PointerEventData.InputButton.Left ?
            Animator.StringToHash("Idle") : Animator.StringToHash(otherClip_.name);

        Debug.Log("Get clicked on");
        //if (eventData.button == PointerEventData.InputButton.Left)
        //{
        //    Debug.Log("Left click");
        //    //anim_.play
        //    //anim_[clipToPlay_.name].speed = 1;
        //    //anim_.Play(clipToPlay_.name, PlayMode.StopAll);
        //    //c_.Play("MarthIdle".GetHashCode());
        //}
        //else if (eventData.button == PointerEventData.InputButton.Right)
        //{
        //    anim_.Play("MarthMoveRight");
        //    //c_.Play("marthmoveright");
        //}

        animator_.Play(id, -1, 0);
        animator_.speed = 1;
    }
    
    void OnValidate()
    {
        if( !playable_.IsValid() )
        {
            return;
        }

        playable_.state = playState_;
        playable_.speed = speed_;
    }
}
