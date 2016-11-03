using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace StevenUniverse.FanGame.StrategyMap
{
    public class DanceAction : CharacterAction
    {
        bool dancing_ = false;
        float rotSpeed_ = 5f;
        float rotIncrement_ = 1.5f;

        public override string UIName
        {
            get
            {
                return dancing_ ? "Stop Dancing" : "Dance";
            }
        }

        protected override IEnumerator Routine()
        {
            if (!dancing_)
            {
                dancing_ = true;
                StartCoroutine(DANCE());
            }
            else
            {
                rotSpeed_ += rotIncrement_;
                Debug.LogFormat("CAN'T STOP THE FEELING!");
            }

            yield return base.Routine();
        }

        IEnumerator DANCE()
        {
            while( true )
            {
                var spriteObject = GetComponentInChildren<SpriteRenderer>();
                spriteObject.transform.RotateAround(transform.position + Vector3.one * .5f, Vector3.forward, rotSpeed_);
                yield return null;
            }
        }
    }
}