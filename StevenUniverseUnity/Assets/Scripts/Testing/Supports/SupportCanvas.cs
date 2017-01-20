using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class SupportCanvas : MonoBehaviour 
{
    [SerializeField]
    Text spokenText_;

    [SerializeField]
    Image[] portraits_;

    [SerializeField]
    GameObject[] namePlates_;
    Text[] namePlateTexts_;

    void Awake()
    {
        namePlateTexts_ = new Text[2];
        for( int i = 0; i < 2; ++i )
        {
            namePlateTexts_[i] = namePlates_[i].GetComponentInChildren<Text>();
        }
    }

    public void SetPortrait( int side, Sprite sprite )
    {
        portraits_[side].sprite = sprite;
    }

    public void SetDialogue(int side, string speaker, string dialogue)
    {
        namePlates_[1 - side].SetActive(false);
        namePlates_[side].SetActive(true);
        namePlateTexts_[side].text = speaker;
        spokenText_.text = dialogue;
    }

    public void SetPortraits( Sprite left, Sprite right )
    {
        portraits_[0].sprite = left;
        portraits_[1].sprite = right;
    }
}
