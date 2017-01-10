using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace SUGame.StrategyMap.UI
{
    public class TurnManagerUIPanel : MonoBehaviour
    {
        Text whoseTurnText_;
        Animator animator_;

        /// <summary>
        /// Returns true if the turn transition animation is playing.
        /// </summary>
        public bool Animating_
        {
            get
            {
                var state = animator_.GetCurrentAnimatorStateInfo(0);
                return state.IsName("TurnManagerFade");
            }
        }

        void Awake()
        {
            whoseTurnText_ = GetComponentInChildren<Text>();
            animator_ = GetComponentInChildren<Animator>();
        }

        public void SetText(string text)
        {
            whoseTurnText_.text = text;
        }

        public void DoFadeAnim()
        {
            animator_.SetTrigger("FadeAnim");
        }

    }
}