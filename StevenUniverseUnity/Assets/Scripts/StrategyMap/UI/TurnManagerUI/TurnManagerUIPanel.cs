using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class TurnManagerUIPanel : MonoBehaviour 
{
    Text whoseTurnText_;
    Animator animator_;

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
