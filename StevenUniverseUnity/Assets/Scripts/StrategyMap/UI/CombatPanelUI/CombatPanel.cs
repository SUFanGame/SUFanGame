using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CombatPanel : MonoBehaviour 
{
    public Animator[] animators_;

    void Awake()
    {

        StartCoroutine(PlayAnims(animators_));
    }

    IEnumerator PlayAnims( Animator[] animators )
    {


        for( int i = 0; i < animators.Length; ++i )
        {
            var a = animators[i];
            a.SetTrigger("Attack");

            yield return new WaitUntil( ()=>(a.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !a.IsInTransition(0)) );
        }
        


        yield return null;
    }


}
